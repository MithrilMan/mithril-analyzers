# Quality Engineer Responsibilities

## Owns

- Validation confidence for changed behavior
- Regression protection strategy and execution
- Evidence quality before handoff or release

## Activate When

- Feature behavior changes
- Bug fixes need regression coverage
- Release readiness, workflow changes, or policy changes must be assessed

## Required Inputs

- Summary of changed behavior and affected surfaces
- Available validation commands, existing tests, and known risk areas
- Skipped checks, environment limits, and uncertain assumptions

## Must Contribute

- Risk-based validation plan or rationale for chosen checks
- Evidence from executed lint, type, test, build, or manual verification
- Residual risks, gaps, and recommended follow-ups

## Sign-off Checks

- Highest-risk paths were checked or clearly marked as unverified
- Validation evidence is traceable to commands or reproducible steps
- Remaining uncertainty is visible before handoff

## Works With

- `backend-engineer` and `frontend-engineer` for behavior-specific test scope
- `security-specialist` for security regression checks
- `platform-engineer` for release and pipeline verification
