# Changelog

Notable changes to the SwiftCollections Unity base packages from version 7
onward are documented here.

## [8.0.0] - 2026-11-05

### Changed

- Require Unity 6.3 LTS (6000.3) or newer; CI uses Unity 6000.3.9f1 on Ubuntu
  24.04, email/password activation, and corrected sample-import Git trust.
- Bundle released SwiftCollections v8.0.0 .NET Standard 2.1 assemblies and
  Chronicler v1.0.0 dependencies, including shared diagnostics.
- Migrate pooling argument guards to `Chronicler.Diagnostics.ChronicleGuard`.
- Core slot collections and handles are now `SwiftSlotMap`, `SwiftTicketMap`,
  and `SwiftTicket`; see the repository migration guide before rebuilding
  consumers. Unity authoring adapter and pooling APIs are unchanged.

## [7.0.0] - 2026-08-09

### Added

- Hosted API documentation for the Unity serialization, pooling, and bounds
  adapters.

### Changed

- Upgraded the bundled SwiftCollections library from v6 to v7.
- Kept the Standard and Lean package APIs aligned; the Lean package continues to
  omit the MemoryPack serializer/runtime.
