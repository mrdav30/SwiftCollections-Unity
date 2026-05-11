# SwiftCollections for Unity (Lean Variant)

SwiftCollections package for Unity without `MemoryPack`.

This is the safer default when Burst AOT compatibility matters or when you want
to bring your own serialization stack. Install the standard base package instead
if you want the default `MemoryPack` build.

## Install

`https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.lean`

## When To Use This Package

- You want SwiftCollections without `MemoryPack`.
- You are targeting Burst AOT or want the safer default for Burst-oriented
  builds.
- You do not need `FixedMathSharp` fixed-point query helpers.

## Optional Modules

- Add `com.mrdav30.swiftcollections.fixedmathsharp.lean` if this project also
  uses `FixedMathSharp` and needs the matching no-`MemoryPack` fixed-point
  query helpers.
- Do not pair this lean base package with
  `com.mrdav30.swiftcollections.fixedmathsharp`; that module is for the
  standard base package.

## Related

- Repo overview and package selection:
  [SwiftCollections-Unity](https://github.com/mrdav30/SwiftCollections-Unity)
