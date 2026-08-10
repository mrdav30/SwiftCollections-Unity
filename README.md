<p align="center">
  <img src=".assets/icon.png" alt="SwiftCollections icon" width="96" height="96">
</p>

# SwiftCollections for Unity

[![Build](https://github.com/mrdav30/SwiftCollections-Unity/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/mrdav30/SwiftCollections-Unity/actions/workflows/build-and-test.yml)
[![Unity 2022.3+](https://img.shields.io/badge/Unity-2022.3%2B-222c37?logo=unity)](https://unity.com/releases/editor/whats-new/2022.3.0)
[![API](https://img.shields.io/badge/docs-Unity_API-6f4eb0)](https://mrdav30.github.io/SwiftCollections-Unity/)
[![Discord](https://img.shields.io/badge/discord-join%20community-5865F2?logo=discord&logoColor=white)](https://discord.gg/mhwK2QFNBA)

**Unity authoring, pooling, and query adapters for
[SwiftCollections](https://github.com/mrdav30/SwiftCollections).**

Keep runtime data in SwiftCollections. Use this package host where Unity owns
the boundary: Inspector-friendly collection fields, `Bounds` conversion,
GameObject pooling, samples, and optional FixedMathSharp query integration.

Requires Unity **2022.3 or newer**. Repository CI currently validates Unity
**6000.5.0f1**.

## Choose your packages

Install exactly one base:

| Base package                        | Use it when                                                                         |
| ----------------------------------- | ----------------------------------------------------------------------------------- |
| `com.mrdav30.swiftcollections`      | You want the Standard runtime with MemoryPack support.                              |
| `com.mrdav30.swiftcollections.lean` | You want the same collection and Unity adapter APIs without the MemoryPack runtime. |

Then, only if you use fixed-point query volumes, add the matching companion:

| Base                                | Optional companion                                 |
| ----------------------------------- | -------------------------------------------------- |
| `com.mrdav30.swiftcollections`      | `com.mrdav30.swiftcollections.fixedmathsharp`      |
| `com.mrdav30.swiftcollections.lean` | `com.mrdav30.swiftcollections.fixedmathsharp.lean` |

> [!WARNING] Do not mix Standard and Lean packages. The companions extend their
> matching base; they do not replace it.

## Install from Git

Make sure Git is available to the Unity Editor. In **Window → Package Manager**,
choose **+ → Install package from git URL…** and paste the URL you need:

```text
# Standard base
https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections#v7.0.0

# Lean base
https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.lean#v7.0.0

# Standard FixedMathSharp companion
https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp#v7.0.0

# Lean FixedMathSharp companion
https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp.lean#v7.0.0
```

Pinning `#v7.0.0` makes installs reproducible. Update the revision deliberately
when adopting a newer release.

If your scripts use a custom asmdef, add the matching reference:

| Package            | Runtime assembly                               |
| ------------------ | ---------------------------------------------- |
| Standard base      | `SwiftCollections.Runtime`                     |
| Lean base          | `SwiftCollections.Lean.Runtime`                |
| Standard companion | `SwiftCollections.FixedMathSharp.Runtime`      |
| Lean companion     | `SwiftCollections.FixedMathSharp.Lean.Runtime` |

## Author collections in Unity

Direct `SwiftList<T>`, `SwiftDictionary<TKey, TValue>`, and other runtime
collections are not Unity field-serialization adapters. Use the
`SerializedSwift*` wrappers for Inspector or asset data, then consume the real
collection through `Runtime`:

```csharp
using SwiftCollections;
using SwiftCollections.Unity;
using UnityEngine;

public sealed class SpawnTable : MonoBehaviour
{
    [SerializeField]
    private SerializedSwiftList<int> _spawnIds = new SerializedSwiftList<int>();

    public SwiftList<int> SpawnIds => _spawnIds.Runtime;
}
```

Generic item, key, and value types must also be supported by Unity
serialization. See the
[serialization guide](https://mrdav30.github.io/SwiftCollections-Unity/guides/serialization.html)
for every adapter and its validation rules.

## Query Unity bounds

The base package converts Unity bounds into the floating-point query volume:

```csharp
using SwiftCollections;
using SwiftCollections.Query;
using UnityEngine;

Bounds unityBounds = new(Vector3.zero, new Vector3(2f, 2f, 2f));
BoundVolume queryBounds = unityBounds.ToBoundVolume();
```

The FixedMathSharp companions add `Bounds.ToFixedBoundVolume()` and the matching
fixed-point query types.

Each companion includes an editor installer for its matching
FixedMathSharp-Unity package. You must install the matching SwiftCollections
base yourself. If the FixedMathSharp dependency needs repair, use:

```text
Tools → mrdav30 → Repair <package-id> Dependencies
```

## Try the sample

Select either base package in Package Manager, open **Samples**, and import
**Demo Scene**. It exercises core collections, GameObject pooling, and BVH
versus linear bounds queries; press **Tab** in Play Mode to switch query modes.
Companion packages do not contain separate samples.

## Burst-oriented projects

Lean removes the MemoryPack runtime dependency, but that alone does not make
every collection or Unity adapter Burst-compatible. Unity objects, managed
collections, serialization callbacks, and GameObject pooling remain managed
code. Validate the exact data path with your project's Burst version and
settings.

## Documentation

- [Getting started](https://mrdav30.github.io/SwiftCollections-Unity/guides/getting-started.html)
- [Serialization](https://mrdav30.github.io/SwiftCollections-Unity/guides/serialization.html)
- [Pooling and queries](https://mrdav30.github.io/SwiftCollections-Unity/guides/pooling-and-queries.html)
- [Unity adapter API](https://mrdav30.github.io/SwiftCollections-Unity/)
- [SwiftCollections core guide and API](https://mrdav30.github.io/SwiftCollections/)
- [Contributing and package sync](CONTRIBUTING.md)

Questions and discussion are welcome in the
[Discord community](https://discord.gg/mhwK2QFNBA). The packages are available
under the [MIT License](LICENSE).
