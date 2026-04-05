# Sample Scene Guide

The demo lives under `Samples/SwiftCollectionsDemo` and is built to showcase:

- `SwiftGameObjectPool` as a true reusable pool with explicit checkout and return
- `SwiftList`, `SwiftDictionary`, and `SwiftHashSet` in visible scene-facing roles
- `SwiftBVH<int>` for broad-phase spatial queries against moving targets
- runtime `Bounds` conversion helpers for `BoundVolume` and `FixedBoundVolume`

## Sample Layout

```text
Samples/
  SwiftCollectionsDemo/
    Scenes/
      DemoScene.unity
    Prefabs/
      Projectile.prefab
      QueryVolume.prefab
      Target.prefab
    Resources/
      SwiftGameObjectPoolAsset.asset
    Scripts/
      DemoBootstrap.cs
      DemoHud.cs
      PooledProjectile.cs
      ProjectileLauncher.cs
      SpatialQueryController.cs
      TargetAgent.cs
      TargetRegistry.cs
    SwiftCollections.Samples.asmdef
```

## How To Run The Sample

1. Open `Samples/SwiftCollectionsDemo/Scenes/DemoScene.unity`.
2. Confirm the sample assembly compiles and the sample prefabs stay linked.
3. Enter Play Mode.
4. Watch the launcher fire pooled projectiles at moving targets.
5. Press `Tab` to switch between `BVH Query` and `Linear Scan`.

The sample is designed to be immediately explorable. You should not need to hand-build the scene or manually create the pool asset first.

## What The Demo Shows

### Pooling Lane

- `ProjectileLauncher` repeatedly requests objects from `SwiftGameObjectPoolManager.Shared`.
- `PooledProjectile` moves forward, expires on lifetime or arena exit, and explicitly returns itself through `ReleaseObject(...)`.
- `TargetRegistry` tracks active projectile count and pool misses for the HUD.
- The launcher exposes `ConfiguredPoolBudget` and `PrewarmEnabled` as display values so the HUD can mirror the pool asset setup.

If the pool is exhausted, the demo increments a miss counter instead of stealing live objects.

### Collections Lane

The sample uses these SwiftCollections types directly:

| Type | Where it is used |
| --- | --- |
| `SwiftList<Transform>` | generated target spawn anchors in `DemoBootstrap` |
| `SwiftList<TargetAgent>` | registered targets for query comparisons in `SpatialQueryController` |
| `SwiftList<int>` | reusable query result buffers and previous highlight scratch data |
| `SwiftDictionary<int, TargetAgent>` | target lookup by stable runtime id in `TargetRegistry` |
| `SwiftHashSet<int>` | currently highlighted target ids in `TargetRegistry` |
| `SwiftBVH<int>` | broad-phase query structure in `SpatialQueryController` |

### BVH Lane

- `DemoBootstrap` spawns the target field at runtime.
- `TargetAgent` updates its movement every frame and reports new bounds back to `SpatialQueryController`.
- `SpatialQueryController` inserts target ids into `SwiftBVH<int>` and updates their bounds as they move.
- Each frame, the query volume sweeps across the arena and collects matching target ids.
- The registry applies highlight changes to the affected targets.

The `Tab` toggle is there specifically so you can compare `SwiftBVH` against a straightforward linear scan without changing scenes.

## Script Map

### `DemoBootstrap`

`DemoBootstrap` is the scene entry point.

It:

- resolves scene references
- initializes the target registry and spatial query controller
- generates spawn anchors at runtime using `SwiftList<Transform>`
- spawns and initializes the target field
- wires the launcher and HUD to the shared runtime state

### `ProjectileLauncher`

`ProjectileLauncher` owns the firing loop.

It:

- requests pooled objects from the `Projectile` pool
- positions them at the configured muzzle
- initializes `PooledProjectile`
- increments the pool miss counter when `TryGetObject(...)` fails

### `PooledProjectile`

`PooledProjectile` is intentionally small.

It:

- moves forward every frame
- keeps a simple runtime `SequenceId` so reuse is easy to spot
- returns itself to the pool on timeout, collision, or arena exit

### `TargetAgent`

`TargetAgent` handles movement and visual state.

The current movement modes are:

- `PingPongX`
- `PingPongZ`
- `Orbit`
- `VerticalBob`

It also converts its renderer bounds into a `BoundVolume` by calling the runtime extension method on `Bounds`.

### `TargetRegistry`

`TargetRegistry` owns the sample's shared state.

It tracks:

- target registration
- active projectile count
- pool miss count
- current query hit count
- query mode label
- highlighted target ids

### `SpatialQueryController`

`SpatialQueryController` owns the broad-phase query system.

It:

- builds the `SwiftBVH<int>`
- keeps a list of registered targets for linear scan fallback
- animates the query volume through the arena
- executes either `BVH Query` or `Linear Scan`
- sends highlight results back to the registry

### `DemoHud`

`DemoHud` is a simple IMGUI overlay that displays:

- pool id
- configured pool budget
- prewarm state
- active projectile count
- pool misses
- registered targets
- current query hits
- current query mode
- current query volume size

## Runtime Bounds Helpers

The bounds conversion helpers live in runtime:

- `Runtime/BoundVolume.Extensions.cs`
- `Runtime/FixedBoundVolume.Extensions.cs`

Usage in the sample:

- `Bounds.ToBoundVolume()` is used by `TargetAgent` and `SpatialQueryController`
- `Bounds.ToFixedBoundVolume()` is available for future fixed-point samples or gameplay code

## Editing The Sample

The easiest places to tune the demo are:

- `DemoBootstrap`
  - target count
  - target rows
  - arena dimensions
  - movement speed range
  - ping-pong amplitude
  - orbit radius
- `SpatialQueryController`
  - query volume size
  - sweep extents
  - sweep speed
  - animated query toggle
- `ProjectileLauncher`
  - shots per second
  - projectile speed
  - projectile lifetime
  - HUD-facing budget and prewarm values
- `Samples/SwiftCollectionsDemo/Resources/SwiftGameObjectPoolAsset.asset`
  - actual pool budget
  - actual prewarm setting
  - pooled prefab assignment

If you change the real pool asset settings, keep the launcher's display-only budget and prewarm fields in sync so the HUD stays truthful.

## Presenter Notes

If this sample is used in release notes, a video, or a README callout, these are the clearest talking points:

- The GameObject pool is explicit and non-stealing.
- Pool exhaustion is surfaced cleanly through a counter.
- Runtime scene state uses SwiftCollections containers instead of default BCL containers.
- `SwiftBVH<int>` drives the moving query volume and can be compared live against a linear scan.
- `Bounds.ToBoundVolume()` is now runtime API, not sample-only glue.

## Verification Checklist

Use this checklist after opening the sample in Unity:

- the sample assembly compiles
- `DemoScene.unity` opens without missing script references
- the `Projectile` pool resolves from `Resources/SwiftGameObjectPoolAsset`
- projectiles are reused and returned to the pool
- pool misses increment if the launcher outruns the configured pool budget
- targets highlight correctly while moving
- pressing `Tab` switches between `BVH Query` and `Linear Scan`
- the HUD values match the actual sample configuration
