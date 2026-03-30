# AGENTS

Default instructions for the whole repository.
Nearest nested `AGENTS.md` overrides and extends these rules.

## Core Rules

- The codebase is the source of truth. Update docs when code and docs diverge.
- Keep scope tight: solve the user task end-to-end with minimal collateral changes.
- Record only project-development decisions in the decisions registry rooted at `docs/specs/DECISIONS.md`.
- If a new decision conflicts with existing decisions, stop and ask the user before updating.

## Project Invariants

- The repository owns the `Mithril.Analyzers` Roslyn package and its supporting docs/tests.
- Keep V1 focused on `BGA001` to `BGA005`; do not pull `BGA100+` heuristics into the same change unless the user explicitly widens scope.
- Use `.editorconfig` as the primary configuration surface for thresholds and behavior. Do not add XML configuration files for V1.
- Package outputs must remain analyzer-only. Do not introduce runtime assets or application-facing dependencies in the produced NuGet package.
- Ignore generated code by default and prefer conservative diagnostics over noisy heuristics.

## Task Execution Loop

Apply this loop for non-trivial work:

1. Select personas with `$persona-orchestration`; assign coordinator, primary executor, required reviewers, and expected contributions.
2. Run context lookup (`$agent-notepad-lookup` and targeted code scan).
3. Validate behavior with relevant checks.
4. Capture new validated learning (`$agent-notepad-capture`).
5. Reconcile stale or conflicting notes (`$agent-notepad-prune`).
6. Commit the completed phase with atomic scope.

## Phase Delivery and Commits

- Split non-trivial work into explicit delivery phases.
- Close each phase with code, related docs/spec updates, and relevant validation.
- Create a dedicated git commit at the end of every completed phase.
- Keep commits atomic: include only files required for that phase.
- If unrelated local changes exist, commit only the phase files and leave unrelated files untouched.
- If unrelated files are already staged, commit with explicit file paths so only phase files are included.
- Skip or defer commits only when the user explicitly requests it.
- Use `$phase-commit-workflow` skill for commit preparation and message quality.

## Personal Notepad

- Maintain a personal operational knowledge base in `.codex/notes/`.
- Treat notes as personal working memory, not as authoritative project specs.
- Consult notes before non-trivial work with `$agent-notepad-lookup`.
- Capture important findings, constraints, and lessons with `$agent-notepad-capture`.
- Prune stale/false/duplicate notes with `$agent-notepad-prune` at phase boundaries.
- Remove or correct any note that is no longer relevant or true.
- Evolve tree structure with `$agent-notepad-map` when branches become crowded or ambiguous.

### Notepad Usage Policy (Mandatory)

- For every non-trivial task, run `$agent-notepad-lookup` before the first file edit.
- For every non-trivial task, run `$agent-notepad-capture` after implementation and before the final response.
- At phase boundary or before commit, run `$agent-notepad-prune` when stale/conflicting notes are possible.
- Run `$agent-notepad-map` when note branches are unclear, crowded, or hard to navigate.

### Non-trivial Task Definition

- A task is non-trivial if at least one condition is true:
- it modifies one or more repository files.
- it investigates runtime/build/test failures.
- it changes architecture, contracts, config, auth, telemetry, infra, or workflows.
- it requires multi-file reasoning or multi-step diagnosis.

### Notepad Completion Gate

- A non-trivial task is not complete unless the final response includes:
- `Notepad: lookup=<done|skipped>, capture=<done|skipped>, prune=<done|skipped>, map=<done|n/a>, notes_updated=<paths>`
- If any required step is skipped, explicitly state the reason.

### Notepad Structure Rules

- Use `.codex/notes/INDEX.md` as the root navigator.
- Treat durable note branches as project-specific and agent-owned; do not assume the same top-level folders across repositories.
- Reserve only `inbox/`, `archive/`, and `_templates/` as system folders.
- Organize notes by the smallest durable branch path that will make future lookup cheaper (for example `architecture/constraints`, `workflow/testing`, `integrations/payments`).
- Keep one narrow topic per leaf file (`kebab-case.md`).
- Keep each branch discoverable with a local `INDEX.md`.
- Prefer splitting large mixed notes into smaller leaf notes.

### Note Entry Rules

- Include minimal traceability in leaf notes:
  - `Status: draft|active|stable|needs-revalidation|superseded`
  - `Confidence: high|medium|low`
  - `Last-Validated: YYYY-MM-DD`
  - `Evidence: <file path | command>`
- Add lifecycle links when relevant:
  - `Related: <note path | none>`
  - `Supersedes: <note path | none>`
  - `Superseded-By: <note path | none>`
- Apply note statuses consistently:
  - `draft` for provisional or weakly evidenced captures, often while routing through `inbox/`
  - `active` for currently trusted notes that guide day-to-day execution
  - `stable` for durable facts or workflow rules validated repeatedly over time
  - `needs-revalidation` for useful notes that may still be true but need a fresh check before reuse
  - `superseded` only when a newer note replaces the same topic
- When evidence changes a prior claim, mark old note `Status: superseded`, link `Superseded-By`, and keep detailed history in `archive/` only when that history is still useful.

## Analyzer Package Changes

- Keep one analyzer per diagnostic and keep shared helpers narrowly focused.
- Favor syntax- and symbol-level checks that are easy to explain over "clever" heuristics with opaque behavior.
- Keep diagnostic IDs stable and document every shipped rule in `README.md` plus `docs/rules/`.
- Prefer option names under the `mithril_analyzers.*` prefix unless a future accepted decision changes the convention.

## Backend Changes

- Use `backend-clean-architecture-guard` whenever a task modifies or reviews backend code in the backend project(s).
- Preserve the current Clean Architecture boundaries. Keep `Domain` pure but not anemic, keep `Application` focused on use-case policy and abstractions, keep `Infrastructure` on adapters and IO, and keep `Api` on transport and composition.
- Do not create oversized backend classes. Split responsibilities before a class starts mixing orchestration, domain policy, IO, validation, mapping, or external-client concerns.
- Apply SOLID pragmatically. Add abstractions only when they reduce coupling or isolate variation; do not over-engineer.
- Before finishing a backend task, if an `.editorconfig` exists, ensure every modified file complies with its directives.

## Frontend Changes

- Use `frontend-fsd-guard` whenever a task modifies or reviews frontend code in the frontend project(s).
- Preserve the current frontend architecture and patterns. Follow existing conventions for state management, side effects, component composition, and styling.
- Do not create oversized frontend components. Split responsibilities before a component starts mixing presentation, state management, side effects, or domain logic.
- Before finishing if the frontend uses a linter or formatter, ensure every modified file complies with its directives.

## Persona System

Personas are defined in `.codex/personas/`.
For each task, explicitly apply one coordinator persona, one primary executor persona, and optional reviewers.
Treat personas as working lenses that contribute distinct task-specific outputs, not as labels or theatrical roleplay.

### Persona Usage Contract

- Selecting a persona requires reading that persona's bio and responsibility files before major decisions or edits.
- State a short lineup before substantial work: coordinator, primary executor, reviewers, and why each is active.
- Each active persona must contribute at least one concrete artifact relevant to the task: scope boundary, tradeoff, implementation constraint, risk, validation plan, or sign-off check.
- Reviewer personas must challenge the plan with role-specific risks or rejection criteria; silent approval does not count as contribution.
- Coordinator resolves disagreement using user intent, code evidence, docs, or validation results, then records the chosen path.
- Re-open persona selection when scope shifts, validation fails, or a new risk domain appears.

### Persona Collaboration Loop

1. Coordinator frames the objective, constraints, unknowns, and success criteria.
2. Primary executor proposes the working approach and required checks.
3. Reviewer personas surface domain-specific risks, edge cases, and acceptance gates.
4. Coordinator chooses the plan, assigns follow-up checks, and closes open tradeoffs.
5. `quality-engineer` confirms evidence before handoff when behavior, workflow, or policy changed.

### Persona File Rules

- Keep persona files generic and repository-agnostic.
- Put project-specific paths, stack details, and local architecture rules in root or nested `AGENTS.md`, not in persona cards.
- Keep persona bios focused on decision lens and anti-patterns; keep responsibility files focused on activation, contributions, and sign-off.

### Coordinator Selection

- Use `systems-analyst` for discovery, ambiguity reduction, and requirement decomposition.
- Use `tech-lead` for cross-layer design or architecture tradeoffs.
- If coordination is uncertain, default to `tech-lead` and ask the user only when product tradeoffs are unresolved.

### Primary Executor Selection

- Product scope, mission alignment, prioritization: `product-lead`
- Product UX, flow design, information architecture, copy, and accessibility direction: `product-designer`
- Frontend architecture, state management, browser behavior, and client performance: `frontend-engineer`
- Backend API/domain implementation: `backend-engineer`
- Data model, migrations, query integrity/performance: `data-warden`
- Security/auth/input hardening: `security-specialist`
- CI/CD, environments, infrastructure, observability, and operational reliability: `platform-engineer`
- Verification, regressions, release confidence: `quality-engineer`

### Reviewer Selection

- Add `security-specialist` for auth, trust boundaries, secrets, or untrusted input.
- Add `data-warden` for persistence/schema/query changes.
- Add `platform-engineer` for CI/CD, release process, environment, observability, or rollback concerns.
- Add `product-lead` for user-facing scope, prioritization, or acceptance tradeoffs.
- Add `product-designer` for interaction-model, copy, accessibility, or flow-clarity risks.
- Add `frontend-engineer` for client-state, browser-runtime, or performance-sensitive UI work.
- Add `quality-engineer` before handoff when behavior changed.

## Skill Usage

- If the user names a skill, apply it in that turn.
- If a task clearly matches a skill description, apply the minimal matching skill set.
- Use `persona-orchestration` whenever a non-trivial task needs explicit coordinator/executor/reviewer contributions.
- Use `skill-creator` when creating or updating skills.
- Keep skill bodies lean and operational; store detailed references in branch notes when needed.
- Validate edited or newly created skills with `quick_validate.py`.

## Project Logs

- `docs/specs/DECISIONS.md` is the canonical entrypoint for the decisions registry. Keep the searchable catalog in `docs/specs/decisions/INDEX.md` and one decision per file in `docs/specs/decisions/entries/`.
- `docs/specs/TECHNICAL_ISSUES.md` is the canonical entrypoint for the technical-issue registry. Keep the searchable catalog in `docs/specs/technical-issues/INDEX.md` and one resolved issue per file in `docs/specs/technical-issues/entries/`.
- Do not turn the front-door files into long rolling logs; agents should route through the index, then open only the relevant entry files.

## Persona Reference

- Catalog and role details: `.codex/personas/INDEX.md`
