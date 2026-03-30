[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$failures = 0
$warnings = 0

function Write-Check {
    param([string]$Message)
    Write-Host "[check] $Message"
}

function Write-Pass {
    param([string]$Message)
    Write-Host "[pass]  $Message"
}

function Write-WarnMsg {
    param([string]$Message)
    $script:warnings++
    Write-Warning $Message
}

function Write-Fail {
    param([string]$Message)
    $script:failures++
    Write-Host "[fail]  $Message"
}

function Join-RepoPath {
    param([string]$RelativePath)
    $normalized = $RelativePath.Replace('/', '\')
    return (Join-Path $repoRoot $normalized)
}

function Test-RequiredFile {
    param([string]$RelativePath)
    $fullPath = Join-RepoPath $RelativePath
    if (Test-Path -LiteralPath $fullPath) {
        Write-Pass "File exists: $RelativePath"
    }
    else {
        Write-Fail "Missing required file: $RelativePath"
    }
}

function Test-RequiredDirectory {
    param([string]$RelativePath)
    $fullPath = Join-RepoPath $RelativePath
    if (Test-Path -LiteralPath $fullPath) {
        Write-Pass "Directory exists: $RelativePath"
    }
    else {
        Write-Fail "Missing required directory: $RelativePath"
    }
}

function Test-MarkdownLinkTargets {
    param([string]$RelativePath)

    $fullPath = Join-RepoPath $RelativePath
    if (-not (Test-Path -LiteralPath $fullPath)) {
        return
    }

    $content = Get-Content -LiteralPath $fullPath -Raw
    $sourceDirectory = Split-Path -Path $fullPath -Parent
    $matches = [regex]::Matches($content, '\[[^\]]+\]\(([^)#]+\.md)(#[^)]+)?\)')

    foreach ($match in $matches) {
        $target = $match.Groups[1].Value
        $resolvedTarget = Join-Path $sourceDirectory ($target.Replace('/', '\'))
        if (Test-Path -LiteralPath $resolvedTarget) {
            $relativeTarget = $resolvedTarget.Replace($repoPrefix, "")
            Write-Pass "Linked file exists: $RelativePath -> $relativeTarget"
        }
        else {
            Write-Fail "Broken markdown link in $RelativePath -> $target"
        }
    }
}

function Test-FileNamePattern {
    param(
        [string]$EntriesRelativePath,
        [string]$Pattern,
        [string[]]$IgnoredFiles = @()
    )

    $entriesPath = Join-RepoPath $EntriesRelativePath
    if (-not (Test-Path -LiteralPath $entriesPath)) {
        return
    }

    $entryFiles = Get-ChildItem -Path $entriesPath -File -Filter "*.md" |
        Where-Object { $IgnoredFiles -notcontains $_.Name }

    foreach ($entryFile in $entryFiles) {
        $relativeFile = $entryFile.FullName.Replace($repoPrefix, "")
        if ($entryFile.Name -match $Pattern) {
            Write-Pass "Entry file name valid: $relativeFile"
        }
        else {
            Write-Fail "Entry file name invalid: $relativeFile"
        }
    }
}

function Test-EntrySections {
    param(
        [string]$EntriesRelativePath,
        [string[]]$RequiredHeadings,
        [string[]]$IgnoredFiles = @()
    )

    $entriesPath = Join-RepoPath $EntriesRelativePath
    if (-not (Test-Path -LiteralPath $entriesPath)) {
        return
    }

    $entryFiles = Get-ChildItem -Path $entriesPath -File -Filter "*.md" |
        Where-Object { $IgnoredFiles -notcontains $_.Name }

    foreach ($entryFile in $entryFiles) {
        $content = Get-Content -LiteralPath $entryFile.FullName -Raw
        $relativeFile = $entryFile.FullName.Replace($repoPrefix, "")

        foreach ($heading in $RequiredHeadings) {
            $pattern = "(?m)^## " + [regex]::Escape($heading) + "\r?$"
            if ($content -match $pattern) {
                Write-Pass "Entry heading exists: $relativeFile -> ## $heading"
            }
            else {
                Write-Fail "Missing entry heading in $relativeFile -> ## $heading"
            }
        }
    }
}

Write-Check "Required directories"
$requiredDirectories = @(
    ".codex\notes",
    ".codex\personas",
    ".codex\skills",
    "docs\specs",
    "docs\specs\decisions",
    "docs\specs\decisions\_templates",
    "docs\specs\decisions\entries",
    "docs\specs\technical-issues",
    "docs\specs\technical-issues\_templates",
    "docs\specs\technical-issues\entries",
    "scripts"
)

foreach ($dir in $requiredDirectories) {
    Test-RequiredDirectory $dir
}

Write-Check "Required files"
$requiredFiles = @(
    "AGENTS.md",
    ".codex/personas/INDEX.md",
    ".codex/notes/INDEX.md",
    ".codex/notes/README.md",
    ".codex/notes/_templates/topic-note.md",
    ".codex/notes/_templates/branch-index.md",
    "docs/specs/DECISIONS.md",
    "docs/specs/decisions/INDEX.md",
    "docs/specs/decisions/_templates/decision-entry.md",
    "docs/specs/TECHNICAL_ISSUES.md",
    "docs/specs/technical-issues/INDEX.md",
    "docs/specs/technical-issues/_templates/technical-issue-entry.md",
    "docs/specs/technical-issues/entries/README.md",
    "scripts/bootstrap-template.ps1",
    "scripts/validate-template.ps1"
)

foreach ($file in $requiredFiles) {
    Test-RequiredFile $file
}

$repoPrefix = $repoRoot + [System.IO.Path]::DirectorySeparatorChar

Write-Check "Notes branch indexes"
$notesRoot = Join-RepoPath ".codex\notes"
if (Test-Path -LiteralPath $notesRoot) {
    $noteDirectories = Get-ChildItem -Path $notesRoot -Recurse -Directory |
        Where-Object {
            $_.Name -ne "_templates" -and
            (Get-ChildItem -Path $_.FullName -Recurse -File -ErrorAction SilentlyContinue | Select-Object -First 1)
        }

    foreach ($dir in $noteDirectories) {
        $indexPath = Join-Path $dir.FullName "INDEX.md"
        $relativeIndex = $indexPath.Replace($repoPrefix, "")
        if (Test-Path -LiteralPath $indexPath) {
            Write-Pass "Branch index exists: $relativeIndex"
        }
        else {
            Write-Fail "Missing branch index: $relativeIndex"
        }
    }
}

Write-Check "Spec registry structure"
Test-MarkdownLinkTargets -RelativePath "docs/specs/decisions/INDEX.md"
Test-MarkdownLinkTargets -RelativePath "docs/specs/technical-issues/INDEX.md"
Test-FileNamePattern -EntriesRelativePath "docs/specs/decisions/entries" -Pattern '^DEC-\d{8}-\d{2}-.+\.md$'
Test-FileNamePattern -EntriesRelativePath "docs/specs/technical-issues/entries" -Pattern '^ISSUE-\d{8}-\d{2}-.+\.md$' -IgnoredFiles @("README.md")
Test-EntrySections -EntriesRelativePath "docs/specs/decisions/entries" -RequiredHeadings @("Context", "Options Considered", "Decision", "Consequences", "Validation / Evidence")
Test-EntrySections -EntriesRelativePath "docs/specs/technical-issues/entries" -RequiredHeadings @("Symptoms", "Root Cause", "Fix", "Validation", "Prevention") -IgnoredFiles @("README.md")

Write-Check "Persona structure"
$personasRoot = Join-RepoPath ".codex\personas"
if ((Test-Path -LiteralPath $personasRoot) -and (Test-Path -LiteralPath (Join-RepoPath ".codex\personas\INDEX.md"))) {
    $personaIndexPath = Join-RepoPath ".codex\personas\INDEX.md"
    $personaIndexContent = Get-Content -LiteralPath $personaIndexPath
    $personaNames = @()

    foreach ($line in $personaIndexContent) {
        if ($line -match '^\| `([^`]+)` \|') {
            $personaNames += $matches[1]
        }
    }

    $personaNames = $personaNames | Select-Object -Unique

    if ($personaNames.Count -eq 0) {
        Write-Fail "No personas discovered in .codex/personas/INDEX.md"
    }
    else {
        foreach ($personaName in $personaNames) {
            $personaDirectory = Join-Path $personasRoot $personaName
            $bioPath = Join-Path $personaDirectory "BIO.md"
            $responsibilityPath = Join-Path $personaDirectory "RESPONSIBILITY.md"
            $relativeBio = $bioPath.Replace($repoPrefix, "")
            $relativeResponsibility = $responsibilityPath.Replace($repoPrefix, "")

            if (Test-Path -LiteralPath $bioPath) {
                Write-Pass "Persona bio exists: $relativeBio"
            }
            else {
                Write-Fail "Missing persona bio: $relativeBio"
            }

            if (Test-Path -LiteralPath $responsibilityPath) {
                Write-Pass "Persona responsibility exists: $relativeResponsibility"
            }
            else {
                Write-Fail "Missing persona responsibility: $relativeResponsibility"
            }
        }

        $extraPersonaDirectories = Get-ChildItem -Path $personasRoot -Directory |
            Where-Object { $personaNames -notcontains $_.Name }

        foreach ($dir in $extraPersonaDirectories) {
            $relativeDir = $dir.FullName.Replace($repoPrefix, "")
            Write-WarnMsg "Persona directory not listed in .codex/personas/INDEX.md: $relativeDir"
        }
    }
}

Write-Check "Backtick path references in AGENTS and SKILL files"
$ignoredReferences = @("INDEX.md", "kebab-case.md", "misc.md")
$placeholderReferencePatterns = @("YYYYMMDD", "\bXX\b", "<.+>")
$referenceSources = @((Join-RepoPath "AGENTS.md"))
$referenceSources += Get-ChildItem -Path (Join-RepoPath ".codex\skills") -Recurse -Filter "SKILL.md" |
    Select-Object -ExpandProperty FullName

foreach ($source in $referenceSources) {
    $content = Get-Content -LiteralPath $source -Raw
    $matches = [regex]::Matches($content, '`([^`]+\.(md|yaml|yml))`')
    foreach ($match in $matches) {
        $reference = $match.Groups[1].Value
        if ($ignoredReferences -contains $reference) {
            continue
        }

        $ignorePlaceholder = $false
        foreach ($placeholderPattern in $placeholderReferencePatterns) {
            if ($reference -match $placeholderPattern) {
                $ignorePlaceholder = $true
                break
            }
        }

        if ($ignorePlaceholder) {
            continue
        }

        $candidateFromRoot = Join-RepoPath $reference
        $candidateFromSource = Join-Path (Split-Path $source -Parent) ($reference.Replace('/', '\'))

        if ((Test-Path -LiteralPath $candidateFromRoot) -or (Test-Path -LiteralPath $candidateFromSource)) {
            continue
        }

        $relativeSource = $source.Replace($repoPrefix, "")
        Write-Fail "Broken reference in $relativeSource -> $reference"
    }
}

Write-Check "Skill structure via quick_validate.py"
$pythonCommand = Get-Command python -ErrorAction SilentlyContinue
$quickValidateCandidates = @()

if ($env:USERPROFILE) {
    $quickValidateCandidates += Join-Path $env:USERPROFILE ".codex\skills\.system\skill-creator\scripts\quick_validate.py"
}

if ($env:CODEX_HOME) {
    $quickValidateCandidates += Join-Path $env:CODEX_HOME "skills\.system\skill-creator\scripts\quick_validate.py"
}

$quickValidate = $quickValidateCandidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1

if (-not $pythonCommand) {
    Write-WarnMsg "python not found; skipping skill validation."
}
elseif (-not $quickValidate) {
    Write-WarnMsg "quick_validate.py not found; skipping skill validation."
}
else {
    $skillDirs = Get-ChildItem -Path (Join-RepoPath ".codex\skills") -Directory | Sort-Object Name
    foreach ($skillDir in $skillDirs) {
        $output = & $pythonCommand.Source $quickValidate $skillDir.FullName 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Pass "Skill valid: $($skillDir.Name)"
        }
        else {
            Write-Fail "Skill invalid: $($skillDir.Name) :: $($output -join ' ')"
        }
    }
}

Write-Host ""
Write-Host ("Validation summary: failures={0}, warnings={1}" -f $failures, $warnings)

if ($failures -gt 0) {
    exit 1
}

exit 0
