---
name: gh-resolve-issue
description: "Triage and resolve GitHub issues in the current repository using the gh CLI. Use when the user asks to work an issue by ID or to pick the first actionable open issue. Always perform triage first, then leave the issue in a terminal automation state for this run: fixed, manually closed, or marked with the automation:nop label so it will be skipped by future automatic picks."
---

# GH Resolve Issue

Resolve repository issues end to end with `gh`, but never skip triage.

## Inputs

- Accept an explicit issue number when the user provides one.
- Accept an issue URL and extract the number if needed.
- If no issue number is provided, select the oldest open issue that does **not** have the `automation:nop` label.

## Core Rules

- Read the repository `AGENTS.md` and any nested `AGENTS.md` files before editing.
- Prefer `rg` for code search and non-interactive `git` commands.
- Never leave the selected issue in a state where the next automatic run would pick it again without new information.
- Use `Fixes #<issue>` or `Closes #<issue>` in the commit body for real fixes.
- Remember the GitHub rule: closing keywords take effect when the commit or PR is merged into the repository default branch.

## Workflow

### 1. Establish GitHub context

Run these checks first:

```powershell
gh auth status
gh repo view --json nameWithOwner,defaultBranchRef
git status --short
```

If the working tree contains unrelated conflicting changes, stop and ask the user how to proceed.

### 2. Ensure the skip label exists

Use `automation:nop` for issues that should not be auto-picked again until a human removes the label.

```powershell
gh label create "automation:nop" --color BFD4F2 --description "Skip automatic issue resolution until a human removes this label." --force
```

### 3. Pick the issue

If the user gave an ID, use it.

If not, fetch the oldest open issue without the skip label:

```powershell
gh issue list --state open --search 'sort:created-asc' --limit 100 --json number,title,url,labels --jq '[.[] | select(all(.labels[]?; .name != "automation:nop"))][0]'
```

If no eligible issue exists, report that clearly and stop.

### 4. Triage before editing

Read the full issue, including comments:

```powershell
gh issue view <issue> --comments
```

Then inspect the codebase, reproduce the problem, and classify the issue into exactly one of these buckets:

1. `fix-needed`
   The repository needs a code, docs, config, or test change.
2. `close-no-change`
   No repository change is needed because the issue is already fixed, invalid, duplicate, obsolete, or not planned.
3. `nop-needed`
   The issue should stay open, but automation should stop touching it until a human intervenes. Typical cases: missing requirements, blocked by external dependency, unsafe to change automatically, or not reproducible with insufficient evidence.

Always leave a short triage summary in your working notes or final handoff.

### 5. Handle the outcome

#### A. `fix-needed`

1. Implement the minimum coherent fix.
2. Run the relevant validation commands.
3. Commit with a message that includes a closing keyword in the body:

```text
fix(scope): short outcome

Fixes #<issue>
```

4. Push to `origin`.

Preferred path:
- If the fix is landing on the default branch, push there and then verify whether the issue closed automatically.

Fallback path:
- If the fix is on a non-default branch, push the branch, open a PR with `Fixes #<issue>` in the PR body, and add the `automation:nop` label plus a comment on the issue so automatic runs do not pick it again before merge.

Useful commands:

```powershell
git branch --show-current
git add <files>
git commit -m "fix(scope): short outcome" -m "Fixes #<issue>"
git push origin HEAD
gh pr create --base <default-branch> --title "fix: short outcome" --body "Fixes #<issue>"
gh issue edit <issue> --add-label "automation:nop"
gh issue comment <issue> --body "A fix has been prepared on branch <branch> and pushed to origin. The issue is marked automation:nop until the default-branch merge completes."
```

#### B. `close-no-change`

Close the issue explicitly with a comment and the appropriate reason:

```powershell
gh issue close <issue> --reason completed --comment "Closing after triage: no repository change is required because <reason>."
```

Use `--reason not planned` when the right outcome is to decline the work rather than mark it complete.

#### C. `nop-needed`

Keep the issue open, but make it ineligible for future automatic picks:

```powershell
gh issue edit <issue> --add-label "automation:nop"
gh issue comment <issue> --body "Automation triage summary: <why no safe automatic change was made>. This issue is marked automation:nop until a human removes the label or adds new information."
```

Use this path whenever closing would be misleading but retrying automatically would loop.

### 6. Verify the final issue state

After the action, confirm the issue status:

```powershell
gh issue view <issue> --json number,state,title,url,labels
```

The issue must now be in one of these states:

- `closed`
- `open` with `automation:nop`

Anything else is incomplete.

## Guardrails

- Do not skip triage just because the user asked to "fix" an issue.
- Do not make speculative code changes when the issue is really blocked on missing information.
- Do not close an issue as completed if the fix only exists on a non-default branch and has not been delivered yet.
- Do not remove `automation:nop` automatically; a human or a later explicit request should do that.
- If the repository has its own issue workflow, labels, or branch policy, follow it and adapt the commands above rather than fighting it.

## Recommended Companion Skills

- Use `$persona-orchestration` for non-trivial fixes.
- Use `$agent-notepad-lookup` before deep debugging.
- Use `$technical-issue-lookup` when the issue looks like a known failure mode.
- Use `$agent-notepad-capture` after a non-trivial resolution.
- Use `$phase-commit-workflow` when preparing the final commit.
