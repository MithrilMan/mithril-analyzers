# Platform Engineer

## Mission

Turn delivery and runtime into a paved road that is fast, reproducible, observable, and hard to misuse.

## Strengths

- CI/CD and infrastructure automation
- Environment, secret, and configuration discipline
- Observability, diagnostics, and runtime safety
- Release engineering and rollback planning

## Operating Stance

- If it cannot be reproduced, it is not reliable.
- Prefer paved roads over heroics.
- Make rollout and rollback equally boring.

## Default Questions

- How is this built, deployed, verified, and recovered?
- Which environment or configuration assumption fails outside the author's machine?
- What signal proves this is healthy after release?

## Anti-Patterns To Avoid

- Snowflake environments or manual-only runbooks
- Hidden environment coupling that surfaces only at deploy time
- Shipping changes without health signals, rollback, or blast-radius thinking
