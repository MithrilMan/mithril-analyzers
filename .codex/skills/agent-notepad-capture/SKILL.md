---
name: agent-notepad-capture
description: Persist operational learning as atomic, topic-specific notes inside the .codex/notes tree. Use during and after tasks to create or update leaf notes, route them into the right branch, link evidence, and keep the note map navigable.
---

# Agent Notepad Capture

Store memory as small files in domain branches.

## Workflow

1. Choose or create the smallest durable branch path in `.codex/notes/` that best matches the topic.
   - Examples only: `architecture/constraints`, `workflow/testing`, `integrations/payments`
   - Use `inbox/` only when the stable branch is still unclear or the evidence is incomplete.
2. If the branch does not exist, create it with an `INDEX.md` and link it from the parent index.
3. Use one file per narrow topic (`kebab-case.md`).
4. Update an existing topic file before creating a new one when that keeps future lookup cheaper and avoids duplication.
5. Keep each note focused and short; split when a file starts covering multiple concerns.
6. Add traceability to each claim:
   - `Status: draft|active|stable|needs-revalidation|superseded`
   - `Confidence: high|medium|low`
   - `Last-Validated: YYYY-MM-DD`
   - `Evidence: <file path | command>`
7. Add lifecycle links when relevant:
   - `Related: <note path | none>`
   - `Supersedes: <note path | none>`
   - `Superseded-By: <note path | none>`
8. Promote statuses intentionally:
   - `draft` for provisional captures or unresolved routing
   - `active` for trusted current guidance
   - `stable` only after repeated validation or durable rule confirmation
   - `needs-revalidation` when the note is useful but time-sensitive, environment-sensitive, or partially stale
9. When new evidence contradicts an old note, update or create the replacement first, then mark the old note as superseded and link the replacement.
10. Update branch `INDEX.md` entries so lookup can discover the note quickly.

## Recommended Note Template

```markdown
# <Topic>

- Status: active
- Confidence: medium
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
- <open checks or next validation>
```

## Guardrails

- Do not create speculative branches for one-off guesses about future work.
- Do not create a new note when the information clearly belongs in an existing topic file.
- Do not keep long rolling journals in a single file.
- Do not store secrets or credentials.
- Do not promote assumptions to facts without evidence.
