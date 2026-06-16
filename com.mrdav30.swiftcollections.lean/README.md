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

## Unity Serialization

Direct SwiftCollections fields are runtime collection fields. They keep the
upstream JSON and state serialization contracts for the lean build, but Unity
does not persist their contents directly. Use these adapters for Unity
authoring fields and consume the real collection through `Runtime`.

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

- Add `com.mrdav30.swiftcollections.fixedmathsharp.lean` if this project also
  uses `FixedMathSharp` and needs the matching no-`MemoryPack` fixed-point
  query helpers.
- Do not pair this lean base package with
  `com.mrdav30.swiftcollections.fixedmathsharp`; that module is for the
  standard base package.

## Related

- Repo overview and package selection:
  [SwiftCollections-Unity](https://github.com/mrdav30/SwiftCollections-Unity)
