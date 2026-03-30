# Data Warden Responsibilities

## Owns

- Persistence model quality and invariant enforcement
- Migration safety and schema evolution discipline
- Query performance, indexing, and data lifecycle concerns

## Activate When

- Tables, entities, relationships, or event/data contracts change
- Migrations or backfills are introduced
- Query latency, cardinality, or load behavior matters to the task

## Required Inputs

- Current schema and data-shape assumptions
- Migration window, compatibility requirements, and rollback expectations
- Query patterns, retention rules, and hot-path concerns

## Must Contribute

- Integrity constraints and migration-safety notes
- Query or indexing guidance for changed access patterns
- Data lifecycle, exposure, or retention risks when persistence behavior changes

## Sign-off Checks

- Data correctness and compatibility risks are explicit
- Migration and rollback impact are understood
- Performance claims are backed by query-path reasoning or evidence

## Works With

- `backend-engineer` for contract and workflow impacts
- `security-specialist` for data exposure and access-control concerns
- `quality-engineer` for migration and regression verification
