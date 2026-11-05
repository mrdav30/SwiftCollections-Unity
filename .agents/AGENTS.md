# AGENTS.md

## Repo Scope

- The actual Git repo root is `SwiftCollections-Unity/Assets/Packages`. The
  outer Unity project is a local host for authoring and validation.
- All four manifests require Unity 6.3 LTS (`6000.3`) or newer.
- Both base variants bundle the released .NET Standard 2.1 SwiftCollections
  v8.0.0 assets with Chronicler v1.0.0, including `Chronicler.Diagnostics`.
  Use `Chronicler.Diagnostics.ChronicleGuard` for shared argument guards.
- Current package version is `8.0.0`, tracked in
  `.assets/unity-package-versions.json` and each package `package.json`.
- This repo hosts Unity Package Manager wrappers for the upstream
  SwiftCollections library. Most core data-structure behavior lives in the
  precompiled `SwiftCollections.dll`, not in Unity-side source.
- Prefer optimized, low time-complexity code. Avoid band-aid fixes that hide a
  design or data-structure problem.

## Package Matrix

- `com.mrdav30.swiftcollections` is the standard base package. It includes the
  MemoryPack-enabled SwiftCollections plugin and Unity helpers such as
  `Bounds.ToBoundVolume()`.
- `com.mrdav30.swiftcollections.lean` is the no-MemoryPack base package. Do not
  install it alongside the standard base package.
- `com.mrdav30.swiftcollections.fixedmathsharp` is the FixedMathSharp companion
  for the standard base package.
- `com.mrdav30.swiftcollections.fixedmathsharp.lean` is the FixedMathSharp
  companion for the lean base package.
- FixedMathSharp companions require the matching SwiftCollections base package
  and the matching FixedMathSharp-Unity package. The current dependency version
  is `v8.0.0`.

## Source Layout

- `Build/Base` is the shared source overlay for both base package variants.
- `Build/FixedMathSharp` is the shared source overlay for both FixedMathSharp
  companion variants.
- Each overlay owns its package-family `CHANGELOG.md`; the sync copies it into
  the matching Standard and Lean variants.
- `Build/Editor/SwiftCollectionsPackageSync.cs` syncs managed overlay files into
  the package folders.
- `Build/Editor/SwiftCollectionsUnityPackageExporter.cs` exports all four
  package folders after syncing overlays and regenerates base package
  `Samples~/` content from local `Samples/` mirrors.
- `com.mrdav30.swiftcollections*/Plugins` contains precompiled upstream
  assemblies, PDBs, and XML docs.
- `Runtime/` files in package folders are Unity-side wrapper code. Keep them
  thin; changes to core collections usually belong in
  the sibling `SwiftCollections` repository.
- `Editor/Utility/GitDependencyInstaller.cs` exists only in FixedMathSharp
  companion packages and manages the FixedMathSharp Unity Git dependency.
- `Tests/EditMode` contains base-package Unity EditMode tests.
- `Tests/EditMode.FixedMathSharp` contains FixedMathSharp companion EditMode
  tests.

## Unity Sample Authoring And Export Layout

- Base package samples use two folder shapes: `Samples/` for local authoring and
  `Samples~/` for distributable Git/package-source installs.
- Package manifests should point sample entries at `Samples~/...`. Unity hides
  tilde folders in the Project pane, so edit sample scenes, prefabs, resources,
  and scripts through the local `Samples/` mirror.
- Package-local `.gitignore` files ignore `Samples/`, `Samples.meta`, and
  `Samples~.meta`. Do not commit package-variant `Samples/` folders or top-level
  `Samples~.meta` files.
- Keep nested `.meta` files inside `Samples~/SwiftCollectionsDemo` and local
  `Samples/SwiftCollectionsDemo`. They preserve scene, prefab, resource, asmdef,
  and script references when samples import into `Assets/Samples/...`.
- Shared sample scripts live in
  `Build/Base/Samples/SwiftCollectionsDemo/Scripts`. The sync step hydrates
  missing package-local `Samples/` mirrors from tracked `Samples~/`, then copies
  shared scripts into the visible authoring mirror.
- The exporter overwrites each base package `Samples~/` folder from its local
  `Samples/` mirror and excludes `Samples/` from `.unitypackage` exports.
  FixedMathSharp companion packages do not currently publish samples.

## Coding Expectations

- Prefer SwiftCollections types and helpers over BCL collections when a suitable
  SwiftCollections type exists.
- Do not introduce `List<>`, `Dictionary<>`, `HashSet<>`, `Stack<>`, or similar
  hot-path BCL collections in package runtime code without a clear reason.
- Keep checkout, release, lookup, and query paths at their intended complexity.
  Do not replace O(1) operations with scans.
- Preserve package variant boundaries. Standard code may reference
  MemoryPack-enabled plugin dependencies; lean code must stay no-MemoryPack.
- Preserve Unity package structure and tracked `.meta` files when moving or
  adding committed assets, asmdefs, tests, prefabs, or package content.
- Do not commit generated Unity `Library`, `Temp`, `.ci/unity-project`, or
  export output artifacts.

## GameObjectPool Guidance

- Treat the GameObject pool as a reusable pool, not a round-robin allocator that
  steals live instances.
- Pool checkout and return paths should stay O(1). Avoid scans over all created
  objects in hot paths.
- Preserve explicit return-to-pool semantics. Checked-out objects are expected
  to be released back to the pool.
- `SwiftGameObjectPoolManager.Shared` loads `Resources/SwiftGameObjectPoolAsset`
  by name.
- The shared pool root is intended to survive scene loads, so be careful with
  lifetime and disposal changes.

## Dependency Guidance

- FixedMathSharp companion packages install either `com.mrdav30.fixedmathsharp`
  or `com.mrdav30.fixedmathsharp.lean` via Git URL.
- The PowerShell export entry point runs the version updater with the selected
  host project before launching Unity. It updates only configured dependencies
  already installed in that project's manifest; `-WhatIf` previews without writes.
- Keep companion installer session keys scoped to their package IDs so both
  checks can run in the authoring host.
- If dependency behavior changes, update the package installer,
  `.assets/unity-package-versions.json`, package READMEs, the root README, and
  the CI workflow matrix together.
- Test asmdefs use `overrideReferences`, so they must explicitly list
  precompiled plugin DLL dependencies. Standard tests need MemoryPack-related
  DLLs; lean tests omit MemoryPack and `System.Collections.Immutable`.

## Verification

- Primary CI is `.github/workflows/build-and-test.yml`.
- The workflow creates a temporary Unity project, installs one package variant
  per matrix row, and runs `SwiftCollections.Unity.Tests.EditMode` with
  `game-ci/unity-test-runner@v4`.
- Base package rows copy `Tests/EditMode/*.cs`; FixedMathSharp rows copy
  `Tests/EditMode.FixedMathSharp/*.cs`.
- The Unity test matrix covers standard, lean, FixedMathSharp, and
  FixedMathSharp lean packages on Unity `6000.3.9f1` and Ubuntu 24.04.
- CI activation uses `UNITY_EMAIL` and `UNITY_PASSWORD`. Sample import trusts
  the exact container checkout paths through the runner's global Git config.
- A main-only sample import smoke job installs the standard and lean base
  packages via local Git URL, imports the Demo Scene sample, and scans Unity
  logs for known sample packaging failures.
- Local `dotnet build` is not a substitute for Unity package validation because
  these packages depend on Unity asmdefs, plugin resolution, and Unity-generated
  project state.
- Useful lightweight checks before CI: parse the workflow YAML, parse checked-in
  asmdef JSON, and run `git diff --check`.

## Documentation Expectations

- Keep the root README concise and user-facing: package choice, install URLs,
  dependency pairing, serialization boundaries, and hosted documentation.
- Put operational guidance for agents here rather than bloating the README.
- Generate the Unity API reference only from canonical
  `Build/Base/Runtime/**/*.cs` and `Build/FixedMathSharp/Runtime/**/*.cs`
  through `.docs/api/docfx.json`; do not document synchronized package copies.
- Keep the hosted guides under `.docs/api/guides`, the shared SwiftCollections
  icon under `.assets`, and the custom DocFX template aligned with the root and
  package READMEs. CI should assert these branded assets and cross-repo links.
- Link to the hosted SwiftCollections and FixedMathSharp API references instead
  of republishing their engine-agnostic APIs.
- Restore the pinned DocFX tool with `dotnet tool restore`. CI stages the Unity,
  SwiftCollections, SwiftCollections.FixedMathSharp, Chronicler.Diagnostics, and FixedMathSharp
  reference assemblies under ignored `.docs/api/obj/references` before running
  `dotnet tool run docfx .docs/api/docfx.json`.
- Add concise XML documentation to every public runtime API in the canonical
  Build overlays and sync it into package variants.
- When package layout, dependencies, or CI behavior changes, update README and
  AGENTS in the same pass.
