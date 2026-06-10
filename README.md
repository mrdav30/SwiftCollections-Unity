# SwiftCollections-Unity

[![Build](https://github.com/mrdav30/SwiftCollections-Unity/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/mrdav30/SwiftCollections-Unity/actions/workflows/build-and-test.yml)

Unity package host for [SwiftCollections](https://github.com/mrdav30/SwiftCollections).

This repository contains two base Unity Package Manager packages and two
optional FixedMathSharp modules. Choose one base package, then add the matching
FixedMathSharp module only if your project uses `FixedMathSharp`.

## Which Packages Should I Use?

| Package | Use it when | Install |
| --- | --- | --- |
| `com.mrdav30.swiftcollections` | You want the default SwiftCollections package for standard Unity and `Bounds` workflows. | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections` |
| `com.mrdav30.swiftcollections.lean` | You want the same core package without `MemoryPack`. Prefer this for Burst AOT or your own serialization stack. | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.lean` |
| `com.mrdav30.swiftcollections.fixedmathsharp` | You already use `com.mrdav30.swiftcollections` and need `FixedMathSharp` query helpers such as `FixedBoundVolume` and `Bounds.ToFixedBoundVolume()`. | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp` |
| `com.mrdav30.swiftcollections.fixedmathsharp.lean` | You already use `com.mrdav30.swiftcollections.lean` and need the matching no-`MemoryPack` FixedMathSharp module. | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp.lean` |

## How The Packages Fit Together

Base packages:

- Install `com.mrdav30.swiftcollections` for the standard build.
- Install `com.mrdav30.swiftcollections.lean` when you want the no-`MemoryPack`
  build for Burst AOT or custom serialization workflows.
- Do not install both base packages together.

FixedMathSharp modules:

- Install `com.mrdav30.swiftcollections.fixedmathsharp` alongside
  `com.mrdav30.swiftcollections`.
- Install `com.mrdav30.swiftcollections.fixedmathsharp.lean` alongside
  `com.mrdav30.swiftcollections.lean`.
- These modules add fixed-point query and bounds interop for projects that use
  `FixedMathSharp`; they are not replacements for the base packages.

## FixedMathSharp Dependency Handling

The `*.fixedmathsharp*` modules include an editor-side dependency installer
that attempts to add the matching `FixedMathSharp-Unity` Git dependency for
you. They still require the matching SwiftCollections base package listed
above.

If Unity does not resolve the FixedMathSharp dependency cleanly, use the
matching install URL below or run the package repair menu item under
`Tools > mrdav30`.

- Standard dependency:
  `https://github.com/mrdav30/FixedMathSharp-Unity.git?path=/com.mrdav30.fixedmathsharp`
- Lean dependency:
  `https://github.com/mrdav30/FixedMathSharp-Unity.git?path=/com.mrdav30.fixedmathsharp.lean`

## Notes

- All packages in this repo target Unity `2022.3+`.
- The underlying .NET library lives here:
  [SwiftCollections](https://github.com/mrdav30/SwiftCollections)
- Each package folder keeps a short, package-specific install README.
