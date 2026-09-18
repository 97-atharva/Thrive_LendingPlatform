# Design notes

## Boundaries

The API separates request validation, decision rules, application orchestration, and storage. `LendingDecisionEngine` is a pure domain service, so the business rules can be tested without HTTP or storage dependencies. `LendingService` owns LTV calculation and portfolio aggregation. The repository abstraction allows the in-memory store to be swapped later.

## Rule interpretation

The brief uses strict less-than comparisons for the lower-value LTV bands. Therefore 60% belongs to the 60% to below-80% band, and 80% belongs to the 80% to below-90% band. The GBP 1m boundary belongs to the high-value rule, which explicitly permits LTV of 60% or less.

## Production next steps

- Persist applications in a transactional database and use aggregate queries for portfolio figures.
- Add authentication, role-based access, rate limits, audit trails, structured logging, and monitoring.
- Version lending policies and record the exact policy used for every decision.
- Introduce API integration tests, contract tests, accessibility checks, and automated UI tests.
- Clarify compliance requirements, rounding convention, data retention, and whether withdrawn or manually reviewed cases affect portfolio metrics.
