# BGA003 Accessor Too Long

## What It Matches

- `get`, `set`, and `init` accessors with block bodies that exceed `mithril_analyzers.max_accessor_lines`

## What It Ignores

- expression-bodied accessors
- generated code

## Configuration

```ini
[*.cs]
mithril_analyzers.max_accessor_lines = 60
```

## Notes

- The rule is meant to surface hidden workflow logic inside properties.
