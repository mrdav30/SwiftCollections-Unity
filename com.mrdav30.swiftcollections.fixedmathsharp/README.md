# SwiftCollections for Unity — FixedMathSharp companion

Optional fixed-point query integration for the Standard SwiftCollections base.
It adds `FixedBoundVolume`, fixed query structures, and
`Bounds.ToFixedBoundVolume()`.

Requires Unity 2022.3 or newer.

[Documentation](https://mrdav30.github.io/SwiftCollections-Unity/) ·
[Pooling and queries](https://mrdav30.github.io/SwiftCollections-Unity/guides/pooling-and-queries.html) ·
[Changelog](CHANGELOG.md)

## Required pairing

Install the Standard base first:

```text
https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections#v7.0.0
```

Then install this companion:

```text
https://github.com/mrdav30/SwiftCollections-Unity.git?path=/com.mrdav30.swiftcollections.fixedmathsharp#v7.0.0
```

The companion's editor installer attempts to add the matching
`com.mrdav30.fixedmathsharp` package at `v7.0.0`. It does not install the
SwiftCollections base.

If the FixedMathSharp dependency needs repair, run:

```text
Tools → mrdav30 → Repair com.mrdav30.swiftcollections.fixedmathsharp Dependencies
```

In a custom asmdef, reference `SwiftCollections.Runtime`,
`SwiftCollections.FixedMathSharp.Runtime`, and `FixedMathSharp.Runtime` for code
that consumes all three surfaces.

## Convert a bound

```csharp
using SwiftCollections;
using SwiftCollections.Query;
using UnityEngine;

Bounds unityBounds = new(Vector3.zero, new Vector3(2f, 2f, 2f));
FixedBoundVolume queryBounds = unityBounds.ToFixedBoundVolume();
```

Do not pair this package with the Lean base or Lean FixedMathSharp package.
Companion packages do not include a separate Unity sample.
