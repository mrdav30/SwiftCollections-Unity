# SwiftCollections for Unity Migration Guide

## Migrating from v7 to v8

All four packages now require Unity **6.3 LTS (6000.3) or newer**. Upgrade the
editor before adopting v8; projects on older editors should keep v7 packages.
CI validates Unity 6000.3.9f1.

Update your Standard or Lean base and its optional FixedMathSharp companion
together to v8.0.0. Companions require the matching FixedMathSharp-Unity v8.0.0
base; their editor installer uses that revision. Do not mix Standard and Lean
variants. Rebuild dependent assemblies after updating.

Both bases bundle the released .NET Standard 2.1 SwiftCollections v8.0.0
assemblies and Chronicler v1.0.0 dependencies, including `Chronicler.Diagnostics`.
Lean includes the attribute-only `Chronicler.MemoryPackShim` and omits the
MemoryPack runtime. No separate Chronicler Unity package is required.

The Unity authoring adapters, bounds extensions, and GameObject pool APIs are
unchanged. For scripts using the core collections, update these renamed APIs,
their state types, and nested enumerator types:

| v7 | v8 |
| --- | --- |
| `SwiftBucket<T>` | `SwiftSlotMap<T>` |
| `SwiftGenerationalBucket<T>` | `SwiftTicketMap<T>` |
| `SwiftHandle` | `SwiftTicket` |

Ticket-map named arguments now use `ticket` instead of `handle`. Shared guards
and diagnostic types move to `Chronicler.Diagnostics`; `SwiftThrowHelper`
becomes `ChronicleGuard`. State JSON converters use `Chronicler.Serialization`.
If your asmdef uses `overrideReferences`, add `Chronicler.Diagnostics.dll` to
its precompiled references when using those APIs.

The renamed core collection state layouts are preserved, but persisted CLR
type names and serializer registrations require explicit migration. Re-run
your serialization and simulation tests, especially when using fixed-point
queries whose corrected core results may change simulation outcomes.

See the [SwiftCollections v8 migration guide](https://github.com/mrdav30/SwiftCollections/blob/v8.0.0/docs/MIGRATION.md)
for the complete API changes and the [FixedMathSharp Unity migration guide](https://github.com/mrdav30/FixedMathSharp-Unity/blob/v8.0.0/MIGRATION.md)
for math package upgrades.
