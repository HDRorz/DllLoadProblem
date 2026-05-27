#解决方案

这是 .NET Framework 和 .NET 8.0 在程序集加载机制上的根本差异导致的。

.NET Framework 的加载行为： 使用 Assembly.LoadFile() 时，CLR 会在多个位置探测依赖程序集，包括应用程序基目录（bin 文件夹）。所以只要 DLL 放在发布目录下，即使没有显式声明依赖，运行时也能找到并加载它。

.NET 8.0（.NET Core）的加载行为： .NET Core 使用了完全不同的程序集加载体系 — AssemblyLoadContext。默认的加载逻辑依赖 deps.json 文件来解析程序集依赖关系。Assembly.LoadFile() 在 .NET Core 中会将程序集加载到一个独立的 AssemblyLoadContext 中，而且它不会自动探测应用目录下的其他 DLL。

所以核心原因是：A.dll 内部通过 Assembly.LoadFile() 加载 B.dll 时，.NET 8.0 的 AssemblyLoadContext 不知道去哪里找这个程序集，因为它既不在 deps.json 中声明，也不在默认的探测路径中。

解决方案有几种：

最简单的方式 — 在项目中显式引用 B.dll，让它出现在 deps.json 中：

最直接的修复方式是在 csproj 中加上这个引用：

<Reference Include="B">
  <HintPath>..\DllRef\B.dll</HintPath>
</Reference>
这样构建时它会被写入 deps.json，运行时就能正常解析了。

如果你不想改项目引用（比如这类隐式依赖很多），另一种方式是注册自定义的程序集解析逻辑，在 Program.cs 启动时加上：
System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += (context, assemblyName) =>
{
    var dllPath = Path.Combine(AppContext.BaseDirectory, $"{assemblyName.Name}.dll");
    if (File.Exists(dllPath))
        return context.LoadFromAssemblyPath(dllPath);
    return null;
};
这相当于恢复了 .NET Framework 时代"在应用目录下探测 DLL"的行为。
