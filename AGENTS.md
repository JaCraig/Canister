# AGENTS

## Scope

This repository centers on the `Canister.IoC` library. Keep changes targeted, preserve public APIs unless the task explicitly requires a breaking change, and prefer updating existing patterns over introducing new abstractions.

## Start Here

- Project overview and usage: [README.md](README.md)
- Contribution rules and repo-specific coding expectations: [CONTRIBUTING.md](CONTRIBUTING.md)
- Formatting and naming conventions: [.editorconfig](.editorconfig)
- Documentation site source: [docfx_project/index.md](docfx_project/index.md)

## Repository Map

- `Canister.IoC/`: publishable library package and the primary change surface.
- `Canister.Tests/`: xUnit coverage for the library.
- `Canister.Benchmarks/`: BenchmarkDotNet performance checks.
- `Canister.IoC.Example/`, `TestApp/`, `SimpleMVCTests/`: sample and integration-style usage surfaces.
- `.github/workflows/`: centralized CI entrypoints for test, publish, docs, CodeQL, and dependency review.

## Canonical Commands

- Initial tool setup: `setup.bat`
- Restore/build solution: `dotnet build Canister.sln`
- Run unit tests: `dotnet test Canister.Tests/Canister.Tests.csproj`
- Run benchmarks: `dotnet run -c Release --project Canister.Benchmarks/Canister.Benchmarks.csproj`
- Build docs when DocFX is installed: `docfx build docfx_project/docfx.json`

## Working Rules

- Treat `Canister.IoC` as the source of truth; examples and apps should reflect the library API, not diverge from it.
- Preserve the library target frameworks unless the task explicitly changes support: `net8.0`, `net9.0`, `net10.0`.
- Add or update xUnit tests for behavior changes in the library. The main test project targets `net10.0`.
- Keep XML documentation on public APIs. The package enables documentation generation and DocFX consumes those comments.
- Follow the contribution guide even when it differs from default .NET habits: minimize dependencies, prefer IoC-friendly constructors, aim for thread-safe behavior where practical, and keep async method names suffixed with `Async`.
- The repository prefers graceful null handling over reflexively throwing for every null input. Match the surrounding API contract before introducing stricter argument validation.
- Follow `.editorconfig`: 4-space indentation, CRLF line endings, Allman braces, `_fieldName` private fields, PascalCase locals, no `this.` qualification.

## Known Pitfalls

- `AddCanisterModules()` can scan all assemblies in the entry assembly directory. For production-oriented examples or docs, prefer explicitly registering assemblies as noted in [README.md](README.md).
- `Canister.IoC.csproj` packs on build, so avoid changing packaging metadata casually.
- Public API changes should usually be reflected in tests and documentation together.
- CI is wired through reusable workflows in [.github/workflows/dotnet-test.yml](.github/workflows/dotnet-test.yml) and [.github/workflows/dotnet-publish.yml](.github/workflows/dotnet-publish.yml); use those expectations when choosing validation commands.

## Useful Anchors

- Core extension methods live under `Canister.IoC/ExtensionMethods/`.
- Module abstractions live under `Canister.IoC/Interfaces/` and `Canister.IoC/Modules/`.
- Packaging and analyzer settings live in [Canister.IoC/Canister.IoC.csproj](Canister.IoC/Canister.IoC.csproj).
- Test package versions and coverage tooling live in [Canister.Tests/Canister.Tests.csproj](Canister.Tests/Canister.Tests.csproj).

## Validation Expectations

- For library code changes, prefer running the narrowest relevant `dotnet test` first, then `dotnet build Canister.sln` if the change has broader impact.
- For documentation-only customization changes, validate links and keep guidance concise rather than duplicating README or CONTRIBUTING content.
