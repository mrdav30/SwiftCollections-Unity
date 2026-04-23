# SwiftCollections for Unity (FixedMathSharp Lean Variant)

SwiftCollections variant for `FixedMathSharp` projects without `MemoryPack`.

This is the Burst-friendlier FixedMathSharp option when you want to avoid the
default serialization dependency. Install one SwiftCollections variant only.

## Install

`https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp.lean`

## Dependency Handling

This package includes an editor-side dependency installer that attempts to add
the matching `FixedMathSharp-Unity` package automatically.

If Unity does not resolve that dependency cleanly, install it manually or run:

`Tools > mrdav30 > Repair com.mrdav30.swiftcollections.fixedmathsharp.lean Dependencies`

Manual dependency URL:

`https://github.com/mrdav30/FixedMathSharp-Unity.git?path=/com.mrdav30.fixedmathsharp.lean`

## When To Use This Package

- You need SwiftCollections with `FixedMathSharp` interop.
- You want the no-`MemoryPack` variant for Burst-oriented or custom-serialization
  setups.
- You want the FixedMathSharp package choice instead of the standard Unity-only
  variants.

## Choose Another Variant If

- Use `com.mrdav30.swiftcollections.fixedmathsharp` if you want the standard
  FixedMathSharp-enabled build.
- Use a non-FixedMathSharp variant if you do not need fixed-point query
  volumes.

## Related

- Repo overview and variant selection:
  [SwiftCollections-Unity](https://github.com/mrdav30/SwiftCollections-Unity)
