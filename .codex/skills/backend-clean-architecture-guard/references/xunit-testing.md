# xUnit Backend Testing

## Goals

- Keep tests readable, behavior-focused, and resilient to internal refactors.
- Use tests to protect invariants and workflow seams, not to mirror implementation trivia.

## Best practices

- Name tests around behavior and expected outcome.
- Keep setup local to the test file until repetition clearly justifies a fixture or helper.
- Prefer fakes, stubs, or small builders that express the scenario cleanly over giant shared test harnesses.
- Assert the observable contract: state changes, returned results, persisted records, or emitted events.
- When a class is decomposed into collaborators, adjust tests so they validate the seam and contract rather than forcing the old object shape.
- Keep test data explicit enough that future readers can understand why the scenario matters.

## Heuristics

- If a test breaks because of harmless internal refactors, it may be asserting implementation details instead of behavior.
- If many tests need the same huge setup object, the production boundary may still be too broad or the test helper needs a smaller builder.
