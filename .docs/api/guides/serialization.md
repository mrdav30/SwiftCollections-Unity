---
title: Unity serialization
description: Persist supported SwiftCollections authoring data in scenes, prefabs, MonoBehaviours, and ScriptableObjects.
---

# Unity serialization

SwiftCollections types are runtime collections. Their upstream state, JSON, and
optional MemoryPack contracts do not make them Unity field-serialization
adapters.

When Unity must persist collection contents in a scene, prefab, MonoBehaviour,
or ScriptableObject, use a type from <xref:SwiftCollections.Unity>. Each adapter
stores a Unity-friendly backing shape and exposes the live SwiftCollections
instance through `Runtime`.

## Adapter matrix

| Serialized field | Runtime collection | Unity backing shape | Validation |
| --- | --- | --- | --- |
| `SerializedSwiftList<T>` | `SwiftList<T>` | `T[]` | Item types must be Unity-serializable. |
| `SerializedSwiftDictionary<TKey, TValue>` | `SwiftDictionary<TKey, TValue>` | Key/value entry array | Keys must be unique. |
| `SerializedSwiftBiDictionary<TLeft, TRight>` | `SwiftBiDictionary<TLeft, TRight>` | Left/right entry array | Left and right values must each be unique. |
| `SerializedSwiftArray2D<T>` | `SwiftArray2D<T>` | Width, height, flat `T[]` | Data length must equal `width * height`. |
| `SerializedSwiftArray3D<T>` | `SwiftArray3D<T>` | Width, height, depth, flat `T[]` | Data length must equal `width * height * depth`. |
| `SerializedSwiftSparseSet` | `SwiftSparseSet` | `int[]` | IDs must be non-negative and unique. |
| `SerializedSwiftSparseMap<T>` | `SwiftSparseMap<T>` | Integer key/value entry array | Keys must be non-negative and unique. |

Generic item, key, and value types remain subject to Unity's own serialization
rules.

## Runtime and backing data stay aligned

Unity calls `OnAfterDeserialize` to rebuild `Runtime` from the serialized
backing data. Before Unity serializes the adapter, `OnBeforeSerialize` copies
live runtime contents back into that backing shape.

The adapters also expose mutating methods such as `Add`, `Clear`, `SetItems`,
and indexed assignment where the underlying collection supports them. These
methods update both representations immediately.

```csharp
using SwiftCollections;
using SwiftCollections.Unity;
using UnityEngine;

public sealed class EncounterTable : MonoBehaviour
{
    [SerializeField]
    private SerializedSwiftDictionary<string, int> _weights =
        new SerializedSwiftDictionary<string, int>();

    public SwiftDictionary<string, int> Weights => _weights.Runtime;

    public void SetWeight(string encounterId, int weight)
    {
        _weights[encounterId] = weight;
    }
}
```

Use `ToSwiftList`, `ToSwiftDictionary`, and the other `ToSwift*` methods when
you need a standalone copy rather than the adapter's live runtime instance.

## Invalid authoring data fails explicitly

Adapters validate their invariants when data is assigned and when Unity rebuilds
runtime state. Duplicate dictionary keys, duplicate bi-dictionary sides,
negative sparse IDs, and array dimensions that do not match their flat data
raise exceptions rather than silently dropping or reordering data.

This behavior makes broken scene, prefab, or asset data visible. Treat the
serialized backing shape as authored input with the same invariants required by
the runtime collection.

## Standard and Lean

The Unity adapter surface is the same in both bases:

| Capability | Standard | Lean |
| --- | --- | --- |
| Unity `SerializedSwift*` adapters | Yes | Yes |
| Upstream state and JSON contracts | Yes | Yes |
| MemoryPack runtime | Yes | No |

Choose Lean when you do not need MemoryPack and prefer the smaller dependency
surface. Unity field serialization is independent of that package choice.

For the engine-agnostic serialization states and converters, see the
[SwiftCollections overview](https://mrdav30.github.io/SwiftCollections/guides/OVERVIEW.html).
