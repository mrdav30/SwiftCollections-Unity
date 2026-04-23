# SwiftCollections for Unity (FixedMathSharp Variant)

SwiftCollections variant for projects that use `FixedMathSharp`.

Use this package when you want SwiftCollections plus fixed-point query helpers
such as `FixedBoundVolume` and `Bounds.ToFixedBoundVolume()`. Install one
SwiftCollections variant only.

## Install

`https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp`

## Dependency Handling

This package includes an editor-side dependency installer that attempts to add
the matching `FixedMathSharp-Unity` package automatically.

If Unity does not resolve that dependency cleanly, install it manually or run:

`Tools > mrdav30 > Repair com.mrdav30.swiftcollections.fixedmathsharp Dependencies`

Manual dependency URL:

`https://github.com/mrdav30/FixedMathSharp-Unity.git?path=/com.mrdav30.fixedmathsharp`

## When To Use This Package

- You need SwiftCollections with `FixedMathSharp` interop.
- You want the standard FixedMathSharp-enabled build.
- You do not want the lean no-`MemoryPack` variant.

## Choose Another Variant If

- Use `com.mrdav30.swiftcollections.fixedmathsharp.lean` if you want the
  FixedMathSharp variant without `MemoryPack`.
- Use a non-FixedMathSharp variant if you do not need fixed-point query
  volumes.

## Related

- Repo overview and variant selection:
  [SwiftCollections-Unity](https://github.com/mrdav30/SwiftCollections-Unity)
