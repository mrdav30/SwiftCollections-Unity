---
title: Getting started
description: Choose a SwiftCollections Unity package family, install it reproducibly, connect asmdefs, and import the sample.
---

# Getting started

SwiftCollections for Unity is the host layer around the engine-agnostic
[SwiftCollections library](https://github.com/mrdav30/SwiftCollections). Keep
collection and query state in the core types. Use the adapters when Unity owns
the boundary: serialized authoring data, `Bounds`, GameObjects, and package
workflows.

The packages require Unity **6.3 LTS (6000.3) or newer**. Repository CI currently
validates Unity **6000.3.9f1**.

## 1. Choose exactly one base

| Variant | Package ID | Use it when |
| --- | --- | --- |
| Standard | `com.mrdav30.swiftcollections` | You want MemoryPack support with the runtime and Unity adapters. |
| Lean | `com.mrdav30.swiftcollections.lean` | You want the same collection and adapter APIs without the MemoryPack runtime. |

> [!WARNING]
> Do not install both bases. They contain alternate builds of the same
> SwiftCollections assemblies and types.

Lean is a serialization and dependency choice. Removing MemoryPack does not
make every managed collection, Unity object, or serialization callback
Burst-compatible.

## 2. Install from Git

Make sure Git is installed and available to the Unity Editor. Open **Window →
Package Manager**, select **+**, choose **Install package from git URL…**, and
paste one URL:

### Standard

```text
https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections#v8.0.0
```

### Lean

```text
https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.lean#v8.0.0
```

The `#v8.0.0` revision makes the install reproducible. Update it deliberately
when adopting a newer release.

If your scripts live in a custom asmdef, add the matching reference:

- Standard: `SwiftCollections.Runtime`
- Lean: `SwiftCollections.Lean.Runtime`

To switch variants, remove the installed base and any companion, let Unity
finish compiling, install the other family, and update custom asmdef references.

## 3. Add fixed-point queries only when needed

The base package already contains floating-point bounds and spatial query
structures. Add a FixedMathSharp companion only when your project uses
fixed-point query volumes:

| Installed base | Matching companion | Matching FixedMathSharp-Unity package |
| --- | --- | --- |
| Standard | `com.mrdav30.swiftcollections.fixedmathsharp` | `com.mrdav30.fixedmathsharp` |
| Lean | `com.mrdav30.swiftcollections.fixedmathsharp.lean` | `com.mrdav30.fixedmathsharp.lean` |

### Standard companion

```text
https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp#v8.0.0
```

### Lean companion

```text
https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp.lean#v8.0.0
```

Each companion's editor installer attempts to add its matching
FixedMathSharp-Unity package at `v8.0.0`. It does not install the matching
SwiftCollections base; install that first.

If the FixedMathSharp dependency is missing, use **Tools → mrdav30 → Repair
&lt;package-id&gt; Dependencies** or add the matching package URL manually.

Custom asmdefs that consume the companion surface should reference the matching
base, companion, and FixedMathSharp runtime assemblies.

## 4. Pick the Unity boundary

### Authoring data

Use `SerializedSwift*` wrappers when a collection must persist in a scene,
prefab, MonoBehaviour, or ScriptableObject:

```csharp
using SwiftCollections;
using SwiftCollections.Unity;
using UnityEngine;

public sealed class SpawnTable : ScriptableObject
{
    [SerializeField]
    private SerializedSwiftList<int> _spawnIds = new SerializedSwiftList<int>();

    public SwiftList<int> SpawnIds => _spawnIds.Runtime;
}
```

Read [Serialization](serialization.md) for the supported adapters and their
validation rules.

### Unity bounds

Convert engine bounds once before passing them to a core spatial query:

```csharp
using SwiftCollections;
using SwiftCollections.Query;
using UnityEngine;

Bounds unityBounds = new(Vector3.zero, Vector3.one * 2f);
BoundVolume queryBounds = unityBounds.ToBoundVolume();
```

The companion packages add `ToFixedBoundVolume()`. Read
[Pooling and queries](pooling-and-queries.md) for query and GameObject ownership
guidance.

## 5. Import the sample

Select the installed base package in Package Manager, open its **Samples** tab,
and import **Demo Scene**. Unity copies it into `Assets/Samples`.

The sample demonstrates:

- `SwiftDictionary`, `SwiftHashSet`, and `SwiftList` in a Unity workflow;
- Unity `Bounds` converted for a `SwiftBVH<int>` query;
- a live BVH-versus-linear scan comparison—press **Tab** to switch; and
- budgeted GameObject pooling for projectiles.

The Standard and Lean bases carry matching samples. Companion packages do not
contain separate samples.

## Next steps

- Choose a [Unity serialization shape](serialization.md).
- Configure [pooling and spatial queries](pooling-and-queries.md).
- Browse the [Unity adapter API](xref:SwiftCollections).
- Read the [SwiftCollections core guide and API](https://mrdav30.github.io/SwiftCollections/)
  for container tradeoffs, state serialization, query behavior, and
  diagnostics.

## Upgrading to v8

See the [Unity migration guide](https://github.com/mrdav30/SwiftCollections-Unity/blob/v8.0.0/MIGRATION.md)
for core type renames, Chronicler imports, and dependency pairing.
