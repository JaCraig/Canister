# Deep Analysis Plan

## Scope

- Target: full repository, with emphasis on `Canister.IoC`, sample applications, tests, and workflow/configuration surfaces.
- Depth: deep.
- Focus: reliability, security, maintainability, DX, dependency hygiene, and automation coverage.

## Assumptions

- The reusable workflows under `.github/workflows/` may perform broader build steps internally, but the visible repository-level configuration only names `Canister.Tests` explicitly for test execution.
- Directory-wide assembly scan in `AddDefaultAssemblies()` is intentional behavior and not a target for near-term change.
- `SimpleMVCTests` has been removed from the repository and is no longer part of the maintained sample set.
- CI/sample-surface expansion is intentionally deferred for now.

## Architectural Map

- `Canister.IoC/`: core library. Module discovery, assembly scanning, registration helpers, and diagnostics are here.
- `Canister.Tests/`: xUnit tests for the core library. Current suite validates the main registration and logging paths and passes.
- `Canister.Benchmarks/`: BenchmarkDotNet project for performance comparisons.
- `Canister.IoC.Example/`, `TestApp/`: usage samples and compatibility surfaces.
- `docfx_project/`: documentation site, with the landing page including the repository README.
- `.github/workflows/`: thin entrypoints into centralized CI/CD workflows for tests, publish, docs, dependency review, and CodeQL.

## Findings Summary

### 1. Accepted behavior: default assembly discovery scans the application directory

- Status: accepted by maintainer as expected default behavior. No code change planned.
- Evidence: `CanisterConfiguration.AddDefaultAssemblies()` calls `Directory.EnumerateFiles(..., "*.dll")` and `Assembly.LoadFrom(...)` in [Canister.IoC/Utils/CanisterConfiguration.cs](Canister.IoC/Utils/CanisterConfiguration.cs#L238), and current repository direction is to keep this behavior.

### 2. Resolved: legacy MVC sample removed

- Status: completed by maintainer. `SimpleMVCTests` is no longer present.
- Follow-up: ensure any remaining docs or automation references are cleaned up as needed.

### 3. Fixed: partial type-load failures preserve loadable types and log loader exceptions

- Evidence: [Canister.IoC/Utils/CanisterConfiguration.cs](Canister.IoC/Utils/CanisterConfiguration.cs#L105) now routes assembly discovery through `GetTypesFromAssembly`, preserving loadable types from `ReflectionTypeLoadException.Types` and logging each loader exception at warning level. Regression coverage was added in [Canister.Tests/Utils/CanisterConfigurationTests.cs](Canister.Tests/Utils/CanisterConfigurationTests.cs#L121).
- Impact reduction: partially loadable assemblies no longer lose all registrations silently, and failures emit diagnostic logs.

### 4. Fixed: configuration cache is now concurrency-safe

- Evidence: [Canister.IoC/ExtensionMethods/ServiceCollectionExtensions.Setup.cs](Canister.IoC/ExtensionMethods/ServiceCollectionExtensions.Setup.cs#L24) now uses `ConcurrentDictionary<IServiceCollection, CanisterConfiguration>`, [Canister.IoC/ExtensionMethods/ServiceCollectionExtensions.Setup.cs](Canister.IoC/ExtensionMethods/ServiceCollectionExtensions.Setup.cs#L109) uses `TryRemove`, and [Canister.IoC/ExtensionMethods/ServiceCollectionExtensions.Setup.cs](Canister.IoC/ExtensionMethods/ServiceCollectionExtensions.Setup.cs#L123) uses `GetOrAdd`.
- Impact reduction: concurrent access no longer depends on an unsynchronized `Dictionary` read path.

### 5. Fixed: `setup.bat` is idempotent and non-destructive

- Evidence: [setup.bat](setup.bat#L1) now runs `dotnet tool restore` and no longer runs `dotnet new tool-manifest` or `docfx init --quiet`.
- Impact reduction: repeated setup runs should no longer recreate scaffolding or fail due to already-existing tool manifest state.

### 6. Deferred: CI/sample-surface expansion

- Status: explicitly deferred for now by maintainer.

## Quick Wins (1-2 days)

1. Add explicit tests for the partial-load path and for the preferred explicit-assembly registration path.
2. Add concurrency-focused stress coverage for repeated registration calls.

### Completed Quick Wins

1. `setup.bat` idempotency update completed.
2. `SimpleMVCTests` removal completed by maintainer.
3. configuration cache concurrency fix completed.
4. `ReflectionTypeLoadException` handling and regression test coverage completed.
5. concurrency stress regression coverage for `AddCanisterModules()` completed.

## Strategic Improvements

1. Improve diagnostics and resilience for partial type-load scenarios.
2. Keep sample/documentation/automation surfaces aligned as the maintained sample set evolves.

### Strategic Notes Updated

1. Keep current default assembly scan behavior unless maintainer direction changes.
2. Keep CI/sample-surface expansion deferred until explicitly prioritized.

## Actionable Tasks

| Priority | Task                                                      | Owner                  | Timeline  | Validation                                                                             |
| -------- | --------------------------------------------------------- | ---------------------- | --------- | -------------------------------------------------------------------------------------- |
| P2       | Add concurrency regression coverage for config cache path | Library maintainer     | Completed | Added stress test and validated via `dotnet test Canister.Tests/Canister.Tests.csproj` |
| P2       | Verify setup script behavior across repeated runs         | Maintainer or DX owner | Completed | `setup.bat` ran successfully twice consecutively                                       |
| P3       | CI/sample-surface expansion                               | Maintainer             | Deferred  | Deferred by maintainer                                                                 |

## Local Validation Performed

- `dotnet test Canister.Tests/Canister.Tests.csproj`: passed, 87/87 tests.
- `setup.bat` (run 1): succeeded.
- `setup.bat` (run 2): succeeded.
- `dotnet test Canister.Tests/Canister.Tests.csproj` after partial type-load fix: passed, 88/88 tests.
- `dotnet test Canister.Tests/Canister.Tests.csproj` after adding concurrency regression tests: passed, 89/89 tests.

## Update Validation Needed

- No additional immediate validation required for findings 3-5; all requested fixes and related tests are now implemented and passing.
