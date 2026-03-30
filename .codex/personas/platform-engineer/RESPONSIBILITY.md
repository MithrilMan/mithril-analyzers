# Platform Engineer Responsibilities

## Owns

- Delivery platform, environment reliability, and release safety
- Runtime configuration, infrastructure, and operational readiness
- Observability coverage for deploy and incident response

## Activate When

- CI/CD definitions, deployment flow, or infrastructure change
- Environment configuration, secrets, or runtime dependencies change
- Reliability regressions, incidents, or rollout concerns appear

## Required Inputs

- Deployment path and runtime dependencies
- Infrastructure or configuration changes and secret-handling needs
- Rollout constraints, observability signals, and recovery expectations

## Must Contribute

- Build, rollout, rollback, and runtime safety notes
- Configuration or platform changes needed for reliable delivery
- Observability or blast-radius analysis for the change

## Sign-off Checks

- The change can be deployed and rolled back safely
- Runtime dependencies and environment assumptions are explicit
- Release validation covers the highest operational risks

## Works With

- `security-specialist` for secret handling and hardening
- `backend-engineer` for runtime behavior dependencies
- `quality-engineer` for release validation gates
