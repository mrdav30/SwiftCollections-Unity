# SwiftCollections for Unity (Lean Variant)

SwiftCollections package for Unity without `MemoryPack`.

This is the safer default when Burst AOT compatibility matters or when you want
to bring your own serialization stack. Install one SwiftCollections variant
only.

## Install

`https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.lean`

## When To Use This Package

- You want SwiftCollections without `MemoryPack`.
- You are targeting Burst AOT or want the safer default for Burst-oriented
  builds.
- You do not need `FixedMathSharp` fixed-point query helpers.

## Choose Another Variant If

- Use `com.mrdav30.swiftcollections` if you want the standard build.
- Use one of the `*.fixedmathsharp*` variants if you need
  `FixedBoundVolume` or `Bounds.ToFixedBoundVolume()`.

## Related

- Repo overview and variant selection:
  [SwiftCollections-Unity](https://github.com/mrdav30/SwiftCollections-Unity)
