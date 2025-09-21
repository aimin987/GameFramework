# 路径操作工具 (Utility.Path)

<cite>
**本文档引用的文件**  
- [Utility.Path.cs](file://GameFramework/Utility/Utility.Path.cs)
- [ResourceManager.ResourceLoader.LoadResourceAgent.cs](file://GameFramework/Resource/ResourceManager.ResourceLoader.LoadResourceAgent.cs)
- [FileSystemManager.cs](file://GameFramework/FileSystem/FileSystemManager.cs)
- [ResourceManager.ResourceChecker.cs](file://GameFramework/Resource/ResourceManager.ResourceChecker.cs)
- [ResourceManager.ResourceUpdater.cs](file://GameFramework/Resource/ResourceManager.ResourceUpdater.cs)
- [ResourceManager.VersionListProcessor.cs](file://GameFramework/Resource/ResourceManager.VersionListProcessor.cs)
</cite>

## 目录
1. [简介](#简介)
2. [核心功能详解](#核心功能详解)
3. [跨平台路径兼容性处理](#跨平台路径兼容性处理)
4. [实际应用场景](#实际应用场景)
5. [最佳实践与使用建议](#最佳实践与使用建议)
6. [总结](#总结)

## 简介

`Utility.Path` 是 Game Framework 框架中用于处理文件路径的核心工具类，位于 `GameFramework.Utility` 命名空间下。该模块提供了一系列静态方法，用于规范化路径格式、生成远程路径以及清理空目录，确保在不同操作系统（如 Windows、macOS、Linux 及移动端）上的一致性行为。通过统一路径分隔符并标准化路径表示方式，`Utility.Path` 有效避免了因平台差异导致的路径解析错误。

**Section sources**  
- [Utility.Path.cs](file://GameFramework/Utility/Utility.Path.cs#L1-L20)

## 核心功能详解

### 路径规范化（GetRegularPath）

`GetRegularPath` 方法用于将任意格式的路径转换为标准格式，即将反斜杠 `\` 统一替换为正斜杠 `/`。此方法接受一个字符串类型的路径参数，并返回规范化后的路径。若输入路径为 `null`，则返回 `null`。

该方法在资源加载、文件系统访问等场景中被广泛调用，以确保路径格式统一，避免因操作系统默认分隔符不同而导致的问题。

**Section sources**  
- [Utility.Path.cs](file://GameFramework/Utility/Utility.Path.cs#L23-L36)
- [FileSystemManager.cs](file://GameFramework/FileSystem/FileSystemManager.cs#L104-L158)
- [ResourceManager.ResourceLoader.cs](file://GameFramework/Resource/ResourceManager.ResourceLoader.cs#L480)

### 远程路径生成（GetRemotePath）

`GetRemotePath` 方法用于将本地路径转换为可被 Unity 资源系统识别的远程格式路径（如 `file:///` 或 `http://` 开头的 URI）。该方法首先调用 `GetRegularPath` 对路径进行规范化处理，然后判断路径是否已包含协议前缀（`://`），若无则自动添加 `file:///` 前缀，并修正可能出现的多余斜杠（如 `file:////`）。

此功能常用于从本地文件系统加载资源时，确保路径符合 Unity 的资源加载规范。

**Section sources**  
- [Utility.Path.cs](file://GameFramework/Utility/Utility.Path.cs#L38-L44)
- [ResourceManager.ResourceChecker.cs](file://GameFramework/Resource/ResourceManager.ResourceChecker.cs#L82-L84)
- [ResourceManager.ResourceLoader.cs](file://GameFramework/Resource/ResourceManager.ResourceLoader.cs#L607)

### 空目录清理（RemoveEmptyDirectory）

`RemoveEmptyDirectory` 方法用于递归删除指定目录下的所有空子目录。该方法首先检查目录是否存在，然后遍历其子目录并递归调用自身进行清理。只有当目录下既无文件也无非空子目录时，才会执行删除操作。该方法具有良好的异常容错能力，在删除过程中遇到权限问题或其他异常时会返回 `false` 而不会中断程序执行。

该功能在资源更新完成后用于清理不再使用的空目录，保持存储结构整洁。

**Section sources**  
- [Utility.Path.cs](file://GameFramework/Utility/Utility.Path.cs#L54-L77)
- [ResourceManager.ResourceChecker.cs](file://GameFramework/Resource/ResourceManager.ResourceChecker.cs#L219)
- [ResourceManager.cs](file://GameFramework/Resource/ResourceManager.cs#L2551)

## 跨平台路径兼容性处理

`Utility.Path` 模块的核心设计目标之一是实现跨平台路径操作的一致性。在 Windows 系统中，路径分隔符通常为反斜杠 `\`，而在 macOS、Linux 及移动端系统中则使用正斜杠 `/`。直接使用硬编码的分隔符（如 `"folder\\subfolder"` 或 `"folder/subfolder"`）会导致跨平台兼容性问题。

通过 `GetRegularPath` 方法，所有路径均被统一转换为以 `/` 作为分隔符的标准格式，从而消除了平台差异带来的影响。此外，`GetRemotePath` 方法生成的 `file:///` 格式路径也被所有平台的 Unity 引擎所支持，进一步增强了可移植性。

开发者应始终使用 `Utility.Path.GetRegularPath(Path.Combine(...))` 来构建路径，而非手动拼接字符串。

**Section sources**  
- [Utility.Path.cs](file://GameFramework/Utility/Utility.Path.cs#L23-L44)
- [ResourceManager.ResourceLoader.LoadResourceAgent.cs](file://GameFramework/Resource/ResourceManager.ResourceLoader.LoadResourceAgent.cs#L200-L207)

## 实际应用场景

### 资源加载路径构建

在资源管理器中，当需要从只读区或读写区加载资源时，系统会根据资源存储位置动态构建完整路径：

```csharp
string fullPath = Utility.Path.GetRegularPath(
    Path.Combine(
        resourceInfo.StorageInReadOnly ? m_ReadOnlyPath : m_ReadWritePath,
        resourceInfo.UseFileSystem ? resourceInfo.FileSystemName : resourceInfo.ResourceName.FullName
    )
);
```

此模式确保无论运行在哪种操作系统上，路径都能正确解析。

### 版本列表文件路径处理

在资源版本检查与更新流程中，多个关键文件（如远程版本列表、本地版本列表）的路径均通过 `Utility.Path` 进行处理：

```csharp
string localVersionListFilePath = Utility.Path.GetRegularPath(
    Path.Combine(m_ResourceManager.m_ReadWritePath, RemoteVersionListFileName)
);
m_DownloadManager.AddDownload(
    localVersionListFilePath,
    Utility.Path.GetRemotePath(Path.Combine(m_ResourceManager.m_UpdatePrefixUri, latestVersionListFullNameWithCrc32)),
    this
);
```

这保证了下载任务能够正确识别本地缓存路径和远程服务器路径。

### 缓存目录管理

在资源初始化和更新完成后，系统会调用 `RemoveEmptyDirectory` 清理读写路径下的空目录，释放存储空间并维持目录结构的清晰。

**Section sources**  
- [ResourceManager.ResourceChecker.cs](file://GameFramework/Resource/ResourceManager.ResourceChecker.cs#L147-L207)
- [ResourceManager.VersionListProcessor.cs](file://GameFramework/Resource/ResourceManager.VersionListProcessor.cs#L87-L148)
- [ResourceManager.ResourceUpdater.cs](file://GameFramework/Resource/ResourceManager.ResourceUpdater.cs#L81)

## 最佳实践与使用建议

- **禁止硬编码路径分隔符**：永远不要使用 `"folder\\file"` 或 `"folder/file"` 这样的字符串拼接方式。
- **始终使用 Path.Combine**：使用 `System.IO.Path.Combine` 来拼接路径片段，确保平台兼容性。
- **立即规范化路径**：在拼接完成后立即调用 `Utility.Path.GetRegularPath` 进行标准化。
- **远程资源使用 GetRemotePath**：当路径需作为 URI 传递给资源加载系统时，务必使用 `GetRemotePath` 包装。
- **定期清理空目录**：在资源更新或删除操作后，考虑调用 `RemoveEmptyDirectory` 维护目录整洁。

遵循这些原则可显著提升项目的稳定性和可维护性，特别是在多平台发布时。

**Section sources**  
- [Utility.Path.cs](file://GameFramework/Utility/Utility.Path.cs#L23-L77)

## 总结

`Utility.Path` 模块虽小，但在保障 Game Framework 跨平台能力方面发挥着至关重要的作用。通过对路径格式的统一处理、远程路径的标准化生成以及空目录的智能清理，该工具类有效屏蔽了底层操作系统的差异，使开发者能够专注于业务逻辑而非路径兼容性问题。建议所有涉及文件路径操作的代码均通过该工具类进行封装，以确保整个项目的一致性和健壮性。