---
name: persona-orchestration
description: Select and operationalize repository personas as collaborating roles for non-trivial work. Use when a task needs explicit coordinator, primary executor, and reviewer contributions, especially for ambiguous requests, multi-step implementation, cross-domain changes, architecture or workflow updates, or evidence-based review.
---

# Persona Orchestration

## Overview

Use this skill to turn persona selection into an explicit collaboration loop. Keep the lineup small and require concrete, task-specific outputs from each active persona.

## Workflow

1. Read `.codex/personas/INDEX.md`, then open only the selected personas' bio and responsibility files.
2. Choose the lineup:
   - one coordinator: `systems-analyst` for discovery-heavy work, `tech-lead` for architecture or multi-domain work
   - one primary executor: the persona that owns the dominant delivery surface
   - reviewers only for real risk areas
3. Write a short persona brief:
   - task objective
   - active personas and why they are active
   - concrete contribution expected from each persona
   - evidence that will resolve disagreements
4. Run the collaboration loop:
   - coordinator frames scope, constraints, unknowns, and success criteria
   - primary executor proposes the working approach and required validation
   - each reviewer adds at least one concrete risk, edge case, or acceptance gate
   - coordinator chooses the working plan and records open follow-ups
5. Re-open the loop when scope changes, validation fails, or a new risk domain appears.
6. Close the task with evidence:
   - involve `quality-engineer` when behavior, workflow, or policy changed
   - call out unresolved risks explicitly
   - update notes if the task is non-trivial

## Output Format

```markdown
## Persona Lineup
- Coordinator: <persona + why>
- Primary Executor: <persona + why>
- Reviewers: <persona + why> or none

## Contributions
- <persona> -> <concrete contribution>

## Working Decision
- <chosen path and why it won>

## Re-open Triggers
- <condition that would require new persona input>
```

## Guardrails

- Personas are working lenses, not theatrical voices.
- Do not activate reviewers just to satisfy a checklist.
- Reviewer approval without a concrete challenge is not a valid contribution.
- Keep project-specific constraints in `AGENTS.md` or nested `AGENTS.md`, not in persona cards.
- If personas disagree, resolve the conflict with code, docs, user intent, or validation evidence.

## References

- Read `references/collaboration-patterns.md` when the task type or lineup is unclear.
