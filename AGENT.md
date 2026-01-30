# OXM-Library — Agent Guide

This file gives AI agents and contributors context and a prioritized plan for improving the library.

## Project identity

- **What it is**: Object-to-XML mapping library and XSD/WSDL → C#/VB code generator. Runtime parses/serializes XML via LINQ to XML and reflection (no XmlSerializer). GUI (OxmStylizer) and plug-ins (e.g. WSDL) extend the generator.
- **Target**: .NET 8. SDK-style projects; solution: `OxmLibrary.sln`.
- **Key projects**:
  - **OxmLibrary.Runtime** — Core: `ElementBase`, `ElementBaseWriter`, parsing/writing, `TypeHandling`, attributes. Consumable as a library.
  - **OxmLibrary.CodeGeneration** — XSD/inference → class code; uses System.CodeDom; depends on Runtime.
  - **OxmLibrary.GUI** — WinForms app (OxmStylizer) for XSD/XML and generator config.
  - **OxmLibrary.WSDLServiceExtensions** — WSDL import and code gen.
  - **OxmLibrary.WSDLRuntime** — WCF client runtime (contract serializer, message inspector).
  - **GlassButton** — WinForms control used by GUI.

## Repo layout

```
OxmLibrary.sln          # Main solution
Directory.Build.props   # Shared MSBuild (LangVersion, Nullable, etc.)
nuget.config            # nuget.org only (override if you need other feeds)
global.json             # SDK 8.0
OxmLibrary/             # Runtime + GlassButton
OxmLibrary.CodeGeneration/
OxmLibrary.GUI/
OxmLibraryServiceExtensions/
OxmWSDLPluginRuntime/
```

## Improvement plan (prioritized)

### Phase 1 — Quality & shipping (do first)

1. **Tests**
   - Add `OxmLibrary.Tests` (xUnit or NUnit, .NET 8) referencing OxmLibrary.Runtime.
   - Cover: `ElementBase.MapToPackage` (string, XElement, XmlReader), `ElementBaseWriter` / `ToString`, `TypeHandling` parsing, attributes (`OxmXmlAttributeAttribute`, `OxmRegexValidatorAttribute`).
   - Add tests for code generation where feasible (e.g. round-trip: XSD → generated C# → parse sample XML).

2. **Documentation & packaging**
   - Enable XML doc generation on **OxmLibrary.Runtime** (`<GenerateDocumentationFile>true</GenerateDocumentationFile>`, fix CS1591 where needed or keep NoWarn for now).
   - Add package metadata to Runtime for potential NuGet: `PackageId`, `PackageVersion`, `PackageReadmeFile`, `PackageLicenseExpression` (see LICENSE).

3. **.gitignore**
   - Ignore `*.user`, `*.suo`, `*.userosscache`, `*.sln.docstates`, `[Oo]bj/`, `[Bb]in/`, `.vs/`, `packages/`, `*.pfx` (or keep pfx if intentional), IDE/OS cruft.

### Phase 2 — Code health

4. **Nullable reference types**
   - Consider enabling nullable in Directory.Build.props (or per-project) and annotating public APIs of OxmLibrary.Runtime first; fix warnings incrementally.

5. **Analyzers**
   - Add Microsoft.CodeAnalysis.NetAnalyzers (or BuiltInAnalyzers) and optionally StyleCop / EditorConfig for consistent style.

6. **Code cleanup**
   - Address the TODO in `CodeTemplatesBase.cs` (refactor into class descriptor).
   - Reduce `#region` usage where it doesn’t add value; normalize naming (e.g. `IName` → `elementName` where it’s a parameter).

### Phase 3 — Features & polish

7. **README & docs**
   - README: quick example (XSD → generate → parse XML), link to AGENT.md and (if added) CONTRIBUTING.md.
   - Optionally: add XML doc for public APIs of ElementBase, ElementBaseWriter, TypeHandling.

8. **API surface**
   - Consider async overloads for large XML (e.g. `MapToPackageAsync`) if needed; keep sync API as default.

9. **GUI & WSDL**
   - Improve error handling and user feedback in OxmLibrary.GUI; ensure WSDL plug-in works with .NET 8 WCF client stack.

## Conventions for agents

- **Build**: `dotnet build OxmLibrary.sln`. Run GUI: `dotnet run --project OxmLibrary.GUI`.
- **Branching**: Prefer small, focused changes; keep main buildable.
- **C#**: Follow existing style (nullable disabled for now; LangVersion latest). Prefer clear names and minimal regions.
- **Tests**: Place in `OxmLibrary.Tests`; run with `dotnet test`.
- **Docs**: When adding public APIs, add XML comments; keep AGENT.md and README in sync with new projects or major behavior changes.

## Current status

- Solution is on .NET 8; Runtime, GlassButton, WSDLRuntime build. CodeGeneration and WSDLServiceExtensions need NuGet restore (System.CodeDom, WCF, etc.) — ensure network/NuGet config if restore fails.
- **OxmLibrary.Tests** added (xUnit, net8.0): TypeHandling and ElementBase round-trip tests. Run with `dotnet test` after `dotnet restore`.
- OxmLibrary.Runtime: XML docs enabled; PackageId, Apache-2.0, RepositoryType set. .gitignore updated.
- README: build instructions, tests, AGENT link; feature list and Example Page can be expanded. “Example Page” can be expanded.

---

*Update this file when completing plan items or when architecture or priorities change.*
