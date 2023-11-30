# Loading .NET assemblies

.NET provide already APIs to load additional assemblies via
[`AssemblyLoadContext`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.loader.assemblyloadcontext).
Yarhl provides extensions methods for `AssemblyLoadContext` to facilitate
loading files from disk.

You can use the main `AssemblyLoadContext` from `AssemblyLoadContext.Default` to
load them. For advanced use cases, it's possible to create a new
`AssemblyLoadContext` that would provide isolation.

> [!TIP]  
> If you plan to use [`ConverterLocator`](./locate-types.md#converterlocator),
> remember to call `ScanAssemblies` after loading new assemblies.

<!-- ignore markdown warning -->

> [!WARNING]  
> Loading a .NET assembly may load also its required dependencies. You may run
> into dependency issues if they use different versions of a base library such
> as Yarhl or Newtonsoft.Json.

<!-- ignore markdown warning -->

> [!IMPORTANT]  
> There may a security risk by loading **untrusted** assemblies from a file or a
> directory. .NET does provide any security feature to validate it's not
> malicious code.

## Load from file paths

The method
[`TryLoadFromAssemblyPath`](<xref:Yarhl.Plugins.AssemblyLoadContextExtensions.TryLoadFromAssemblyPath(System.Runtime.Loader.AssemblyLoadContext,System.String)>)
will try to load the .NET assembly in the given path. If this assembly fails to
load (e.g. it's not a .NET binary) it will return `null`.

Similar, the method
[`TryLoadFromAssembliesPath`](<xref:Yarhl.Plugins.AssemblyLoadContextExtensions.TryLoadFromAssembliesPath(System.Runtime.Loader.AssemblyLoadContext,System.Collections.Generic.IEnumerable{System.String})>)
will try to load every assembly in the list of paths given. If any of them fails
to load, no exception will be raised and it would be skipped.

Additionally, this API will skip any file where its name starts with any of the
following prefixes. The goal is to prevent loading unwanted dependencies. If you
want to force loading them, use `TryLoadFromAssemblyPath`.

- `System.`
- `Microsoft.`
- `netstandard`
- `nuget`
- `nunit`
- `testhost`

## Load from a directory

The method
[`TryLoadFromDirectory`](<xref:Yarhl.Plugins.AssemblyLoadContextExtensions.TryLoadFromDirectory(System.Runtime.Loader.AssemblyLoadContext,System.String,System.Boolean)>)
will try to load every file in the given directory with an extension `.dll` or
`.exe`. If any of them fails, no error will be reported and it would be skipped.

Via an argument it's possible to configure if it should load files from the
given directory or from its subdirectories recursively as well.

## Load from executing directory

A common use case it's to load every assembly from the executable directory.
Because .NET will load an assembly lazily, only when type actually need it, upon
startup not every assembly from the executable directory could be loaded.

The method
[`TryLoadFromBaseLoadDirectory`](<xref:Yarhl.Plugins.AssemblyLoadContextExtensions.TryLoadFromBaseLoadDirectory(System.Runtime.Loader.AssemblyLoadContext)>)
addresses this use case by loading every `.dll` and `.exe` from the current
`AppDomain.CurrentDomain.BaseDirectory`.

> [!TIP]  
> To use _plugins_ in a _controlled way_, the application may add a set of
> `PackageReference`s. After running `dotnet publish` these dependencies will be
> copied to the output directory. At startup call
> `AssemblyLoadContext.Default.TryLoadFromBaseLoadDirectory` to load all of
> them. Otherwise, unless the application also references their types, the
> assemblies will not be loaded.

<!-- ignore warning -->

> [!NOTE]  
> It does not use `Environment.ProcessPath` because sometimes the application
> (or tests) may run by passing the main library file to the `dotnet` host
> application (e.g. `dotnet MyApp.dll`). In that case it would scan the
> installation path of the .NET SDK instead of the application installation
> directory.
