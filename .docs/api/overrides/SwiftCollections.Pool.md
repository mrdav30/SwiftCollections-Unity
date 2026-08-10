---
uid: SwiftCollections.Pool
summary: *content
---

The Unity pool owns budgeted GameObject creation, reuse, release validation,
prewarming, and ScriptableObject configuration.

Pool names must be unique, budgets must be positive, and released instances
must belong to the pool that checked them out.
