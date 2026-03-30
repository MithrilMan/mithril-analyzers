# Tailwind CSS v4

Official sources consulted:

- https://tailwindcss.com/docs/theme
- https://tailwindcss.com/docs/functions-and-directives
- https://tailwindcss.com/docs/detecting-classes-in-source-files
- https://tailwindcss.com/docs/upgrade-guide

Bundled companion script:

- `scripts/tailwind-canonical-classes.mjs` for bulk cleanup of canonical Tailwind v4 classes in a target frontend workspace

## Goals

- Keep Tailwind styling explicit, canonical, and easy for the scanner to detect.
- Promote repeated design values into theme tokens instead of repeating arbitrary values in JSX.
- Use arbitrary syntax deliberately for one-off cases, not as the default design-token path.

## Best practices

- Prefer canonical utility classes over equivalent arbitrary forms when both exist.
- Use `@theme` for durable project tokens such as colors, shadows, radii, spacing aliases, fonts, and breakpoints.
- When the project already uses CSS custom properties as the source of truth, bridge them into Tailwind tokens through the local theme pattern instead of repeating `var(...)` in markup.
- Prefer semantic tokens like `text-muted`, `bg-brand-soft`, or `border-stroke-subtle` over repeated `text-[var(--text-muted)]`-style utilities.
- Use arbitrary values for genuinely one-off cases such as `calc(...)`, precise layout constraints, or unsupported values that should not become reusable tokens.
- In Tailwind v4, use the CSS-variable shorthand with parentheses, like `bg-(--brand-color)`, not the old bracket form from earlier versions.
- Use `@utility` for shared custom utilities that should participate in variants instead of hiding the pattern in long copied class strings.
- When a project ships a local canonicalization script, prefer that workflow over manual one-by-one cleanup in the editor.
- Keep class names statically discoverable. Do not build utility names with string concatenation or interpolation that hides the final class from Tailwind's scanner.
- When conditionally styling, choose between complete class names that already exist in source, not partial fragments like `"text-" + tone + "-600"`.
- If a repeated arbitrary value starts appearing across multiple files, stop and decide whether it should become a token in `@theme` or a shared utility.

## Heuristics

- If the same arbitrary `var(...)` value appears more than once or twice, it probably wants a theme token.
- If a class string is hard to read because it mixes reusable semantics with raw values, extract the raw value into the theme first.
- If the editor reports `suggestCanonicalClasses`, treat that as a cleanup signal, not cosmetic noise.
- Do not assume Tailwind v4's public `canonicalizeCandidates()` API catches every IntelliSense `suggestCanonicalClasses` warning; some editor suggestions need an extra theme-aware pass.
- If a style is layout-specific and unlikely to repeat, an arbitrary value is fine.

## Canonicalization Workflow

- When the target frontend uses Tailwind CSS v4 and you need bulk cleanup of `suggestCanonicalClasses` warnings, run the bundled script from the frontend workspace root:
  - `node <repo>/.codex/skills/frontend-fsd-guard/scripts/tailwind-canonical-classes.mjs --check --css src/index.css --src src`
  - add `--write` to apply the edits in place
- The script first uses Tailwind v4's official `canonicalizeCandidates()` API, then applies a second theme-aware pass for exact scale matches that IntelliSense flags but the public API does not normalize by itself.
- The current implementation already covers exact canonical rewrites like:
  - `rounded-[32px] -> rounded-4xl`
  - `min-h-[22rem] -> min-h-88`
  - `min-w-[18rem] -> min-w-72`
  - `max-w-[1800px] -> max-w-450`
- The script only rewrites string literals used as Tailwind class sources, specifically JSX `className` / `class` attributes and common class-composition helpers such as `cn`, `clsx`, `twMerge`, `classNames`, and `cva`.
- If the workspace already exposes project-local npm scripts that wrap this tool, prefer those aliases for day-to-day use and keep the skill script as the shared source of truth.
