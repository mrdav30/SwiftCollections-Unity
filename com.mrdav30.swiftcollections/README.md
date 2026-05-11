# SwiftCollections for Unity

Default SwiftCollections package for Unity.

Choose this base package when you want the standard SwiftCollections build for
Unity. Install the lean base package instead if you want the no-`MemoryPack`
build.

## Install

`https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections`

## When To Use This Package

- You want the default SwiftCollections package.
- You want the standard build rather than the no-`MemoryPack` lean build.
- You do not need `FixedMathSharp` fixed-point query helpers.

## Optional Modules

- Add `com.mrdav30.swiftcollections.fixedmathsharp` if this project also uses
  `FixedMathSharp` and needs `FixedBoundVolume` or
  `Bounds.ToFixedBoundVolume()`.
- Do not pair this standard base package with
  `com.mrdav30.swiftcollections.fixedmathsharp.lean`; that module is for the
  lean base package.

## Related

- Repo overview and package selection:
  [SwiftCollections-Unity](https://github.com/mrdav30/SwiftCollections-Unity)
