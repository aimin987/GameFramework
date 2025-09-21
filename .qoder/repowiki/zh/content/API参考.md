
# API参考

<cite>
**本文档引用的文件**   
- [IResourceManager.cs](file://GameFramework/Resource/IResourceManager.cs)
- [IEventManager.cs](file://GameFramework/Event/IEventManager.cs)
- [IObjectPool.cs](file://GameFramework/ObjectPool/IObjectPool.cs)
- [IDataNodeManager.cs](file://GameFramework/DataNode/IDataNodeManager.cs)
- [IDownloadManager.cs](file://GameFramework/Download/IDownloadManager.cs)
- [IFileSystemManager.cs](file://GameFramework/FileSystem/IFileSystemManager.cs)
- [IFsmManager.cs](file://GameFramework/Fsm/IFsmManager.cs)
- [ILocalizationManager.cs](file://GameFramework/Localization/ILocalizationManager.cs)
- [INetworkManager.cs](file://GameFramework/Network/INetworkManager.cs)
- [IObjectPoolManager.cs](file://GameFramework/ObjectPool/IObjectPoolManager.cs)
- [IProcedureManager.cs](file://GameFramework/Procedure/IProcedureManager.cs)
- [ISettingManager.cs](file://GameFramework/Setting/ISettingManager.cs)
- [ISoundManager.cs](file://GameFramework/Sound/ISoundManager.cs)
- [IUIManager.cs](file://GameFramework/UI/IUIManager.cs)
- [IWebRequestManager.cs](file://GameFramework/WebRequest/IWebRequestManager.cs)
- [EventManager.cs](file://GameFramework/Event/EventManager.cs)
- [ObjectPoolManager.cs](file://GameFramework/ObjectPool/ObjectPoolManager.cs)
- [ResourceManager.cs](file://GameFramework/Resource/ResourceManager.cs)
- [DataNodeManager.cs](file://GameFramework/DataNode/DataNodeManager.cs)
- [DownloadManager.cs](file://GameFramework/Download/DownloadManager.cs)
- [FileSystemManager.cs](file://GameFramework/FileSystem/FileSystemManager.cs)
- [FsmManager.cs](file://GameFramework/Fsm/FsmManager.cs)
- [LocalizationManager.cs](file://GameFramework/Localization/LocalizationManager.cs)
- [NetworkManager.cs](file://GameFramework/Network/NetworkManager.cs)
- [ProcedureManager.cs](file://GameFramework/Procedure/ProcedureManager.cs)
- [SettingManager.cs](file://GameFramework/Setting/SettingManager.cs)
- [SoundManager.cs](file://GameFramework/Sound/SoundManager.cs)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs)
- [WebRequestManager.cs](file://GameFramework/WebRequest/WebRequestManager.cs)
</cite>

## 目录
1. [资源模块](#资源模块)
2. [事件模块](#事件模块)
3. [对象池模块](#对象池模块)
4. [数据节点模块](#数据节点模块)
5. [下载模块](#下载模块)
6. [文件系统模块](#文件系统模块)
7. [有限状态机模块](#有限状态机模块)
8. [本地化模块](#本地化模块)
9. [网络模块](#网络模块)
10. [流程模块](#流程模块)
11. [设置模块](#设置模块)
12. [声音模块](#声音模块)
13. [界面模块](#界面模块)
14. [Web请求模块](#web请求模块)

## 资源模块

### IResourceManager 接口

`IResourceManager` 是资源管理器的核心接口，负责管理游戏中的所有资源，包括加载、卸载、更新和校验等操作。

**属性**

- **ReadOnlyPath**: 获取资源只读区路径。
- **ReadWritePath**: 获取资源读写区路径。
- **ResourceMode**: 获取资源模式。
- **CurrentVariant**: 获取当前变体。
- **PackageVersionListSerializer**: 获取单机模式版本资源列表序列化器。
- **UpdatableVersionListSerializer**: 获取可更新模式版本资源列表序列化器。
- **ReadOnlyVersionListSerializer**: 获取本地只读区版本资源列表序列化器。
- **ReadWriteVersionListSerializer**: 获取本地读写区版本资源列表序列化器。
- **ResourcePackVersionListSerializer**: 获取资源包版本资源列表序列化器。
- **ApplicableGameVersion**: 获取当前资源适用的游戏版本号。
- **InternalResourceVersion**: 获取当前内部资源版本号。
- **AssetCount**: 获取资源数量。
- **ResourceCount**: 获取资源数量。
- **ResourceGroupCount**: 获取资源组数量。
- **UpdatePrefixUri**: 获取或设置资源更新下载地址。
- **GenerateReadWriteVersionListLength**: 获取或设置每更新多少字节的资源，重新生成一次版本资源列表。
- **ApplyingResourcePackPath**: 获取正在应用的资源包路径。
- **ApplyWaitingCount**: 获取等待应用资源数量。
- **UpdateRetryCount**: 获取或设置资源更新重试次数。
- **UpdatingResourceGroup**: 获取正在更新的资源组。
- **UpdateWaitingCount**: 获取等待更新资源数量。
- **UpdateWaitingWhilePlayingCount**: 获取使用时下载的等待更新资源数量。
- **UpdateCandidateCount**: 获取候选更新资源数量。
- **LoadTotalAgentCount**: 获取加载资源代理总数量。
- **LoadFreeAgentCount**: 获取可用加载资源代理数量。
- **LoadWorkingAgentCount**: 获取工作中加载资源代理数量。
- **LoadWaitingTaskCount**: 获取等待加载资源任务数量。
- **AssetAutoReleaseInterval**: 获取或设置资源对象池自动释放可释放对象的间隔秒数。
- **AssetCapacity**: 获取或设置资源对象池的容量。
- **AssetExpireTime**: 获取或设置资源对象池对象过期秒数。
- **AssetPriority**: 获取或设置资源对象池的优先级。
- **ResourceAutoReleaseInterval**: 获取或设置资源对象池自动释放可释放对象的间隔秒数。
- **ResourceCapacity**: 获取或设置资源对象池的容量。
- **ResourceExpireTime**: 获取或设置资源对象池对象过期秒数。
- **ResourcePriority**: 获取或设置资源对象池的优先级。

**事件**

- **ResourceVerifyStart**: 资源校验开始事件。
- **ResourceVerifySuccess**: 资源校验成功事件。
- **ResourceVerifyFailure**: 资源校验失败事件。
- **ResourceApplyStart**: 资源应用开始事件。
- **ResourceApplySuccess**: 资源应用成功事件。
- **ResourceApplyFailure**: 资源应用失败事件。
- **ResourceUpdateStart**: 资源更新开始事件。
- **ResourceUpdateChanged**: 资源更新改变事件。
- **ResourceUpdateSuccess**: 资源更新成功事件。
- **ResourceUpdateFailure**: 资源更新失败事件。
- **ResourceUpdateAllComplete**: 资源更新全部完成事件。

**方法**

- **SetReadOnlyPath(string readOnlyPath)**: 设置资源只读区路径。
- **SetReadWritePath(string readWritePath)**: 设置资源读写区路径。
- **SetResourceMode(ResourceMode resourceMode)**: 设置资源模式。
- **SetCurrentVariant(string currentVariant)**: 设置当前变体。
- **SetObjectPoolManager(IObjectPoolManager objectPoolManager)**: 设置对象池管理器。
- **SetFileSystemManager(IFileSystemManager fileSystemManager)**: 设置文件系统管理器。
- **SetDownloadManager(IDownloadManager downloadManager)**: 设置下载管理器。
- **SetDecryptResourceCallback(DecryptResourceCallback decryptResourceCallback)**: 设置解密资源回调函数。
- **SetResourceHelper(IResourceHelper resourceHelper)**: 设置资源辅助器。
- **AddLoadResourceAgentHelper(ILoadResourceAgentHelper loadResourceAgentHelper)**: 增加加载资源代理辅助器。
- **InitResources(InitResourcesCompleteCallback initResourcesCompleteCallback)**: 使用单机模式并初始化资源。
- **CheckVersionList(int latestInternalResourceVersion)**: 使用可更新模式并检查版本资源列表。
- **UpdateVersionList(int versionListLength, int versionListHashCode, int versionListCompressedLength, int versionListCompressedHashCode, UpdateVersionListCallbacks updateVersionListCallbacks)**: 使用可更新模式并更新版本资源列表。
- **VerifyResources(int verifyResourceLengthPerFrame, VerifyResourcesCompleteCallback verifyResourcesCompleteCallback)**: 使用可更新模式并校验资源。
- **CheckResources(bool ignoreOtherVariant, CheckResourcesCompleteCallback checkResourcesCompleteCallback)**: 使用可更新模式并检查资源。
- **ApplyResources(string resourcePackPath, ApplyResourcesCompleteCallback applyResourcesCompleteCallback)**: 使用可更新模式并应用资源包资源。
- **UpdateResources(UpdateResourcesCompleteCallback updateResourcesCompleteCallback)**: 使用可更新模式并更新所有资源。
- **UpdateResources(string resourceGroupName, UpdateResourcesCompleteCallback updateResourcesCompleteCallback)**: 使用可更新模式并更新指定资源组的资源。
- **StopUpdateResources()**: 停止更新资源。
- **VerifyResourcePack(string resourcePackPath)**: 校验资源包。
- **GetAllLoadAssetInfos()**: 获取所有加载资源任务的信息。
- **GetAllLoadAssetInfos(List<TaskInfo> results)**: 获取所有加载资源任务的信息。
- **HasAsset(string assetName)**: 检查资源是否存在。
- **LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks)**: 异步加载资源。
- **LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks)**: 异步加载资源。
- **LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks)**: 异步加载资源。
- **LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData)**: 异步加载资源。
- **LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks)**: 异步加载资源。
- **LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks, object userData)**: 异步加载资源。
- **LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)**: 异步加载资源。
- **LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)**: 异步加载资源。
- **UnloadAsset(object asset)**: 卸载资源。
- **LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks)**: 异步加载场景。
- **LoadScene(string sceneAssetName, int priority, LoadSceneCallbacks loadSceneCallbacks)**: 异步加载场景。
- **LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks, object userData)**: 异步加载场景。
- **LoadScene(string sceneAssetName, int priority, LoadSceneCallbacks loadSceneCallbacks, object userData)**: 异步加载场景。
- **UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks)**: 异步卸载场景。
- **UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)**: 异步卸载场景。
- **GetBinaryPath(string binaryAssetName)**: 获取二进制资源的实际路径。
- **GetBinaryPath(string binaryAssetName, out bool storageInReadOnly, out bool storageInFileSystem, out string relativePath, out string fileName)**: 获取二进制资源的实际路径。
- **GetBinaryLength(string binaryAssetName)**: 获取二进制资源的长度。
- **LoadBinary(string binaryAssetName, LoadBinaryCallbacks loadBinaryCallbacks)**: 异步加载二进制资源。
- **LoadBinary(string binaryAssetName, LoadBinaryCallbacks loadBinaryCallbacks, object userData)**: 异步加载二进制资源。
- **LoadBinaryFromFileSystem(string binaryAssetName)**: 从文件系统中加载二进制资源。
- **LoadBinaryFromFileSystem(string binaryAssetName, byte[] buffer)**: 从文件系统中加载二进制资源。
- **LoadBinaryFromFileSystem(string binaryAssetName, byte[] buffer, int startIndex)**: 从文件系统中加载二进制资源。
- **LoadBinaryFromFileSystem(string binaryAssetName, byte[] buffer, int startIndex, int length)**: 从文件系统中加载二进制资源。
- **LoadBinarySegmentFromFileSystem(string binaryAssetName, int length)**: 从文件系统中加载二进制资源的片段。
- **LoadBinarySegmentFromFileSystem(string binaryAssetName, int offset, int length)**: 从文件系统中加载二进制资源的片段。
- **LoadBinarySegmentFromFileSystem(string binaryAssetName, byte[] buffer)**: 从文件系统中加载二进制资源的片段。
- **LoadBinarySegmentFromFileSystem(string binaryAssetName, byte[] buffer, int length)**: 从文件系统中加载二进制资源的片段。
- **LoadBinarySegmentFromFileSystem(string binaryAssetName, byte[] buffer, int startIndex, int length)**: 从文件系统中加载二进制资源的片段。
- **LoadBinarySegmentFromFileSystem(string binaryAssetName, int offset, byte[] buffer)**: 从文件系统中加载二进制资源的片段。
- **LoadBinarySegmentFromFileSystem(string binaryAssetName, int offset, byte[] buffer, int length)**: 从文件系统中加载二进制资源的片段。
- **LoadBinarySegmentFromFileSystem(string binaryAssetName, int offset, byte[] buffer, int startIndex, int length)**: 从文件系统中加载二进制资源的片段。
- **HasResourceGroup(string resourceGroupName)**: 检查资源组是否存在。
- **GetResourceGroup()**: 获取默认资源组。
- **GetResourceGroup(string resourceGroupName)**: 获取资源组。
- **GetAllResourceGroups()**: 获取所有资源组。
- **GetAllResourceGroups(List<IResourceGroup> results)**: 获取所有资源组。
- **GetResourceGroupCollection(params string[] resourceGroupNames)**: 获取资源组集合。
- **GetResourceGroupCollection(List<string> resourceGroupNames)**: 获取资源组集合。

**Section sources**
- [IResourceManager.cs](file://GameFramework/Resource/IResourceManager.cs#L18-L830)
- [ResourceManager.cs](file://GameFramework/Resource/ResourceManager.cs#L18-L2588)

## 事件模块

### IEventManager 接口

`IEventManager` 是事件管理器的核心接口，负责管理游戏中的所有事件，包括订阅、取消订阅和抛出事件等操作。

**属性**

- **EventHandlerCount**: 获取事件处理函数的数量。
- **EventCount**: 获取事件数量。

**方法**

- **Count(int id)**: 获取指定事件类型编号的事件处理函数的数量。
- **Check(int id, EventHandler<GameEventArgs> handler)**: 检查是否存在指定的事件处理函数。
- **Subscribe(int id, EventHandler<GameEventArgs> handler)**: 订阅事件处理函数。
- **Unsubscribe(int id, EventHandler<GameEventArgs> handler)**: 取消订阅事件处理函数。
- **SetDefaultHandler(EventHandler<GameEventArgs> handler)**: 设置默认事件处理函数。
- **Fire(object sender, GameEventArgs e)**: 抛出事件，这个操作是线程安全的，即使不在主线程中抛出，也可保证在主线程中回调事件处理函数，但事件会在抛出后的下一帧分发。
- **FireNow(object sender, GameEventArgs e)**: 抛出事件立即模式，这个操作不是线程安全的，事件会立刻分发。

**Section sources**
- [IEventManager.cs](file://GameFramework/Event/IEventManager.cs#L14-L80)
- [EventManager.cs](file://GameFramework/Event/EventManager.cs#L14-L150)

## 对象池模块

### IObjectPool<T> 接口

`IObjectPool<T>` 是对象池的核心接口，负责管理特定类型对象的创建、获取、回收和释放等操作。

**属性**

- **Name**: 获取对象池名称。
- **FullName**: 获取对象池完整名称。
- **ObjectType**: 获取对象池对象类型。
- **Count**: 获取对象池中对象的数量。
- **CanReleaseCount**: 获取对象池中能被释放的对象的数量。
- **AllowMultiSpawn**: 获取是否允许对象被多次获取。
- **AutoReleaseInterval