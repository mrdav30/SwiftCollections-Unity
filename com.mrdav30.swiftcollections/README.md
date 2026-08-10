# SwiftCollections for Unity

Default SwiftCollections package for Unity.

[API Documentation](https://mrdav30.github.io/SwiftCollections-Unity/) ·
[Changelog](CHANGELOG.md)

Choose this base package when you want the standard SwiftCollections build for
Unity. Install the lean base package instead if you want the no-`MemoryPack`
build.

## Install

`https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections`

## Included

- The MemoryPack-enabled SwiftCollections runtime.
- Unity-serializable collection adapters.
- Unity bounds conversion and GameObject pooling helpers.

## Unity Serialization

Direct SwiftCollections fields are runtime collection fields. They keep the
upstream JSON, MemoryPack, and state serialization contracts, but Unity does not
persist their contents directly. Use these adapters for Unity authoring fields
and consume the real collection through `Runtime`.

See the [API documentation](https://mrdav30.github.io/SwiftCollections-Unity/)
for the available `SerializedSwift*` adapters. Generic item, key, and value
types must themselves be Unity-serializable.

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
