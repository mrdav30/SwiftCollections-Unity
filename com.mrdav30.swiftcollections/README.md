# SwiftCollections for Unity

The Standard base package: SwiftCollections runtime assemblies plus Unity
authoring adapters, bounds conversion, GameObject pooling, and the Demo Scene.
This variant includes MemoryPack support.

Requires Unity 2022.3 or newer.

[Documentation](https://mrdav30.github.io/SwiftCollections-Unity/) ·
[Getting started](https://mrdav30.github.io/SwiftCollections-Unity/guides/getting-started.html) ·
[Changelog](CHANGELOG.md)

## Install

In **Window → Package Manager**, choose **+ → Install package from git URL…**:

```text
https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections#v7.0.0
```

Do not install the Lean base in the same project. In a custom asmdef, reference
`SwiftCollections.Runtime`.

## Author Unity data

Use a `SerializedSwift*` wrapper for Inspector or asset data and access its
runtime collection through `Runtime`:

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

Direct SwiftCollections runtime fields retain their upstream state, JSON, and
MemoryPack contracts, but Unity does not persist their contents as authoring
fields. Generic values inside adapters must also be Unity-serializable.

Read the [serialization guide](https://mrdav30.github.io/SwiftCollections-Unity/guides/serialization.html)
for the adapter matrix and validation rules.

## Optional FixedMathSharp companion

Add `com.mrdav30.swiftcollections.fixedmathsharp` when the project also uses the
Standard FixedMathSharp-Unity package and needs `FixedBoundVolume` or
`Bounds.ToFixedBoundVolume()`. Do not pair this base with the Lean companion.

## Sample

Import **Demo Scene** from this package's **Samples** tab. It demonstrates core
collections, GameObject pooling, and BVH versus linear bounds queries.
