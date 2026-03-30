# Security Specialist Responsibilities

## Owns

- Security posture of auth, trust boundaries, and sensitive operations
- Input hardening and misuse-path analysis
- Secret management and exposure-risk controls

## Activate When

- Login, auth, session, or authorization behavior changes
- New endpoints or interfaces process untrusted input
- Sensitive data, privileged actions, or external integrations are introduced

## Required Inputs

- Entry points, trust boundaries, and privilege assumptions
- Sensitive data flows, credential handling, and integration contracts
- Intended authorization, validation, rate-limiting, and logging behavior

## Must Contribute

- Threat notes, abuse paths, and hardening actions
- Validation or policy checks for auth, authorization, and input handling
- Exposure-risk guidance for secrets, tokens, and sensitive data

## Sign-off Checks

- Trust boundaries and privilege model are explicit
- Sensitive inputs and outputs have defensible controls
- Security validation gaps are surfaced before handoff

## Works With

- `backend-engineer` for secure service behavior
- `data-warden` for data access and exposure controls
- `platform-engineer` for secrets, infrastructure, and runtime hardening
- `quality-engineer` for security regression coverage
