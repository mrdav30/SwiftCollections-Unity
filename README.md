# SwiftCollections-Unity

Unity package host for SwiftCollections.

This repository contains four installable Unity Package Manager variants. Choose
one package only. The variants overlap and are not meant to be installed
together.

## Which Package Should I Use?

| Package | Use it when | Install |
| --- | --- | --- |
| `com.mrdav30.swiftcollections` | You want the default SwiftCollections package for standard Unity and `Bounds` workflows. | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections` |
| `com.mrdav30.swiftcollections.lean` | You want the same core package without `MemoryPack`. Prefer this for Burst AOT or your own serialization stack. | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.lean` |
| `com.mrdav30.swiftcollections.fixedmathsharp` | You use `FixedMathSharp` and want SwiftCollections plus fixed-point query helpers such as `FixedBoundVolume` and `Bounds.ToFixedBoundVolume()`. | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp` |
| `com.mrdav30.swiftcollections.fixedmathsharp.lean` | You use `FixedMathSharp` and want the no-`MemoryPack` variant for Burst-oriented or custom-serialization setups. | `https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp.lean` |

## How The Variants Differ

`Lean` variants:

- Omit `MemoryPack`.
- Prefer these when Burst AOT compatibility or a custom serialization layer is
  more important than the default serialization path.

`FixedMathSharp` variants:

- Are alternative SwiftCollections package choices, not add-on packages to stack
  with the non-FixedMathSharp variants.
- Add fixed-point query and bounds interop for projects that use
  `FixedMathSharp`.
- Use a non-FixedMathSharp variant if you only need the regular Unity
  `Bounds.ToBoundVolume()` helpers.

## FixedMathSharp Dependency Handling

The `*.fixedmathsharp*` packages include an editor-side dependency installer
that attempts to add the matching `FixedMathSharp-Unity` Git dependency for
you.

If Unity does not resolve that dependency cleanly, use the matching install URL
below or run the package repair menu item under `Tools > mrdav30`.

- Standard dependency:
  `https://github.com/mrdav30/FixedMathSharp-Unity.git?path=/com.mrdav30.fixedmathsharp`
- Lean dependency:
  `https://github.com/mrdav30/FixedMathSharp-Unity.git?path=/com.mrdav30.fixedmathsharp.lean`

## Notes

- All packages in this repo target Unity `2022.3+`.
- The underlying .NET library lives here:
  [SwiftCollections](https://github.com/mrdav30/SwiftCollections)
- Each package folder keeps a short, package-specific install README.
