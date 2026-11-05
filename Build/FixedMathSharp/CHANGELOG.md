# Changelog

Notable changes to the SwiftCollections FixedMathSharp Unity companion packages
from version 7 onward are documented here.

## [8.0.0] - 2026-11-05

### Changed

- Scope dependency-installer session flags to each package so both variants can
  check their dependencies in the authoring host.
- Require Unity 6.3 LTS (6000.3) or newer.
- Bundle released SwiftCollections.FixedMathSharp v8.0.0 .NET Standard 2.1
  assemblies, paired with the matching SwiftCollections v8.0.0 base.
- Update the matching FixedMathSharp-Unity dependency and CI matrix to v8.0.0.
  The Unity bounds adapter API is unchanged.

## [7.0.0] - 2026-08-09

### Added

- Hosted API documentation for Unity conversion to FixedMathSharp-backed
  `SwiftCollections.Query` bounds.

### Changed

- Upgraded the bundled SwiftCollections FixedMathSharp integration from v6 to
  v7.
- Updated the matching FixedMathSharp-Unity dependency to v7.0.0.
- Kept the Standard and Lean companion APIs aligned.
