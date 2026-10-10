# XML documentation

Prism library projects generate XML documentation and treat documentation compiler
diagnostics as errors through `src/Directory.Build.props`. This includes missing
public API comments, malformed XML, invalid or ambiguous `cref` references,
misplaced comments, invalid include files, and incorrect parameter/type parameter
tags. Keep these diagnostics out of project `NoWarn` lists.

`src/Directory.Build.targets` reapplies the policy immediately before compilation
and removes documentation diagnostics from `NoWarn`, including numeric forms.
This is necessary because SDK imports can add their own suppressions after Prism's
props are evaluated: .NET 10 Android adds CS1573 and CS1591 for XML output.
Unrelated suppressions remain intact.

Follow Microsoft's [recommended XML documentation tags](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/recommended-tags):

- Describe public types and members with meaningful summaries and complete sentences.
- Describe parameters, generic type parameters, and return values in terms of the
  API's contract. Parameter names must match the declaration.
- Include remarks for defaults, lifecycle ordering, failure conditions, and other
  behavior callers need to understand. Use examples for common workflows.
- Use `cref` for resolvable code references, `paramref`/`typeparamref` for arguments,
  and `langword` for language keywords. Escape XML characters inside code examples.
- Use explicit `inheritdoc` on implementations and overrides when the inherited
  contract accurately describes the member. Document changed behavior explicitly.

The compiler checks comment presence and XML/reference validity. It does not judge
the usefulness of prose or require every possible documentation tag; those remain
part of code review. See Microsoft's [warning/error configuration](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/errors-warnings)
and [CS1591](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/cs1591).

## Regression check

After restoring dependencies, run:

```powershell
dotnet restore src/Prism.Events/Prism.Events.csproj
./build/Test-XmlDocumentationPolicy.ps1
```

The check first builds the real Prism.Events project using its normal build
imports. It adds temporary source files to demonstrate that missing documentation
fails with **CS1591**, malformed XML fails with **CS1570**, and incomplete parameter
documentation fails with **CS1573**. It repeats the probes with a late-imported
`NoWarn` setting to reproduce SDK suppressions, verifies unrelated suppressions
remain intact, removes temporary files in a `finally` block, and verifies the
project builds again. It does not create or
commit a smoke application, and it does not pass a separate warnings-as-errors
setting that could mask a broken repository policy. Run it in an isolated checkout
without another build of Prism.Events running concurrently.

The Core pull request workflow runs this check. Each library workflow also watches
`src/Directory.Build.props` and `src/Directory.Build.targets`, so changes to the
shared documentation policy trigger the relevant builds.
