---
title: Pooling and spatial queries
description: Own pooled GameObjects explicitly and convert Unity Bounds for SwiftCollections spatial queries.
---

# Pooling and spatial queries

These adapters exist where Unity engine objects meet SwiftCollections runtime
data. Keep the conversion and ownership boundary visible.

## Convert bounds before querying

The base packages add <xref:SwiftCollections.BoundVolumeExtensions>.
`Bounds.ToBoundVolume()` copies Unity's minimum and maximum coordinates into a
`System.Numerics`-backed <xref:SwiftCollections.Query.BoundVolume>.

```csharp
using SwiftCollections;
using SwiftCollections.Query;
using UnityEngine;

Bounds unityBounds = new(Vector3.zero, new Vector3(4f, 2f, 4f));
BoundVolume queryBounds = unityBounds.ToBoundVolume();

var bvh = new SwiftBVH<int>(capacity: 64);
bvh.Insert(1, queryBounds);
```

The base library owns `SwiftBVH<TKey>`, `SwiftSpatialHash<TKey>`, and
`SwiftOctree<TKey>`. Choose between them using the
[core query guide](https://mrdav30.github.io/SwiftCollections/guides/OVERVIEW.html#spatial-queries).

## Add fixed-point bounds deliberately

The matching FixedMathSharp companion adds
<xref:SwiftCollections.FixedBoundVolumeExtensions>. The conversion maps Unity
float bounds to `Vector3d` at the engine boundary and returns a
<xref:SwiftCollections.Query.FixedBoundVolume>.

```csharp
using SwiftCollections;
using SwiftCollections.Query;
using UnityEngine;

Bounds unityBounds = new(Vector3.zero, new Vector3(4f, 2f, 4f));
FixedBoundVolume queryBounds = unityBounds.ToFixedBoundVolume();
```

Install the Standard companion with the Standard base and
`com.mrdav30.fixedmathsharp`. Install the Lean companion with both Lean bases.
Do not mix package families.

The FixedMathSharp companion converts existing Unity float data; that initial
input is not made deterministic retroactively. Convert or load authoritative
values once, then keep subsequent simulation state in fixed-point types.

## Configure a GameObject pool

<xref:SwiftCollections.Pool.SwiftGameObjectPool> owns one prefab, a name, a
positive budget, and an optional prewarm flag. It reuses released inactive
instances before creating more and refuses to exceed its budget.

<xref:SwiftCollections.Pool.SwiftGameObjectPoolAsset> groups those definitions
in a ScriptableObject, validates unique non-empty names and valid prefabs, and
creates a persistent pool root when initialized.

To use the shared <xref:SwiftCollections.Pool.SwiftGameObjectPoolManager>:

1. Create a **SwiftCollections → SwiftGameObjectPoolAsset** asset.
2. Configure each pool with a unique name, prefab, positive budget, and optional
   prewarm.
3. Place the asset in a `Resources` folder and name it
   `SwiftGameObjectPoolAsset`.
4. Rent with `GetObject` or `TryGetObject`.
5. Return the instance to the same pool with `ReleaseObject`.

```csharp
using SwiftCollections.Pool;
using UnityEngine;

if (SwiftGameObjectPoolManager.Shared.TryGetObject("Projectile", out GameObject projectile))
{
    projectile.SetActive(true);

    // Return it when the projectile is finished.
    SwiftGameObjectPoolManager.Shared.ReleaseObject("Projectile", projectile);
}
```

`TryGetObject` returns `false` when the named pool is missing or exhausted.
`ReleaseObject` throws when the pool does not own the checked-out instance.
These checks make pool ownership errors visible instead of silently accepting a
foreign or double-released GameObject.

## See both paths in the sample

Import **Demo Scene** from either base package. It launches pooled projectiles
and visualizes a BVH query beside a linear scan. Press **Tab** in Play Mode to
switch the active query path and compare the behavior.
