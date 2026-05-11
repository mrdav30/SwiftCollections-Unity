# SwiftCollections for Unity (FixedMathSharp Module)

FixedMathSharp interop module for the standard SwiftCollections package.

Use this package alongside `com.mrdav30.swiftcollections` when you want
fixed-point query helpers such as `FixedBoundVolume` and
`Bounds.ToFixedBoundVolume()`.

## Install

`https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp`

## Dependency Handling

This package includes an editor-side dependency installer that attempts to add
the matching `FixedMathSharp-Unity` package automatically.

This package also requires the standard SwiftCollections base package:

`https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections`

If Unity does not resolve that dependency cleanly, install it manually or run:

`Tools > mrdav30 > Repair com.mrdav30.swiftcollections.fixedmathsharp Dependencies`

Manual dependency URL:

`https://github.com/mrdav30/FixedMathSharp-Unity.git?path=/com.mrdav30.fixedmathsharp`

## When To Use This Package

- Your project already uses `com.mrdav30.swiftcollections`.
- You need `FixedMathSharp` interop for SwiftCollections query structures.
- You want the module that matches the standard `MemoryPack` base package.

## Choose The Lean Module If

- Use `com.mrdav30.swiftcollections.fixedmathsharp.lean` only when your base
  package is `com.mrdav30.swiftcollections.lean`.

## Related

- Repo overview and package selection:
  [SwiftCollections-Unity](https://github.com/mrdav30/SwiftCollections-Unity)
