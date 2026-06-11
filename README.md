# SwiftCollections-Unity

[![build-and-test](https://github.com/mrdav30/SwiftCollections-Unity/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/mrdav30/SwiftCollections-Unity/actions/workflows/build-and-test.yml)

Unity Package Manager host for [SwiftCollections](https://github.com/mrdav30/SwiftCollections).

This repository ships two base Unity packages and two optional FixedMathSharp companion packages. Choose one base package, then add the matching FixedMathSharp companion only if your project uses fixed-point query volumes.

- Current package version: `5.0.0`
- Supported Unity version: `2022.3+`

## Packages

| Package | Use it when | Install |
| --- | --- | --- |
| `com.mrdav30.swiftcollections` | You want the standard SwiftCollections package for Unity, including `Bounds.ToBoundVolume()` and MemoryPack-enabled assemblies. | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections` |
| `com.mrdav30.swiftcollections.lean` | You want the same Unity integration without MemoryPack. Prefer this for Burst AOT or your own serialization stack. | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.lean` |
| `com.mrdav30.swiftcollections.fixedmathsharp` | You already use `com.mrdav30.swiftcollections` and need FixedMathSharp query helpers such as `FixedBoundVolume` and `Bounds.ToFixedBoundVolume()`. | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp` |
| `com.mrdav30.swiftcollections.fixedmathsharp.lean` | You already use `com.mrdav30.swiftcollections.lean` and need the matching no-MemoryPack FixedMathSharp companion. | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp.lean` |

## Package Pairing

Base packages:

- Install either `com.mrdav30.swiftcollections` or `com.mrdav30.swiftcollections.lean`.
- Do not install both base packages in the same Unity project.

FixedMathSharp companions:

- Install `com.mrdav30.swiftcollections.fixedmathsharp` with `com.mrdav30.swiftcollections`.
- Install `com.mrdav30.swiftcollections.fixedmathsharp.lean` with `com.mrdav30.swiftcollections.lean`.
- The companions add fixed-point bounds interop; they do not replace the base packages.

## FixedMathSharp Dependency

The `*.fixedmathsharp*` packages include an editor-side dependency installer that attempts to add the matching FixedMathSharp-Unity Git dependency. They still require the matching SwiftCollections base package listed above.

If Unity does not resolve the FixedMathSharp dependency cleanly, add the matching dependency URL manually or run the package repair menu item under `Tools > mrdav30`.

- Standard dependency: `https://github.com/mrdav30/FixedMathSharp-Unity.git?path=/com.mrdav30.fixedmathsharp`
- Lean dependency: `https://github.com/mrdav30/FixedMathSharp-Unity.git?path=/com.mrdav30.fixedmathsharp.lean`

## Tests

The `build-and-test` workflow runs Unity EditMode tests on Unity `6000.3.9f1` for all four package variants:

- SwiftCollections
- SwiftCollections Lean
- SwiftCollections FixedMathSharp
- SwiftCollections FixedMathSharp Lean

The base package tests live in `Tests/EditMode` and cover Unity `Bounds` conversion, a representative BVH query, core DLL loading, and `SwiftGameObjectPool` reuse and exhaustion behavior. FixedMathSharp companion tests live in `Tests/EditMode.FixedMathSharp` and cover `Bounds.ToFixedBoundVolume()`.

## Notes

- Most core collection behavior lives in the upstream [SwiftCollections](https://github.com/mrdav30/SwiftCollections) library and is shipped here as precompiled plugin DLLs.
- Shared Unity-side runtime files are maintained under `Build/Base` and `Build/FixedMathSharp`, then synchronized into the package folders.
- Each package folder keeps a short, package-specific install README.
