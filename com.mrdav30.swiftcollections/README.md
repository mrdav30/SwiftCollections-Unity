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

## Unity Serialization

Direct SwiftCollections fields are runtime collection fields. They keep the
upstream JSON, MemoryPack, and state serialization contracts, but Unity does
not persist their contents directly. Use these adapters for Unity authoring
fields and consume the real collection through `Runtime`.

| Serialized field type | Runtime type | Notes |
| --- | --- | --- |
| `SerializedSwiftList<T>` | `SwiftList<T>` | Ordered list data. |
| `SerializedSwiftDictionary<TKey, TValue>` | `SwiftDictionary<TKey, TValue>` | Duplicate keys are invalid. |
| `SerializedSwiftArray2D<T>` | `SwiftArray2D<T>` | Flat data length must match width and height. |
| `SerializedSwiftArray3D<T>` | `SwiftArray3D<T>` | Flat data length must match width, height, and depth. |
| `SerializedSwiftSparseSet` | `SwiftSparseSet` | IDs must be non-negative and unique. |
| `SerializedSwiftSparseMap<T>` | `SwiftSparseMap<T>` | Keys must be non-negative and unique. |
| `SerializedSwiftBiDictionary<TLeft, TRight>` | `SwiftBiDictionary<TLeft, TRight>` | Left and right keys must each be unique. |

Generic item, key, and value types must themselves be Unity-serializable.

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
