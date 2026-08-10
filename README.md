# SwiftCollections-Unity

[![Build](https://github.com/mrdav30/SwiftCollections-Unity/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/mrdav30/SwiftCollections-Unity/actions/workflows/build-and-test.yml)
[![Discord](https://img.shields.io/badge/discord-join%20community-5865F2?logo=discord&logoColor=white)](https://discord.gg/mhwK2QFNBA)

[API Documentation](https://mrdav30.github.io/SwiftCollections-Unity/)

Unity package host for
[SwiftCollections](https://github.com/mrdav30/SwiftCollections).

This repository ships two base Unity packages and two optional FixedMathSharp
companion packages. Choose one base package, then add the matching
FixedMathSharp companion only if your project uses fixed-point query volumes.

- Supported Unity version: `2022.3+`

## Packages

| Package                                            | Use it when                                                                                                                                        | Install                                                                                                        |
| -------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------- |
| `com.mrdav30.swiftcollections`                     | You want the standard SwiftCollections package for Unity, including `Bounds.ToBoundVolume()` and MemoryPack-enabled assemblies.                    | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections`                     |
| `com.mrdav30.swiftcollections.lean`                | You want the same Unity integration without MemoryPack. Prefer this for Burst AOT or your own serialization stack.                                 | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.lean`                |
| `com.mrdav30.swiftcollections.fixedmathsharp`      | You already use `com.mrdav30.swiftcollections` and need FixedMathSharp query helpers such as `FixedBoundVolume` and `Bounds.ToFixedBoundVolume()`. | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp`      |
| `com.mrdav30.swiftcollections.fixedmathsharp.lean` | You already use `com.mrdav30.swiftcollections.lean` and need the matching no-MemoryPack FixedMathSharp companion.                                  | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp.lean` |

## Package Pairing

Base packages:

- Install either `com.mrdav30.swiftcollections` or
  `com.mrdav30.swiftcollections.lean`.
- Do not install both base packages in the same Unity project.

FixedMathSharp companions:

- Install `com.mrdav30.swiftcollections.fixedmathsharp` with
  `com.mrdav30.swiftcollections`.
- Install `com.mrdav30.swiftcollections.fixedmathsharp.lean` with
  `com.mrdav30.swiftcollections.lean`.
- The companions add fixed-point bounds interop; they do not replace the base
  packages.

## Unity Serialization

The upstream SwiftCollections types are runtime collection types. They keep the
core JSON, MemoryPack, and state serialization contracts, but Unity does not
persist their contents when used directly as serialized fields. For Unity
authoring data, use the adapters in `SwiftCollections.Unity` and consume the
real collection through `Runtime`.

| Serialized field type                        | Runtime type                       | Unity backing shape              | Notes                                            |
| -------------------------------------------- | ---------------------------------- | -------------------------------- | ------------------------------------------------ |
| `SerializedSwiftList<T>`                     | `SwiftList<T>`                     | `T[]`                            | Ordered list data.                               |
| `SerializedSwiftDictionary<TKey, TValue>`    | `SwiftDictionary<TKey, TValue>`    | key/value entry array            | Duplicate keys are invalid.                      |
| `SerializedSwiftArray2D<T>`                  | `SwiftArray2D<T>`                  | width, height, flat `T[]`        | Data length must equal `width * height`.         |
| `SerializedSwiftArray3D<T>`                  | `SwiftArray3D<T>`                  | width, height, depth, flat `T[]` | Data length must equal `width * height * depth`. |
| `SerializedSwiftSparseSet`                   | `SwiftSparseSet`                   | `int[]`                          | IDs must be non-negative and unique.             |
| `SerializedSwiftSparseMap<T>`                | `SwiftSparseMap<T>`                | integer key/value entry array    | Keys must be non-negative and unique.            |
| `SerializedSwiftBiDictionary<TLeft, TRight>` | `SwiftBiDictionary<TLeft, TRight>` | left/right entry array           | Left and right keys must each be unique.         |

Generic item, key, and value types must themselves be Unity-serializable.

```csharp
using SwiftCollections;
using SwiftCollections.Unity;
using UnityEngine;

public sealed class SpawnTableAsset : ScriptableObject
{
    [SerializeField] private SerializedSwiftList<int> _spawnIds = new SerializedSwiftList<int>();
    [SerializeField] private SerializedSwiftDictionary<string, int> _weights = new SerializedSwiftDictionary<string, int>();

    public SwiftList<int> SpawnIds => _spawnIds.Runtime;
    public SwiftDictionary<string, int> Weights => _weights.Runtime;
}
```

## FixedMathSharp Dependency

The `*.fixedmathsharp*` packages include an editor-side dependency installer
that attempts to add the matching FixedMathSharp-Unity Git dependency. They
still require the matching SwiftCollections base package listed above.

If Unity does not resolve the FixedMathSharp dependency cleanly, add the
matching dependency URL manually or run the package repair menu item under
`Tools > mrdav30`.

- Standard dependency:
  `https://github.com/mrdav30/FixedMathSharp-Unity.git?path=/com.mrdav30.fixedmathsharp`
- Lean dependency:
  `https://github.com/mrdav30/FixedMathSharp-Unity.git?path=/com.mrdav30.fixedmathsharp.lean`
