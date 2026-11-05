# SwiftCollections for Unity — Lean

The Lean base package: the same SwiftCollections and Unity adapter APIs as the
Standard base, without the MemoryPack runtime dependency. It includes Unity
authoring adapters, bounds conversion, GameObject pooling, and the Demo Scene.

Requires Unity 6.3 LTS (6000.3) or newer.

[Documentation](https://mrdav30.github.io/SwiftCollections-Unity/) ·
[Getting started](https://mrdav30.github.io/SwiftCollections-Unity/guides/getting-started.html) ·
[Changelog](CHANGELOG.md)

## Install

In **Window → Package Manager**, choose **+ → Install package from git URL…**:

```text
https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.lean#v8.0.0
```

Do not install the Standard base in the same project. In a custom asmdef,
reference `SwiftCollections.Lean.Runtime`.

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

Direct SwiftCollections runtime fields retain their upstream state and JSON
contracts, but Unity does not persist their contents as authoring fields.
Generic values inside adapters must also be Unity-serializable.

Lean's smaller dependency surface can suit projects with their own
serialization stack. It is not a blanket guarantee that every collection or
Unity adapter is Burst-compatible.

Read the [serialization guide](https://mrdav30.github.io/SwiftCollections-Unity/guides/serialization.html)
for the adapter matrix and validation rules.

## Optional FixedMathSharp companion

Add `com.mrdav30.swiftcollections.fixedmathsharp.lean` when the project also
uses the Lean FixedMathSharp-Unity package and needs `FixedBoundVolume` or
`Bounds.ToFixedBoundVolume()`. Do not pair this base with the Standard
companion.

## Sample

Import **Demo Scene** from this package's **Samples** tab. It demonstrates core
collections, GameObject pooling, and BVH versus linear bounds queries.
