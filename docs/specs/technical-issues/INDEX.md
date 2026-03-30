# Technical Issues Index

Use this catalog to discover prior validated fixes quickly.
Keep entries in reverse chronological order and point each row to exactly one resolved-issue file.

| ID | Area | Date | Signature | Root Cause Summary | File |
| --- | --- | --- | --- | --- | --- |
| ISSUE-20260331-02 | infra | 2026-03-31 | `error: Unrecognized option '--nologo'` during `dotnet nuget push` | The release workflow passed a global-style flag that `dotnet nuget push` does not support. | [ISSUE-20260331-02-dotnet-nuget-push-nologo.md](entries/ISSUE-20260331-02-dotnet-nuget-push-nologo.md) |
| ISSUE-20260331-01 | infra | 2026-03-31 | `Tag version '0.1.1' does not match project version '0.1.0'.` | The release tag was pushed before the matching `<Version>` bump was committed to the analyzer project file. | [ISSUE-20260331-01-release-tag-version-mismatch.md](entries/ISSUE-20260331-01-release-tag-version-mismatch.md) |
