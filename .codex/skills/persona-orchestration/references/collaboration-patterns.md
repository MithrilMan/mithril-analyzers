# Persona Collaboration Patterns

Use these patterns when the correct lineup is not obvious from the task alone.

## Discovery And Requirement Shaping

- Coordinator: `systems-analyst`
- Primary executor: `product-lead`
- Reviewers: `tech-lead` when architecture tradeoffs are already visible
- Expected contributions:
  - `systems-analyst` defines assumptions, unknowns, and dependency map
  - `product-lead` defines outcome, scope boundaries, and acceptance priorities
  - `tech-lead` blocks infeasible or inconsistent directions

## Cross-Layer Feature Or Workflow Change

- Coordinator: `tech-lead`
- Primary executor: dominant delivery persona
- Reviewers: `quality-engineer`, plus risk-specific reviewers
- Expected contributions:
  - `tech-lead` resolves ownership and tradeoffs
  - executor plans the implementation path
  - reviewers add acceptance gates for their risk areas

## Backend Or Data-Sensitive Work

- Coordinator: `tech-lead`
- Primary executor: `backend-engineer` or `data-warden`
- Reviewers: `security-specialist`, `quality-engineer`, and `data-warden` if not primary
- Expected contributions:
  - contract and boundary checks
  - persistence or migration safety review
  - abuse-path and permission review
  - regression and validation plan

## Frontend Or UX Work

- Coordinator: `tech-lead` for structural changes, otherwise `systems-analyst` when discovery dominates
- Primary executor: `ui-ux-engineer`
- Reviewers: `product-lead`, `quality-engineer`, and backend or data reviewers only if contracts are affected
- Expected contributions:
  - flow and state coverage
  - user-value and scope sanity check
  - validation coverage for responsive, accessible, and regression-prone paths

## Guardrails

- Prefer the smallest lineup that still covers the real risks.
- If two personas produce overlapping contributions, drop the weaker one or redefine its task.
- Re-open the lineup when implementation enters a new trust boundary, persistence concern, or release path.
