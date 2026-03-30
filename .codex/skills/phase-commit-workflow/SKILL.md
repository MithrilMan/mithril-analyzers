---
name: phase-commit-workflow
description: Create high-quality phase commits for this repository. Use when work is executed in multiple phases or when preparing any git commit, so each commit is atomic, validated, clearly messaged, and easy to review or rollback.
---

# Phase Commit Workflow

Apply this workflow whenever a phase is complete and a commit is needed.

## Workflow

1. Define commit scope for the completed phase.
2. Inspect working tree and exclude unrelated changes.
3. Validate the phase (run relevant checks or report what cannot run).
4. Craft a commit message with explicit intent and evidence.
5. Commit only phase files.
6. Verify commit content and leave a short handoff summary.

## Commit Scope Rules

- One commit per completed phase.
- Keep commit atomic and reviewable.
- Do not bundle unrelated refactors.
- Do not stage generated noise unless required by the phase.
- Never include secrets, credentials, or local environment artifacts.

## Message Standard

Use this subject format:

`type(scope): short outcome [phase X/N]`

Recommended `type` values:

- `feat` for new behavior
- `fix` for bug fixes
- `refactor` for structural cleanup without behavior change
- `docs` for documentation-only updates
- `test` for test additions/updates
- `chore` for maintenance tasks

Body template:

```text
Why:
- reason for this phase

What:
- key files/changes

Validation:
- commands run and outcomes
```

## Staging Checklist

Before commit:

1. `git status --short`
2. `git diff --stat`
3. `git add <phase-files-only>`
4. `git diff --cached`

If unrelated files are staged, unstage them before committing.

## Phase Handoff Note

After commit, report:

- commit hash
- phase objective completed
- validations executed
- next phase to run
