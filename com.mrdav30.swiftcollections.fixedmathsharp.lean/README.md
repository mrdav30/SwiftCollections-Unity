# SwiftCollections for Unity (FixedMathSharp Lean Module)

FixedMathSharp interop module for the lean SwiftCollections package.

Use this package alongside `com.mrdav30.swiftcollections.lean` when you want
the no-`MemoryPack` base package plus fixed-point query helpers such as
`FixedBoundVolume` and `Bounds.ToFixedBoundVolume()`.

## Install

`https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp.lean`

## Dependency Handling

This package includes an editor-side dependency installer that attempts to add
the matching `FixedMathSharp-Unity` package automatically.

This package also requires the lean SwiftCollections base package:

`https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.lean`

If Unity does not resolve that dependency cleanly, install it manually or run:

`Tools > mrdav30 > Repair com.mrdav30.swiftcollections.fixedmathsharp.lean Dependencies`

Manual dependency URL:

`https://github.com/mrdav30/FixedMathSharp-Unity.git?path=/com.mrdav30.fixedmathsharp.lean`

## When To Use This Package

- Your project already uses `com.mrdav30.swiftcollections.lean`.
- You need `FixedMathSharp` interop for SwiftCollections query structures.
- You want the module that matches the no-`MemoryPack` base package.

## Choose The Standard Module If

- Use `com.mrdav30.swiftcollections.fixedmathsharp` only when your base package
  is `com.mrdav30.swiftcollections`.

## Related

- Repo overview and package selection:
  [SwiftCollections-Unity](https://github.com/mrdav30/SwiftCollections-Unity)
