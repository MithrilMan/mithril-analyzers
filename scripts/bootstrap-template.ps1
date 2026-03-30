[CmdletBinding()]
param(
    [string[]]$NoteBranches = @("inbox", "archive"),
    [switch]$WithNestedAgents,
    [switch]$Force,
    [switch]$ValidateAfter
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$created = 0
$updated = 0
$skipped = 0

function Join-RepoPath {
    param([string]$RelativePath)
    $normalized = $RelativePath.Replace('/', '\')
    return (Join-Path $repoRoot $normalized)
}

function Ensure-Directory {
    param([string]$RelativePath)
    $fullPath = Join-RepoPath $RelativePath
    if (-not (Test-Path -LiteralPath $fullPath)) {
        New-Item -Path $fullPath -ItemType Directory -Force | Out-Null
        Write-Host "[create] directory $RelativePath"
        $script:created++
    }
}

function Write-FileContent {
    param(
        [string]$RelativePath,
        [string]$Content
    )

    $fullPath = Join-RepoPath $RelativePath
    $parent = Split-Path -Path $fullPath -Parent
    if (-not (Test-Path -LiteralPath $parent)) {
        New-Item -Path $parent -ItemType Directory -Force | Out-Null
        $script:created++
    }

    if (Test-Path -LiteralPath $fullPath) {
        if ($Force) {
            Set-Content -LiteralPath $fullPath -Value $Content -Encoding utf8
            Write-Host "[update] file $RelativePath"
            $script:updated++
        }
        else {
            Write-Host "[skip]   file $RelativePath"
            $script:skipped++
        }
    }
    else {
        Set-Content -LiteralPath $fullPath -Value $Content -Encoding utf8
        Write-Host "[create] file $RelativePath"
        $script:created++
    }
}

function To-TitleCase {
    param([string]$Text)
    $parts = $Text -split "[-_]"
    $formatted = $parts | ForEach-Object {
        if ($_.Length -eq 0) { return $_ }
        return $_.Substring(0, 1).ToUpperInvariant() + $_.Substring(1).ToLowerInvariant()
    }
    return ($formatted -join " ")
}

function Ensure-BranchIndexes {
    param([string[]]$Branches)
    foreach ($branch in $Branches) {
        $cleanBranch = $branch.Trim('/').Trim('\')
        if ([string]::IsNullOrWhiteSpace($cleanBranch)) {
            continue
        }

        $segments = $cleanBranch.Replace('/', '\').Split('\')
        $current = ""
        foreach ($segment in $segments) {
            if ([string]::IsNullOrWhiteSpace($current)) {
                $current = $segment
            }
            else {
                $current = "$current\$segment"
            }

            $branchRelativePath = ".codex/notes/$current"
            Ensure-Directory $branchRelativePath

            $indexRelativePath = "$branchRelativePath/INDEX.md"
            if ($current -eq "inbox") {
                $content = @"
# Inbox Index

## Purpose

- Hold fast captures before final routing to a stable branch.
- Keep provisional notes out of the durable taxonomy until branch choice and evidence are clear.

## Leaf Notes

- Add temporary notes here only when branch is unclear.
- Move notes out after validation or at prune time.

## Rules

- Prefer `Status: draft` for inbox notes unless they are already well-evidenced.
- Do not let inbox become a long-lived catch-all branch.
"@
            }
            elseif ($current -eq "archive") {
                $content = @"
# Archive Index

## Purpose

- Store historical note bodies and structure history that should not participate in normal lookup.
- Keep active branches clean by leaving only short superseded pointers when the original lookup path still matters.

## Files

- `structure-history.md` - path migration notes for note-tree reorganizations.

## Rules

- Normal lookup should ignore `archive/` unless tracing history or validating an old assumption.
- Archived note bodies should link to the active replacement when one exists.
"@
            }
            else {
                $title = To-TitleCase $segment
                $content = @"
# $title Index

## Sub-Branches

- Add child branches here when needed.

## Leaf Notes

- Add focused active leaf notes for this branch.
"@
            }
            Write-FileContent -RelativePath $indexRelativePath -Content $content
        }
    }
}

Ensure-Directory ".codex"
Ensure-Directory ".codex/notes"
Ensure-Directory ".codex/notes/_templates"
Ensure-Directory "docs/specs"
Ensure-Directory "docs/specs/decisions"
Ensure-Directory "docs/specs/decisions/_templates"
Ensure-Directory "docs/specs/decisions/entries"
Ensure-Directory "docs/specs/technical-issues"
Ensure-Directory "docs/specs/technical-issues/_templates"
Ensure-Directory "docs/specs/technical-issues/entries"
Ensure-Directory "scripts"

$notesReadme = @"
# Personal Notes Workspace

This folder is the agent operational memory for this repository.
Use it as a tree of small topic notes, not as a single running journal.

## Structure Model

- Treat durable domain branches as agent-owned and project-specific.
- Reserve only `inbox/`, `archive/`, and `_templates/` as system folders.
- Start from `.codex/notes/INDEX.md` to navigate active branches.
- Keep one narrow topic per leaf file and one `INDEX.md` per branch.

## Routing Heuristics

- Create the smallest durable branch path that will make future lookup cheaper.
- Examples only: `architecture/constraints`, `workflow/testing`, `integrations/payments`.
- Use `inbox/` only for short-lived captures when branch choice or evidence is still unclear.
- Create a new domain branch only after repeated work or repeated notes show a stable retrieval pattern.

## Lifecycle

1. Lookup before non-trivial work.
2. Capture new learning in the best current branch, using `draft` when confidence or routing is still weak.
3. Promote or downgrade note status as evidence becomes stronger, weaker, or older.
4. Prune contradictions and stale notes at phase boundaries.

## Status Meanings

- `draft` - provisional capture, weak evidence, or unresolved routing.
- `active` - currently trusted note that guides day-to-day execution.
- `stable` - durable fact or workflow rule revalidated over time.
- `needs-revalidation` - useful but too stale or environment-sensitive to reuse without a fresh check.
- `superseded` - replaced by a newer note on the same topic.

## Superseded and Archive Policy

- Use `Superseded-By` when one note replaces another on the same topic.
- Keep a short superseded pointer in the original branch when that path is still a common lookup route.
- Move the full historical body to `archive/` only when history still matters.
- Normal lookup should ignore `archive/` and `Status: superseded` unless tracing history.
"@
Write-FileContent -RelativePath ".codex/notes/README.md" -Content $notesReadme

$systemBranches = @("inbox", "archive")
$effectiveNoteBranches = @($systemBranches + $NoteBranches) | Select-Object -Unique

$topBranches = $effectiveNoteBranches |
    ForEach-Object { $_.Replace('/', '\').Split('\')[0] } |
    Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
    Sort-Object -Unique

$systemBranchLines = @()
foreach ($systemBranch in $systemBranches) {
    if ($topBranches -contains $systemBranch) {
        if ($systemBranch -eq "inbox") {
            $systemBranchLines += "- ``inbox/INDEX.md`` - short-lived captures awaiting routing or stronger validation."
        }
        elseif ($systemBranch -eq "archive") {
            $systemBranchLines += "- ``archive/INDEX.md`` - historical note bodies and structure history excluded from normal lookup."
        }
    }
}

$systemBranchLines += "- ``_templates/`` - reusable note skeletons."

$domainBranches = $topBranches | Where-Object { $systemBranches -notcontains $_ }
$domainBranchLines = @()

foreach ($domainBranch in $domainBranches) {
    $domainBranchLines += "- ``$domainBranch/INDEX.md`` - project-specific branch scope placeholder."
}

if ($domainBranchLines.Count -eq 0) {
    $domainBranchLines = @("- Add project-specific domain branches here as they emerge (for example ``architecture/``, ``workflow/``, ``integrations/``).")
}

$notesIndex = @"
# Notes Index

Use this file as the root navigator for the active note tree.
Durable domain branches are project-specific and agent-owned; only `inbox/`, `archive/`, and `_templates/` are reserved system folders.

## System Folders

$($systemBranchLines -join "`n")

## Domain Branches

$($domainBranchLines -join "`n")

## Pinned Leaf Notes

- Add current high-signal leaf notes here.

## Rules

- Create new domain branches only when they reduce future lookup cost.
- Keep one narrow topic per file.
- `Pinned Leaf Notes` is curated, not exhaustive.
- Prefer creating a new child file over growing a mixed note.
- Link every new leaf note in its nearest branch index.
"@
Write-FileContent -RelativePath ".codex/notes/INDEX.md" -Content $notesIndex

$topicTemplate = @"
# <Topic>

- Status: draft
- Confidence: low
- Last-Validated: YYYY-MM-DD
- Evidence: <path or command>
- Related: <note path or none>
- Supersedes: <note path or none>
- Superseded-By: <note path or none>

## Facts

- <single factual statement>

## Implications

- <what this changes for execution>

## Follow-ups

- <next checks>
"@
Write-FileContent -RelativePath ".codex/notes/_templates/topic-note.md" -Content $topicTemplate

$branchTemplate = @"
# <Branch> Index

## Sub-Branches

- ``<child>/INDEX.md`` - <scope>

## Leaf Notes

- ``<topic>.md`` - <what this active note contains>

## Notes

- Keep this index focused on active discovery paths.
- Omit superseded notes unless a short pointer is needed for common lookup paths.
"@
Write-FileContent -RelativePath ".codex/notes/_templates/branch-index.md" -Content $branchTemplate

$decisionsTemplate = @"
# DECISIONS

Project-level decisions only.
This file is the canonical entrypoint for the decisions registry.
Use it as a router for agents, not as a rolling journal.

## Registry Layout

- `docs/specs/decisions/INDEX.md` - searchable catalog of decisions, newest first.
- `docs/specs/decisions/entries/` - one Markdown file per decision.
- `docs/specs/decisions/_templates/decision-entry.md` - template for new decision entries.

## Workflow

1. Read this file, then read `docs/specs/decisions/INDEX.md`.
2. Search the index for overlapping or conflicting accepted decisions.
3. Open only the candidate decision files you need from `docs/specs/decisions/entries/`.
4. Create or update exactly one entry file per durable project decision.
5. Update the index row and keep entries in reverse chronological order.
6. When a decision changes direction, update the old entry to `superseded` and link the replacement entry.

## Naming Rule

- Use `DEC-YYYYMMDD-XX-short-title.md` for entry files.

## Current Catalog

- Open `docs/specs/decisions/INDEX.md`.
"@
Write-FileContent -RelativePath "docs/specs/DECISIONS.md" -Content $decisionsTemplate

$decisionsIndexTemplate = @"
# Decisions Index

Use this catalog to discover relevant project decisions quickly.
Keep entries in reverse chronological order and point each row to exactly one decision file.

| ID | Status | Date | Domain | Summary | File |
| --- | --- | --- | --- | --- | --- |

No decisions logged yet.
"@
Write-FileContent -RelativePath "docs/specs/decisions/INDEX.md" -Content $decisionsIndexTemplate

$decisionEntryTemplate = @"
# [DEC-YYYYMMDD-XX] Short title

- Date: YYYY-MM-DD
- Status: proposed | accepted | superseded
- Domain: architecture | api | auth | data | dependency | workflow | docs | other
- Keywords:
  - keyword
- Supersedes: <DEC-ID or none>
- Superseded-By: <DEC-ID or none>

## Context

- Problem, trigger, or constraint that forced the decision.

## Options Considered

### Option A

- Tradeoffs:
  - Benefits and costs.

### Option B

- Tradeoffs:
  - Benefits and costs.

## Decision

- Final choice.

## Consequences

- Expected impact, guardrails, and follow-up work.

## Validation / Evidence

- Files, commands, or results that justify the decision.
"@
Write-FileContent -RelativePath "docs/specs/decisions/_templates/decision-entry.md" -Content $decisionEntryTemplate

$issuesTemplate = @"
# TECHNICAL ISSUES

Resolved technical incidents only.
This file is the canonical entrypoint for the technical-issue registry.
Use it as a router for agents, not as a rolling journal.

## Registry Layout

- `docs/specs/technical-issues/INDEX.md` - searchable catalog of resolved issues, newest first.
- `docs/specs/technical-issues/entries/` - one Markdown file per resolved issue.
- `docs/specs/technical-issues/_templates/technical-issue-entry.md` - template for new issue entries.

## Workflow

1. Read this file, then read `docs/specs/technical-issues/INDEX.md`.
2. Search the index for matching symptoms, areas, and root-cause patterns.
3. Open only the candidate issue files you need from `docs/specs/technical-issues/entries/`.
4. Update an existing issue file when the signature and root cause match; otherwise create a new entry file.
5. Update the index row and keep entries in reverse chronological order.
6. Log only validated fixes for resolved incidents.

## Naming Rule

- Use `ISSUE-YYYYMMDD-XX-short-title.md` for entry files.

## Current Catalog

- Open `docs/specs/technical-issues/INDEX.md`.
"@
Write-FileContent -RelativePath "docs/specs/TECHNICAL_ISSUES.md" -Content $issuesTemplate

$issuesIndexTemplate = @"
# Technical Issues Index

Use this catalog to discover prior validated fixes quickly.
Keep entries in reverse chronological order and point each row to exactly one resolved-issue file.

| ID | Area | Date | Signature | Root Cause Summary | File |
| --- | --- | --- | --- | --- | --- |

No technical issues logged yet.
"@
Write-FileContent -RelativePath "docs/specs/technical-issues/INDEX.md" -Content $issuesIndexTemplate

$issueEntryTemplate = @"
# [ISSUE-YYYYMMDD-XX] Short title

- Date: YYYY-MM-DD
- Area: frontend | backend | infra | docs | tooling
- Services / Modules:
  - module-or-command
- Keywords:
  - keyword

## Symptoms

- Exact error text, failing command, or visible behavior.

## Root Cause

- Validated technical cause.

## Fix

- What changed, with affected file paths when useful.

## Validation

- Commands, tests, and outcomes that confirmed the fix.

## Prevention

- Guardrails, follow-up checks, or monitoring ideas.
"@
Write-FileContent -RelativePath "docs/specs/technical-issues/_templates/technical-issue-entry.md" -Content $issueEntryTemplate

$technicalIssueEntriesReadme = @"
# Technical Issue Entries

Store one Markdown file per resolved technical incident in this directory.
Keep the searchable catalog in `../INDEX.md`.
"@
Write-FileContent -RelativePath "docs/specs/technical-issues/entries/README.md" -Content $technicalIssueEntriesReadme

Ensure-BranchIndexes -Branches $effectiveNoteBranches

if ($WithNestedAgents) {
    $candidateNestedRoots = @(
        "backend",
        "src",
        "docs",
        "src/shared/ui"
    )

    foreach ($nestedRoot in $candidateNestedRoots) {
        $nestedPath = Join-RepoPath $nestedRoot
        if (Test-Path -LiteralPath $nestedPath) {
            $nestedAgentsContent = @"
# AGENTS

Nested instructions for ``$nestedRoot``.
This file extends root ``AGENTS.md``.
Add subtree-only rules here.
"@
            Write-FileContent -RelativePath "$nestedRoot/AGENTS.md" -Content $nestedAgentsContent
        }
    }
}

Write-Host ""
Write-Host ("Bootstrap summary: created={0}, updated={1}, skipped={2}" -f $created, $updated, $skipped)

if ($ValidateAfter) {
    $validator = Join-RepoPath "scripts/validate-template.ps1"
    if (Test-Path -LiteralPath $validator) {
        & $validator
        exit $LASTEXITCODE
    }
    Write-Warning "validate-template.ps1 not found; validation skipped."
}

exit 0
