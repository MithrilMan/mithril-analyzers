# BGA002 Method Too Long

## What It Matches

- methods with block bodies
- constructors with block bodies
- local functions with block bodies

## What It Ignores

- expression-bodied members
- generated code

## Configuration

```ini
[src/**/*.cs]
mithril_analyzers.max_method_lines = 100
```

## Notes

- The diagnostic is reported on the method or local-function identifier.
- V1 intentionally avoids lambda analysis to keep precision high.
