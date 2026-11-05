---
title: SwiftCollections for Unity
description: Unity authoring adapters, GameObject pooling, bounds conversion, package guidance, and API reference for SwiftCollections.
---

<div class="sc-hero">
  <p class="sc-kicker">SWIFTCOLLECTIONS AT THE UNITY BOUNDARY</p>
  <h1>Author in Unity. Keep hot paths focused.</h1>
  <p>Bring SwiftCollections into Unity with serializable collection adapters,
  explicit bounds conversion, GameObject pooling, samples, and matching
  FixedMathSharp query companions.</p>
  <div class="sc-actions">
    <a href="guides/getting-started.md">Get started</a>
    <a href="xref:SwiftCollections">Browse the Unity API</a>
  </div>
</div>

## Choose one base

<div class="sc-card-grid">
  <div class="sc-card">
    <h3><a href="https://github.com/mrdav30/SwiftCollections-Unity/tree/v8.0.0/com.mrdav30.swiftcollections">Standard</a></h3>
    <p>The core runtime, Unity authoring adapters, pooling, bounds conversion,
    samples, and MemoryPack support.</p>
  </div>
  <div class="sc-card">
    <h3><a href="https://github.com/mrdav30/SwiftCollections-Unity/tree/v8.0.0/com.mrdav30.swiftcollections.lean">Lean</a></h3>
    <p>The same collection and Unity adapter APIs without the MemoryPack runtime
    dependency.</p>
  </div>
  <div class="sc-card">
    <h3><a href="https://mrdav30.github.io/SwiftCollections/">Core library</a></h3>
    <p>Explore the engine-agnostic collections, handles, serialization state,
    diagnostics, and spatial query structures underneath the adapters.</p>
  </div>
</div>

## Add Unity where Unity owns the work

<div class="sc-card-grid">
  <div class="sc-card">
    <h3><a href="xref:SwiftCollections.Unity">Serializable authoring</a></h3>
    <p>Persist supported collection shapes in scenes, prefabs, and assets, then
    consume their live SwiftCollections representation through
    <code>Runtime</code>.</p>
  </div>
  <div class="sc-card">
    <h3><a href="xref:SwiftCollections.Pool">GameObject pooling</a></h3>
    <p>Configure budgeted pools as a ScriptableObject, prewarm deliberately, and
    rent or release instances through explicit ownership checks.</p>
  </div>
  <div class="sc-card">
    <h3><a href="xref:SwiftCollections.BoundVolumeExtensions">Unity bounds</a></h3>
    <p>Convert <code>Bounds</code> to floating-point query volumes at the engine
    boundary.</p>
  </div>
  <div class="sc-card">
    <h3><a href="xref:SwiftCollections.FixedBoundVolumeExtensions">Fixed-point queries</a></h3>
    <p>Add the matching companion package for FixedMathSharp-backed bounds and
    deterministic spatial query structures.</p>
  </div>
</div>

## Package pairing

| Base | Optional FixedMathSharp companion |
| --- | --- |
| `com.mrdav30.swiftcollections` | `com.mrdav30.swiftcollections.fixedmathsharp` |
| `com.mrdav30.swiftcollections.lean` | `com.mrdav30.swiftcollections.fixedmathsharp.lean` |

Install exactly one base. Companions extend their matching base and do not
replace it.

## Resources

- [Getting started](guides/getting-started.md)
- [Unity serialization](guides/serialization.md)
- [Pooling and spatial queries](guides/pooling-and-queries.md)
- [SwiftCollections core guide and API](https://mrdav30.github.io/SwiftCollections/)
- [FixedMathSharp core guide and API](https://mrdav30.github.io/FixedMathSharp/)
- [Package source, issues, and releases](https://github.com/mrdav30/SwiftCollections-Unity)

The API reference is generated only from the canonical adapter source under
`Build/Base/Runtime` and `Build/FixedMathSharp/Runtime`. Core APIs remain owned
by their engine-agnostic documentation sites.
