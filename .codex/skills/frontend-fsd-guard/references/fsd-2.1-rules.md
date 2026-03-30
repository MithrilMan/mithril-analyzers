# FSD 2.1 Rules

Official sources consulted:

- Overview: https://fsd.how/docs/get-started/overview/
- Layers: https://fsd.how/docs/reference/layers/
- Slices and segments: https://fsd.how/docs/reference/slices-segments/
- Public API: https://fsd.how/docs/reference/public-api/
- Migration from v2.0 to v2.1: https://fsd.how/docs/guides/migration/from-v2-0/
- Cross-imports: https://fsd.how/docs/guides/issues/cross-imports/

## What FSD 2.1 is optimizing for

- FSD is for frontend applications, not libraries.
- The model is incremental: a project can start small and extract lower layers only when reuse or domain boundaries justify it.
- Monorepos are supported. Each package can act as its own FSD root with its own layers.
- The v2.1 mental model is page-first: start with `app`, `pages`, and `shared`; move code down to `widgets`, `features`, or `entities` only after reuse or domain ownership is proven.
- `processes` still exists historically in diagrams, but the current spec marks it as deprecated and recommends moving its concerns to `app` and `features`.

## Canonical hierarchy

FSD has a fixed three-level hierarchy:

1. `layer`
2. `slice`
3. `segment`

Rules:

- Layer names are standardized and should stay lowercase.
- Adding custom layers is discouraged because layer semantics are part of the standard.
- `app` and `shared` do not have slices; they are split directly into segments.
- `pages`, `widgets`, `features`, and `entities` follow `layer/slice/segment`.

Canonical layers from top to bottom:

1. `app`
2. `processes` (deprecated)
3. `pages`
4. `widgets`
5. `features`
6. `entities`
7. `shared`

Typical minimum viable FSD project:

- `src/app`
- `src/pages`
- `src/shared`

## Typical project structure

This is a representative FSD 2.1 tree, not a requirement to use every layer:

```text
src/
  app/
    entrypoint/
    providers/
    routes/
    store/
    styles/
    analytics/
  pages/
    home/
      ui/
      api/
    auth/
      ui/
      api/
  widgets/
    page-layout/
      ui/
    feed/
      ui/
      model/
  features/
    auth-by-email/
      ui/
      api/
      model/
      config/
    add-to-cart/
      ui/
      api/
      model/
  entities/
    user/
      ui/
      api/
      model/
    product/
      ui/
      api/
      model/
      @x/
        cart.ts
  shared/
    api/
    config/
    i18n/
    lib/
      dates/
        README.md
      text/
        README.md
    routes/
    ui/
      button/
        index.ts
      text-field/
        index.ts
```

Structure notes:

- Omit unused layers instead of scaffolding them preemptively.
- Keep folder names lowercase.
- Do not introduce `processes/` in new structures.
- Prefer extracting code downwards only when it is reused or it clarifies a real domain boundary.
- In `shared/lib`, each internal library should have one clear area of focus. Avoid a generic helpers dump.

## Layer-by-layer structure guidance

| Layer | What belongs there | Typical segments or contents | Structural notes |
| --- | --- | --- | --- |
| `app` | Global runtime wiring and app-wide concerns | `entrypoint`, `routes`, `store`, `styles`, `providers`, `analytics` | No slices. Segments can import each other freely. |
| `pages` | Route or screen composition roots | `ui`, `api`, loading states, error boundaries | One page usually maps to one slice. Similar pages can share one slice. Keep page-local code here if it is not reused. |
| `widgets` | Large, self-sufficient UI blocks | Usually `ui`, sometimes `model`, `api` | Use for reusable large blocks, page layouts, or major independent page regions. |
| `features` | User-visible interactions that deliver business value | `ui`, `api`, `model`, `config` | Not everything deserves a feature. If something is used on only one page, keep it in that page first. |
| `entities` | Business concepts the product works with | `ui`, `api`, `model`, sometimes `@x` | Store entity types, validation, storage, API mapping, and appearance reuse. Put entity-to-entity interaction logic higher when possible. |
| `shared` | Reusable foundation and external-world integration | `api`, `ui`, `lib`, `config`, `routes`, `i18n` | No slices. Segments can import each other freely. `shared/ui` may be business-themed, but should not contain business logic. |
| `processes` | Former multi-page escape hatch | N/A for new work | Deprecated. Prefer `app` and `features`. |

## Slices

Slices are domain-oriented folders inside `pages`, `widgets`, `features`, and `entities`.

Rules:

- Slice names are not standardized; they come from the business or application domain.
- A good slice is highly cohesive and minimally coupled with other slices on the same layer.
- Slices on the same layer should not import each other directly as a default pattern.
- If several slices are closely related, they may be grouped under a structural folder, but that folder must not become a place for shared code.

Allowed grouping example:

```text
features/
  post/
    like/
    compose/
    delete/
```

Not allowed inside the grouping folder:

- `features/post/shared.ts`
- `features/post/lib/`
- any hidden code-sharing area outside the actual slices

## Segments

Segments organize code by technical purpose, not by artifact type.

Preferred standardized segment names:

- `ui` for presentation, visual composition, styles, and UI-only behavior
- `api` for requests, DTOs, mappers, and backend integration code
- `model` for domain state, schemas, stores, and business logic
- `lib` for focused internal libraries needed by the slice
- `config` for flags and configuration

Guidelines:

- Custom segments are most common in `app` and `shared`, where slice decomposition does not apply.
- Segment names should describe purpose, not essence.
- Avoid primary segment names like `components`, `hooks`, or `types`; they make navigation worse because they describe file shape, not responsibility.

## Import rule on layers

Core rule:

> A module inside a slice can only import slices from layers strictly below.

Practical effect by layer:

- `app` can import from `pages`, `widgets`, `features`, `entities`, and `shared`.
- `pages/<slice>` can import from `widgets`, `features`, `entities`, and `shared`.
- `widgets/<slice>` can import from `features`, `entities`, and `shared`.
- `features/<slice>` can import from `entities` and `shared`.
- `entities/<slice>` can import from `shared`.
- `shared` can only import from `shared`.

Exceptions:

- `app` and `shared` do not have slices, so their segments can import each other freely inside the same layer.
- Files inside the same slice can import each other directly.

## Public API rules

Every slice, and every segment inside `app` or `shared`, should expose an explicit public API.

Rules:

- External consumers must import through that public API, not through internal file paths.
- Prefer explicit named exports in `index.ts` over `export *`.
- The public API should protect consumers from internal refactors, expose only what is needed, and make behavioral breaking changes visible.
- Inside the same slice, do not import through that slice's own barrel. Use relative paths to the concrete file instead.
- Between different slices, use absolute imports to the public API, usually through an alias.

Example:

```text
features/comments/index.ts
features/comments/ui/CommentList.tsx
features/comments/model/useComments.ts
```

Recommended:

- `pages/post/ui/PostPage.tsx -> import { CommentList } from "@/features/comments";`
- `features/comments/ui/CommentList.tsx -> import { useComments } from "../model/useComments";`

Avoid:

- `features/comments/ui/CommentList.tsx -> import { useComments } from "../";`

That pattern can create circular imports when the barrel also re-exports `CommentList`.

## Public API caveats for `shared`

The official docs call out two common barrel-file problems in `shared`:

- Large bundles or broken tree-shaking when one big `shared/ui/index.ts` or `shared/lib/index.ts` re-exports unrelated modules.
- Slower dev-server and bundler performance in large projects with too many index files.

Preferred structure:

```text
shared/
  ui/
    button/
      index.ts
    text-field/
      index.ts
  lib/
    dates/
      index.ts
    text/
      index.ts
```

Preferred imports:

- `@/shared/ui/button`
- `@/shared/ui/text-field`
- `@/shared/lib/dates`

Avoid:

- one mega barrel for all of `shared/ui`
- one mega barrel for all of `shared/lib`
- extra `index.ts` files inside segments of sliced layers when the slice barrel is already enough

## Cross-imports and `@x`

Cross-imports are imports between different slices on the same layer. The official guidance treats them as a code smell because they weaken ownership, isolation, testability, and refactor safety.

Preferred handling order:

1. Merge slices if they always change together.
2. Move shared domain logic down into an `entities` slice when the reuse is truly domain-level.
3. Compose slices from a higher layer (`pages` or `app`) with inversion-of-control patterns.
4. Only if unavoidable, allow reuse through an explicit public API.

`@x` notation:

- Use `@x` only for explicit entity-to-entity cross-references.
- Treat it as a last resort, not as a general reuse tool.
- Keep it limited to the `entities` layer.

Example:

```text
entities/
  song/
    @x/
      artist.ts
    index.ts
  artist/
    model/
      artist.ts
```

Then code in `entities/artist` may import from `entities/song/@x/artist`.

## FSD 2.1 decomposition heuristics

Use these heuristics when deciding how much structure a project really needs:

- Start by making `app` and `shared` clean and navigable.
- Put most route-specific UI and logic into `pages` first.
- Extract `widgets` when a page has large independent blocks or when a block is reused.
- Extract `features` when a user-facing interaction is reused across pages or is large enough to deserve its own navigation point.
- Extract `entities` for stable business concepts, shared domain UI, validation, and domain logic.
- If a slice is used once, reconsider whether it should stay in `pages`.
- If a layer accumulates too many tiny slices, grouping or merging is usually better than further fragmentation.

## Official tooling worth using

- The official ecosystem includes the `Steiger` architectural linter.
- `npx steiger src` is explicitly recommended by the migration guide when moving to the v2.1 page-first mental model.
- The most relevant checks for over-structured projects are `insignificant-slice` and `excessive-slicing`.

## Review checklist for describing an FSD structure

- Does the described tree use the canonical layers and omit `processes` for new work?
- Is the minimum shape `app/pages/shared`, with lower layers added only when justified?
- Do `app` and `shared` avoid slices and use segments directly?
- Are `pages`, `widgets`, `features`, and `entities` described as `layer/slice/segment`?
- Are slice names domain-driven rather than artifact-driven?
- Are segment names purpose-driven rather than essence-driven?
- Is the import rule clear and consistent with layer direction?
- Is public API usage explicit, including the barrel-file caveats for `shared/ui` and `shared/lib`?
- Are same-layer cross-imports treated as a smell rather than a default?
- Is `@x` limited to explicit `entities` cross-references?
