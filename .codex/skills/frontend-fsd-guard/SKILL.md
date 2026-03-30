---
name: frontend-fsd-guard
description: Enforce Feature-Sliced Design 2.1 during frontend work. Use when editing or reviewing a frontend codebase, especially for refactors, new slices, import-boundary cleanup, public API design, or architecture-sensitive UI changes.
---

# Frontend Fsd Guard

## Overview

Apply this workflow before making structural frontend changes. Use it to keep code inside the canonical FSD layers, expose stable public APIs, and prevent accidental cross-layer or deep-import drift.

FSD shorthand:

- `app` wires runtime concerns.
- `pages` own route composition roots.
- `widgets` own large reusable page blocks and cross-feature page regions.
- `features` own user-facing actions and interactions.
- `entities` own domain concepts and reusable domain helpers.
- `shared` owns framework-agnostic UI, utilities, and API plumbing.

If a file feels too large, split it by ownership first: route composition in `pages`, reusable page regions in `widgets`, interactions in `features`, and domain helpers in `entities`.

Full FSD rules live in `references/fsd-2.1-rules.md`.

Keep this skill focused on workflow and architecture checks. Stack-specific best practices live in the `references/` files and should be loaded only when the target frontend actually uses that stack or when the current change touches it directly.

## Workflow

1. Identify the frontend scope first:
   - determine which frontend root is being changed if there are multiple (for example, `frontend/public/` vs `frontend/admin/`)
   - if the change touches multiple frontends, run the workflow for each one and keep their constraints in view separately; do not merge them into one generic FSD workflow
   - confirm whether the change belongs to a route composition root, a reusable page region, a widget, a feature interaction, or an entity helper before moving files

2. Read the local constraints before editing:
   - read the nearest nested `AGENTS.md` for the target frontend tree, if one exists
   - treat nested `AGENTS.md` files as the repository-specific policy layer; keep this skill focused on reusable FSD workflow
   - inspect the local `## Stack` section and any `## Technical References` section
   - treat that AGENTS metadata as the authoritative map for which technical references from `references/` apply to the current frontend

3. Load only the references that match the stack or the touched surface:
   - always read `references/fsd-2.1-rules.md`
   - if the nearest `AGENTS.md` exposes `## Technical References`, prefer that list over guessing from package names
   - read `references/react-19-composition.md` when the target uses React 19 or the change touches hooks/effects/render composition
   - read `references/react-router.md` when routes, params, navigation, or route-owned composition change
   - read `references/tanstack-query.md` when server state, mutations, cache patching, or optimistic flows change
   - read `references/zustand-immer.md` when local shared UI state, selectors, or client-side stores change
   - read `references/zod-contracts.md` when runtime validation or API contract shaping changes
   - read `references/tailwindcss-v4.md` when the target uses Tailwind CSS v4 or the change touches utility styling, theme tokens, arbitrary values, or class-detection behavior
   - read `references/shadcn-ui.md` when the target uses shadcn/ui, local shadcn-style primitives, or Radix-composed component ownership/composition patterns
   - read `references/css-modules.md` when component styling or visual composition changes
   - read `references/xyflow-react.md` when `@xyflow/react` owns part of the interaction
   - do not bulk-read every reference by default; keep context proportional to the change

4. Map the change to the correct layer before moving code:
   - `app`: providers, entrypoints, global runtime wiring
   - `pages`: route/screen composition roots and route-local orchestration
   - `widgets`: large reusable page blocks, shells, side panels, dashboards, and other self-sufficient regions that combine multiple features or entities
   - `features`: user-facing interactions or actions such as forms, dialogs, editors, and one-purpose workflows
   - `entities`: domain concepts and their UI/model/api, plus reusable domain-facing presenters
   - `shared`: framework, UI kit, cross-domain utilities, API plumbing, and business-agnostic helpers
   - avoid `processes`; the official spec marks it deprecated

   When a route grows too large, keep the page thin and move reusable regions into `widgets` before pushing lower-level domain logic into `shared`.
   If one TSX file starts owning multiple panels, dialogs, filters, or editing surfaces, split it before it becomes a 400-plus-line god file. Prefer page-local `ui/`, `model/`, and `lib/` first, then promote truly reusable regions into `widgets`, `features`, or `entities`.

5. Enforce import boundaries while editing:
   - import only from lower layers
   - do not let `shared` import from `entities`, `features`, `widgets`, `pages`, or `app`
   - do not let `widgets` import from `pages` or `app`
   - let `widgets` import from `features`, `entities`, and `shared`
   - let `pages` import from `widgets`, `features`, `entities`, and `shared`
   - keep same-layer cross-slice imports rare and only through explicit public APIs
   - avoid deep imports into another slice internals

6. Keep public APIs deliberate:
   - add or update `index.ts` exports for slices/segments that are used outside
   - do not use wildcard exports by default
   - if a file inside a slice needs another file from the same slice, import it directly, not through that slice's barrel

7. Place code by purpose, not by essence:
   - prefer `ui`, `api`, `model`, `lib`, `config`
   - avoid vague segments like `components`, `hooks`, or `types` unless they are already an established local convention
   - use `ui` for composition and rendering, `model` for state and rules, and `lib` for narrow helpers that support one slice

8. Choose the state boundary before growing a store:
   - prefer component-local React state when the value is not shared outside one subtree
   - when UI-only state must be shared across one bounded frontend area, prefer one Zustand store for that bounded context composed from thematic slices
   - keep invariants that must change together inside the same slice/store instead of scattering them across many peer stores
   - avoid both god stores and multiple global stores that really belong to one workflow

9. Validate with the repo guardrails after structural changes:
   - run the frontend-specific FSD lint or equivalent boundary check for the target codebase
   - run the relevant typecheck, lint, test, or build commands for the changed frontend
   - if the target frontend exposes the bundled file-size wrapper, run it or rely on `pnpm run lint` when that wrapper is already part of the lint pipeline
   - if the loaded Tailwind reference applies and utility-heavy styling changed, run the bundled canonicalization workflow or the target frontend's equivalent wrapper

## Review Checklist

- Does each changed file belong to the layer/slice that owns its responsibility?
- Are cross-slice imports going through public APIs instead of internal paths?
- Did `shared` remain business-agnostic?
- Are `pages` composing slices instead of accumulating domain logic?
- Did route-local composition stay in `pages` while reusable multi-surface regions moved to `widgets`?
- Did `widgets` stay focused on reusable page regions instead of absorbing route-only logic?
- Did new reusable UI blocks stop at `widgets` or `entities/ui` instead of leaking into `shared/ui`?
- Were only the relevant stack references loaded instead of dumping unrelated guidance into the task?
- If Zustand state changed, does the store boundary still match one bounded context and are the slices thematic rather than accidental?
- If the change adds generated/API files, are they kept in the owning frontend and not in generic tooling folders?
- If Tailwind utility styling changed, did the bundled canonicalization workflow run or did the target frontend use a local wrapper around it?

## References

- Repository-specific frontend policy: nearest nested `AGENTS.md` under the target frontend tree, if present
- FSD architecture rules: `references/fsd-2.1-rules.md`
- React 19 composition and effects: `references/react-19-composition.md`
- React Router ownership and URL state: `references/react-router.md`
- TanStack Query server-state rules: `references/tanstack-query.md`
- Zustand + Immer local-state rules: `references/zustand-immer.md`
- Zod runtime-contract rules: `references/zod-contracts.md`
- Tailwind CSS v4 utility and token rules: `references/tailwindcss-v4.md`
- shadcn/ui and shadcn-style primitive composition: `references/shadcn-ui.md`
- Frontend file-size guard script: `scripts/tsx-file-size-guard.mjs`
- Tailwind canonicalizer script: `scripts/tailwind-canonical-classes.mjs`
- CSS Modules styling rules: `references/css-modules.md`
- React Flow / XYFlow interaction rules: `references/xyflow-react.md`
