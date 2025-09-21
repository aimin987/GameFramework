
# UI管理核心系统

<cite>
**本文档中引用的文件**  
- [UIManager.cs](file://GameFramework/UI/UIManager.cs)
- [IUIManager.cs](file://GameFramework/UI/IUIManager.cs)
- [ObjectPoolManager.cs](file://GameFramework/ObjectPool/ObjectPoolManager.cs)
- [ResourceManager.cs](file://GameFramework/Resource/ResourceManager.cs)
- [LoadAssetCallbacks.cs](file://GameFramework/Resource/LoadAssetCallbacks.cs)
- [UIManager.UIFormInstanceObject.cs](file://GameFramework/UI/UIManager.UIFormInstanceObject.cs)
- [UIManager.OpenUIFormInfo.cs](file://GameFramework/UI/UIManager.OpenUIFormInfo.cs)
</cite>

## 目录
1. [简介](#简介)
2. [核心架构与模块继承](#核心架构与模块继承)
3. [对象池与资源管理集成](#对象池与资源管理集成)
4. [OpenUIForm完整生命周期](#openuiform完整生命周期)
5. [CloseUIForm回收机制](#closeuiform回收机制)
6. [UI实例对象池配置](#ui实例对象池配置)
7. [资源依赖加载与事件系统](#资源依赖加载与事件系统)
8. [初始化与关闭最佳实践](#初始化与关闭最佳实践)

## 简介
UI管理器（UIManager）是游戏框架中的核心模块之一，负责统一管理所有用户界面的生命周期。作为GameFrameworkModule的子类，UIManager通过与对象池管理器（ObjectPoolManager）和资源管理器（ResourceManager）深度集成，实现了高效的界面实例化、资源加载和内存回收机制。本文档详细阐述其核心架构、关键流程和配置参数，为开发者提供完整的系统级理解。

## 核心架构与模块继承
UIManager继承自GameFrameworkModule抽象类，遵循游戏框架的模块化设计原则。通过实现IUIManager接口，提供了标准化的UI管理功能。模块优先级机制确保UIManager在游戏框架中按正确顺序初始化和关闭。

```mermaid
classDiagram
class GameFrameworkModule {
+virtual int Priority
+abstract void Update(float elapseSeconds, float realElapseSeconds)
+abstract void Shutdown()
}
class IUIManager {
+int UIGroupCount
+float InstanceAutoReleaseInterval
+int InstanceCapacity
+float InstanceExpireTime
+int InstancePriority
+event EventHandler<OpenUIFormSuccessEventArgs> OpenUIFormSuccess
+event EventHandler<OpenUIFormFailureEventArgs> OpenUIFormFailure
+event EventHandler<OpenUIFormUpdateEventArgs> OpenUIFormUpdate
+event EventHandler<OpenUIFormDependencyAssetEventArgs> OpenUIFormDependencyAsset
+event EventHandler<CloseUIFormCompleteEventArgs> CloseUIFormComplete
+void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
+void SetResourceManager(IResourceManager resourceManager)
+void SetUIFormHelper(IUIFormHelper uiFormHelper)
+bool HasUIGroup(string uiGroupName)
+IUIGroup GetUIGroup(string uiGroupName)
+IUIGroup[] GetAllUIGroups()
+bool AddUIGroup(string uiGroupName, IUIGroupHelper uiGroupHelper)
+bool AddUIGroup(string uiGroupName, int uiGroupDepth, IUIGroupHelper uiGroupHelper)
+bool HasUIForm(int serialId)
+bool HasUIForm(string uiFormAssetName)
+IUIForm GetUIForm(int serialId)
+IUIForm GetUIForm(string uiFormAssetName)
+IUIForm[] GetUIForms(string uiFormAssetName)
+void GetUIForms(string uiFormAssetName, List<IUIForm> results)
+IUIForm[] GetAllLoadedUIForms()
+void GetAllLoadedUIForms(List<IUIForm> results)
+int[] GetAllLoadingUIFormSerialIds()
+void GetAllLoadingUIFormSerialIds(List<int> results)
+bool IsLoadingUIForm(int serialId)
+bool IsLoadingUIForm(string uiFormAssetName)
+bool IsValidUIForm(IUIForm uiForm)
+int OpenUIForm(string uiFormAssetName, string uiGroupName)
+int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority)
+int OpenUIForm(string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm)
+int OpenUIForm(string uiFormAssetName, string uiGroupName, object userData)
+int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm)
+int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, object userData)
+int OpenUIForm(string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, object userData)
+int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm, object userData)
+void CloseUIForm(int serialId)
+void CloseUIForm(int serialId, object userData)
+void CloseUIForm(IUIForm uiForm)
+void CloseUIForm(IUIForm uiForm, object userData)
+void CloseAllLoadedUIForms()
+void CloseAllLoadedUIForms(object userData)
+void CloseAllLoadingUIForms()
+void RefocusUIForm(IUIForm uiForm)
+void RefocusUIForm(IUIForm uiForm, object userData)
+void SetUIFormInstanceLocked(object uiFormInstance, bool locked)
+void SetUIFormInstancePriority(object uiFormInstance, int priority)
}
class UIManager {
-Dictionary<string, UIGroup> m_UIGroups
-Dictionary<int, string> m_UIFormsBeingLoaded
-HashSet<int> m_UIFormsToReleaseOnLoad
-Queue<IUIForm> m_RecycleQueue
-LoadAssetCallbacks m_LoadAssetCallbacks
-IObjectPoolManager m_ObjectPoolManager
-IResourceManager m_ResourceManager
-IObjectPool<UIFormInstanceObject> m_InstancePool
-IUIFormHelper m_UIFormHelper
-int m_Serial
-bool m_IsShutdown
-EventHandler<OpenUIFormSuccessEventArgs> m_OpenUIFormSuccessEventHandler
-EventHandler<OpenUIFormFailureEventArgs> m_OpenUIFormFailureEventHandler
-EventHandler<OpenUIFormUpdateEventArgs> m_OpenUIFormUpdateEventHandler
-EventHandler<OpenUIFormDependencyAssetEventArgs> m_OpenUIFormDependencyAssetEventHandler
-EventHandler<CloseUIFormCompleteEventArgs> m_CloseUIFormCompleteEventHandler
+UIManager()
+override void Update(float elapseSeconds, float realElapseSeconds)
+override void Shutdown()
+void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
+void SetResourceManager(IResourceManager resourceManager)
+void SetUIFormHelper(IUIFormHelper uiFormHelper)
+bool AddUIGroup(string uiGroupName, int uiGroupDepth, IUIGroupHelper uiGroupHelper)
+int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm, object userData)
+void CloseUIForm(IUIForm uiForm, object userData)
+private void InternalOpenUIForm(int serialId, string uiFormAssetName, UIGroup uiGroup, object uiFormInstance, bool pauseCoveredUIForm, bool isNewInstance, float duration, object userData)
+private void LoadAssetSuccessCallback(string uiFormAssetName, object uiFormAsset, float duration, object userData)
+private void LoadAssetFailureCallback(string uiFormAssetName, LoadResourceStatus status, string errorMessage, object userData)
+private void LoadAssetUpdateCallback(string uiFormAssetName, float progress, object userData)
+private void LoadAssetDependencyAssetCallback(string uiFormAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
}
GameFrameworkModule <|-- UIManager
IUIManager <|.. UIManager
```

**图示来源**  
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L39-L1024)
- [IUIManager.cs](file://GameFramework/UI/IUIManager.cs#L17-L380)
- [GameFrameworkModule.cs](file://GameFramework/Base/GameFrameworkModule.cs#L12-L37)

**本节来源**  
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L39-L1024)
- [IUIManager.cs](file://GameFramework/UI/IUIManager.cs#L17-L380)

## 对象池与资源管理集成
UIManager通过SetObjectPoolManager和SetResourceManager方法与对象池和资源系统建立连接。在SetObjectPoolManager调用时，会创建专门用于UI实例的单次获取对象池（UI Instance Pool），实现界面实例的高效复用。

```mermaid
sequenceDiagram
participant Client as "客户端"
participant UIManager as "UIManager"
participant ObjectPoolManager as "ObjectPoolManager"
participant ResourceManager as "ResourceManager"
Client->>UIManager : SetObjectPoolManager()
UIManager->>ObjectPoolManager : CreateSingleSpawnObjectPool<UIFormInstanceObject>()
ObjectPoolManager-->>UIManager : 返回IObjectPool<UIFormInstanceObject>
UIManager->>UIManager : m_InstancePool = 返回值
Client->>UIManager : SetResourceManager()
UIManager->>UIManager : m_ResourceManager = 输入参数
Client->>UIManager : OpenUIForm()
UIManager->>UIManager : m_InstancePool.Spawn()
alt 实例存在
UIManager->>UIManager : InternalOpenUIForm()
else 实例不存在
UIManager->>ResourceManager : LoadAsset()
end
```

**图示来源**  
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L83-L100)
- [ObjectPoolManager.cs](file://GameFramework/ObjectPool/ObjectPoolManager.cs#L384-L674)
- [ResourceManager.cs](file://GameFramework/Resource/ResourceManager.cs#L1462-L1627)

**本节来源**  
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L83-L100)
- [ObjectPoolManager.cs](file://GameFramework/ObjectPool/ObjectPoolManager.cs#L384-L674)

## OpenUIForm完整生命周期
OpenUIForm操作包含序列编号生成、资源异步加载、实例化和初始化等多个阶段。系统首先检查对象池中是否存在可用实例，若不存在则通过资源管理器异步加载资源。

```mermaid
flowchart TD
Start([OpenUIForm]) --> ValidateInput["验证输入参数"]
ValidateInput --> GetSerial["生成序列编号 m_Serial++"]
GetSerial --> CheckPool["检查实例对象池 m_InstancePool.Spawn()"]
CheckPool --> PoolHit{"实例存在?"}
PoolHit --> |是| InternalOpen["InternalOpenUIForm()"]
PoolHit --> |否| MarkLoading["标记为加载中 m_UIFormsBeingLoaded.Add()"]
MarkLoading --> LoadAsset["调用资源管理器 m_ResourceManager.LoadAsset()"]
LoadAsset --> WaitCallbacks["等待加载回调"]
subgraph "加载成功回调"
WaitCallbacks --> Success["LoadAssetSuccessCallback()"]
Success --> RemoveLoading["移除加载标记 m_UIFormsBeingLoaded.Remove()"]
RemoveLoading --> CreateInstance["创建实例对象 UIFormInstanceObject.Create()"]
CreateInstance --> RegisterPool["注册到对象池 m_InstancePool.Register()"]
RegisterPool --> InternalOpen
end
subgraph "加载失败回调"
WaitCallbacks --> Failure["LoadAssetFailureCallback()"]
Failure --> RemoveLoading
Failure --> TriggerEvent["触发失败事件 m_OpenUIFormFailureEventHandler()"]
end
subgraph "加载更新回调"
WaitCallbacks --> Update["LoadAssetUpdateCallback()"]
Update --> TriggerEvent["触发更新事件 m_OpenUIFormUpdateEventHandler()"]
end
subgraph "依赖资源回调"
WaitCallbacks --> Dependency["LoadAssetDependencyAssetCallback()"]
Dependency --> TriggerEvent["触发依赖事件 m_OpenUIFormDependencyAssetEventHandler()"]
end
InternalOpen --> CreateUIForm["调用辅助器创建UI m_UIFormHelper.CreateUIForm()"]
CreateUIForm --> OnInit["调用OnInit()"]
OnInit --> AddToGroup["添加到界面组 uiGroup.AddUIForm()"]
AddToGroup --> OnOpen["调用OnOpen()"]
AddToGroup --> RefreshGroup["刷新界面组 uiGroup.Refresh()"]
RefreshGroup --> TriggerSuccess["触发成功事件 m_OpenUIFormSuccessEventHandler()"]
TriggerSuccess --> End([返回序列编号])
```

**图示来源**  
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L722-L763)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L975-L1024)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L939-L973)

**本节来源**  
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L722-L763)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L975-L1024)

## CloseUIForm回收机制
关闭界面时，系统会根据界面状态执行不同的回收策略。正在加载的界面会被标记为待释放，已加载的界面则进入回收队列，最终由对象池统一管理。

```mermaid
flowchart TD
Start([CloseUIForm]) --> CheckLoading["检查是否正在加载 IsLoadingUIForm()"]
CheckLoading --> IsLoading{"正在加载?"}
IsLoading --> |是| MarkRelease["标记待释放 m_UIFormsToReleaseOnLoad.Add()"]
IsLoading --> |否| GetUIForm["获取界面实例 GetUIForm()"]
GetUIForm --> Validate["验证界面有效性"]
Validate --> RemoveFromGroup["从界面组移除 uiGroup.RemoveUIForm()"]
RemoveFromGroup --> OnClose["调用OnClose()"]
OnClose --> RefreshGroup["刷新界面组 uiGroup.Refresh()"]
RefreshGroup --> TriggerEvent["触发完成事件 m_CloseUIFormCompleteEventHandler()"]
TriggerEvent --> EnqueueRecycle["加入回收队列 m_RecycleQueue.Enqueue()"]
EnqueueRecycle --> End([方法返回])
subgraph "Update轮询"
UpdateStart([Update]) --> CheckRecycle["检查回收队列 m_RecycleQueue.Count > 0"]
CheckRecycle --> HasRecycle{"有回收项?"}
HasRecycle --> |是| Dequeue["出队列 m_RecycleQueue.Dequeue()"]
Dequeue --> OnRecycle["调用OnRecycle()"]
OnRecycle --> Unspawn["对象池反生实例 m_InstancePool.Unspawn()"]
Unspawn --> UpdateStart
HasRecycle --> |否| UpdateEnd([Update结束])
end
```

**图示来源**  
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L779-L836)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L633-L763)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L100-L115)

**本节来源**  
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L779-L836)

## UI实例对象池配置
UI实例对象池的配置参数直接影响内存性能和资源利用率。通过InstanceAutoReleaseInterval、InstanceCapacity等属性，开发者可以精细控制对象池行为。

| 配置属性 | 获取/设置方法 | 默认值 | 说明 |
|---------|-------------|-------|------|
| 自动释放间隔 | InstanceAutoReleaseInterval | 由ObjectPoolManager决定 | 控制对象池自动释放可释放对象的时间间隔（秒） |
| 对象池容量 | InstanceCapacity | int.MaxValue | 限制对象池中对象的最大数量 |
| 对象过期时间 | InstanceExpireTime | float.MaxValue | 设置对象在池中可存活的最长时间（秒） |
| 对象池优先级 | InstancePriority | 0 | 决定对象池在轮询时的优先级顺序 |

```mermaid
classDiagram
class ObjectPoolBase {
+string Name
+string FullName
+Type ObjectType
+int Count
+int CanReleaseCount
+bool AllowMultiSpawn
+float AutoReleaseInterval
+int Capacity
+float ExpireTime
+int Priority
+void Release()
+void Release(int toReleaseCount)
+void ReleaseAllUnused()
+ObjectInfo[] GetAllObjectInfos()
}
class IObjectPool~T~ {
+T Spawn(string name)
+T Spawn()
+bool Unspawn(T obj)
+bool Unspawn(string name, T obj)
+bool HasObject(T obj)
+bool HasObject(string name, T obj)
+void Register(T obj, bool spawn)
+void Update(float elapseSeconds, float realElapseSeconds)
+void Shutdown()
}
class UIFormInstanceObject {
-object m_UIFormAsset
-IUIFormHelper m_UIFormHelper
+static UIFormInstanceObject Create(string name, object uiFormAsset, object uiFormInstance, IUIFormHelper uiFormHelper)
+override void Clear()
+protected internal override void Release(bool isShutdown)
}
ObjectPoolBase <|-- IObjectPool
IObjectPool <|-- UIManager.m_InstancePool
ObjectBase <|-- UIFormInstanceObject
```

**图示来源**  
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L117-L149)
- [ObjectPoolBase.cs](file://GameFramework/ObjectPool/ObjectPoolBase.cs#L12-L152)
- [UIManager.UIFormInstanceObject.cs](file://GameFramework/UI/UIManager.UIFormInstanceObject.cs#L12-L60)

**本节来源**  
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L117-L149)

## 资源依赖加载与事件系统
UIManager通过LoadAssetCallbacks处理资源依赖加载，与ResourceManager深度集成。加载过程中的各个阶段都会触发相应的事件，实现完整的回调机制。

```mermaid
sequenceDiagram
participant UIManager as "UIManager"
participant ResourceManager as "ResourceManager"
participant LoadAssetCallbacks as "LoadAssetCallbacks"
UIManager->>ResourceManager : LoadAsset(uiFormAssetName, priority, m_LoadAssetCallbacks, userData)
ResourceManager->>ResourceManager : 创建加载任务
ResourceManager->>LoadAssetCallbacks : LoadAssetDependencyAssetCallback()
LoadAssetCallbacks->>UIManager : 转发到 m_OpenUIFormDependencyAssetEventHandler
UIManager->>UIManager : 触发 OpenUIFormDependencyAsset 事件
ResourceManager->>LoadAssetCallbacks : LoadAssetUpdateCallback()
LoadAssetCallbacks->>UIManager : 转发到 m_OpenUIFormUpdateEventHandler
UIManager->>UIManager : 触发 OpenUIFormUpdate 事件
ResourceManager->>LoadAssetCallbacks : LoadAssetSuccessCallback()
LoadAssetCallbacks->>UIManager : 调用 LoadAssetSuccessCallback()
UIManager->>UIManager : 创建UI实例并初始化
UIManager->>UIManager : 触发 OpenUIFormSuccess 事件
ResourceManager->>LoadAssetCallbacks : LoadAssetFailureCallback()
LoadAssetCallbacks->>UIManager : 调用 LoadAssetFailureCallback()
UIManager->>UIManager : 触发 OpenUIFormFailure 事件
```

**图示来源**  
- [LoadAssetCallbacks.cs](file://GameFramework/Resource/LoadAssetCallbacks.cs#L12-L145)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L975-L102