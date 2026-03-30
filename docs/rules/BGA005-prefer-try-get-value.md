# BGA005 Prefer `TryGetValue`

## What It Matches

- simple `if (dictionary.ContainsKey(key))` checks followed by `dictionary[key]` access in the true branch

## What It Ignores

- compound conditions
- `if` statements with `else`
- nested-function patterns that make the data flow ambiguous
- generated code

## Notes

- V1 is intentionally conservative and may miss edge cases rather than flagging uncertain patterns.
