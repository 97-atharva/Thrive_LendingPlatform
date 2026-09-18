# AI work log

This is a concise record of the AI-assisted process requested in the assessment. The implementation and final choices were reviewed against the supplied brief.

## Prompt 1: requirement extraction

> Analyse the supplied candidate test and identify the required inputs, outputs, business rules, technical preferences, and submission artefacts.

Outcome: identified a C# API and React UI as the preferred stack; inputs were loan amount, secured asset value, and credit score; outputs were decision plus portfolio metrics. I also captured the explicit high-value and lower-value LTV bands.

## Prompt 2: domain design

> Propose a small, testable architecture for lending decisions and in-memory portfolio metrics. Keep business rules independent of HTTP and storage.

Outcome: used a pure decision engine, a service for LTV calculation and aggregation, and an in-memory repository behind an interface. This keeps the assessment compact while making rules easy to test.

## Prompt 3: edge-case review

> Review the rules for boundary values at GBP 100k, GBP 1m, GBP 1.5m, and 60%, 80%, and 90% LTV. State the unambiguous interpretation and test cases.

Correction adopted: the first draft could have treated 60% as the under-60% band. On review, the wording says `LTV < 60%`, so exactly 60% instead receives the 800 credit-score threshold for loans below GBP 1m. At GBP 1m exactly, the high-value rule takes priority and allows 60% LTV with a score of at least 950. The unit tests cover these boundaries.

## Prompt 4: UI review

> Build a focused React form that shows a decision, LTV, human-readable reason, and the required portfolio figures. Handle validation and an unavailable API clearly.

Outcome: the UI posts to the API, refreshes portfolio metrics after a valid submission, uses accessible alert/live regions, and avoids presenting a failed request as a lending decision.

## Human review and trade-offs

- AI suggestions were limited to this assessment's scope; no claims of production-grade lending compliance are made.
- I chose to include declined applications in mean LTV because the brief says across all applications, while the written-loan total includes successful loans only.
- Data is intentionally in-memory for demonstrability. Persistence and controls are documented as future work.
