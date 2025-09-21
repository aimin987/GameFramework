# UI生命周期管理

<cite>
**Referenced Files in This Document**   
- [UIManager.cs](file://GameFramework/UI/UIManager.cs)
- [OpenUIFormSuccessEventArgs.cs](file://GameFramework/UI/OpenUIFormSuccessEventArgs.cs)
- [OpenUIFormFailureEventArgs.cs](file://GameFramework/UI/OpenUIFormFailureEventArgs.cs)
- [CloseUIFormCompleteEventArgs.cs](file://GameFramework/UI/CloseUIFormCompleteEventArgs.cs)
- [UIManager.UIGroup.cs](file://GameFramework/UI/UIManager.UIGroup.cs)
- [UIManager.UIFormInstanceObject.cs](file://GameFramework/UI/UIManager.UIFormInstanceObject.cs)
- [IUIForm.cs](file://GameFramework/UI/IUIForm.cs)
</cite>

## 目录
1. [简介](#简介)
2. [核心组件](#核心组件)
3. [UI打开流程](#ui打开流程)
4. [UI关闭流程](#ui关闭流程)
5. [事件系统](#事件系统)
6. [序列编号机制](#序列编号机制)
7. [异常处理](#异常处理)
8. [最佳实践](#最佳实践)

## 简介
UI生命周期管理是游戏框架中的核心功能之一，负责界面的创建、显示、隐藏和销毁等完整生命周期管理。本系统通过`UIManager`类实现，提供了`OpenUIForm`和`CloseUIForm`等核心方法，实现了从请求打开、资源加载、实例化到最终激活的完整流程，以及关闭时的回收策略与事件通知机制。

## 核心组件

UI生命周期管理涉及多个核心组件的协同工作：

- `UIManager`：核心管理器，负责协调整个UI生命周期
- `UIGroup`：界面组，管理一组具有相同优先级的界面
- `IUIForm`：界面接口，定义了所有UI界面必须实现的方法
- `UIFormInstanceObject`：界面实例对象，用于对象池管理
- 各类事件参数类：用于事件传递和通知

**Section sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L20-L30)
- [IUIForm.cs](file://GameFramework/UI/IUIForm.cs#L10-L25)

## UI打开流程

### 打开流程概述
UI打开流程从调用`OpenUIForm`方法开始，经历资源加载、实例化、初始化、加入界面组到最终激活的完整过程。该流程支持同步和异步两种模式，当界面实例存在于对象池中时可直接使用，否则需要异步加载资源。

```mermaid
sequenceDiagram
participant Client as "客户端"
participant UIManager as "UIManager"
participant ResourceManager as "ResourceManager"
participant UIGroup as "UIGroup"
participant UIFormHelper as "UIFormHelper"
Client->>UIManager : OpenUIForm(uiFormAssetName, uiGroupName)
activate UIManager
UIManager->>UIManager : 生成序列编号
alt 实例存在于对象池
UIManager->>UIManager : InternalOpenUIForm(直接实例化)
UIManager->>UIFormHelper : CreateUIForm
UIFormHelper-->>UIManager : 返回IUIForm
UIManager->>UIGroup : AddUIForm
UIManager->>IUIForm : OnInit
UIManager->>IUIForm : OnOpen
UIManager->>UIGroup : Refresh
UIManager->>Client : 返回序列编号
else 需要异步加载
UIManager->>ResourceManager : LoadAsset(异步加载)
ResourceManager-->>UIManager : 加载完成
alt 加载成功
UIManager->>UIManager : LoadAssetSuccessCallback
UIManager->>UIFormHelper : InstantiateUIForm
UIManager->>UIManager : InternalOpenUIForm
UIManager->>Client : 返回序列编号
else 加载失败
UIManager->>UIManager : LoadAssetFailureCallback
UIManager->>Client : 触发OpenUIFormFailure事件
end
end
deactivate UIManager
```

**Diagram sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L722-L763)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L975-L997)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L999-L1024)

**Section sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L722-L763)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L975-L1024)

### 多态OpenUIForm方法
`OpenUIForm`方法提供了多个重载版本，以满足不同的使用场景：

```mermaid
classDiagram
class UIManager {
+OpenUIForm(string uiFormAssetName, string uiGroupName) int
+OpenUIForm(string uiFormAssetName, string uiGroupName, int priority) int
+OpenUIForm(string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm) int
+OpenUIForm(string uiFormAssetName, string uiGroupName, object userData) int
+OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm) int
+OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, object userData) int
+OpenUIForm(string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, object userData) int
+OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm, object userData) int
}
```

所有重载方法最终都会调用最完整的八参数版本，其中：
- `uiFormAssetName`：界面资源名称
- `uiGroupName`：目标界面组名称
- `priority`：资源加载优先级
- `pauseCoveredUIForm`：是否暂停被覆盖的界面
- `userData`：用户自定义数据

**Diagram sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L633-L722)

**Section sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L633-L722)

### pauseCoveredUIForm参数的影响
`pauseCoveredUIForm`参数对界面栈的行为有重要影响：

```mermaid
flowchart TD
Start([开始打开新界面]) --> CheckPause["检查pauseCoveredUIForm参数"]
CheckPause --> |true| PauseCovered["暂停被覆盖的界面"]
CheckPause --> |false| KeepRunning["保持被覆盖界面运行"]
PauseCovered --> UpdateStack["更新界面栈状态"]
KeepRunning --> UpdateStack
UpdateStack --> RefreshGroup["调用UIGroup.Refresh()"]
RefreshGroup --> NotifyEvents["通知相关事件"]
NotifyEvents --> End([流程结束])
subgraph "PauseCovered分支"
PauseCovered --> CallOnPause["调用被覆盖界面的OnPause()"]
PauseCovered --> CallOnCover["调用被覆盖界面的OnCover()"]
end
subgraph "KeepRunning分支"
KeepRunning --> CallOnReveal["调用被覆盖界面的OnReveal()"]
end
```

当`pauseCoveredUIForm`为`true`时，新打开的界面会暂停其下方的所有界面，防止它们继续更新；为`false`时，被覆盖的界面将继续正常运行。

**Diagram sources**
- [UIManager.UIGroup.cs](file://GameFramework/UI/UIManager.UIGroup.cs#L400-L450)
- [IUIForm.cs](file://GameFramework/UI/IUIForm.cs#L80-L95)

**Section sources**
- [UIManager.UIGroup.cs](file://GameFramework/UI/UIManager.UIGroup.cs#L400-L450)
- [IUIForm.cs](file://GameFramework/UI/IUIForm.cs#L80-L95)

## UI关闭流程

### 关闭流程概述
UI关闭流程从调用`CloseUIForm`方法开始，经历从界面组移除、状态通知到最终回收的完整过程。系统提供了多种关闭方式，包括通过序列编号关闭和直接关闭界面实例。

```mermaid
sequenceDiagram
participant Client as "客户端"
participant UIManager as "UIManager"
participant UIGroup as "UIGroup"
participant IUIForm as "IUIForm"
Client->>UIManager : CloseUIForm(serialId)
activate UIManager
UIManager->>UIManager : 检查是否正在加载
alt 正在加载
UIManager->>UIManager : 标记为待释放
UIManager->>UIManager : 移除加载记录
UIManager->>Client : 返回
else 已加载完成
UIManager->>UIManager : GetUIForm
UIManager->>UIGroup : RemoveUIForm
activate UIGroup
UIGroup->>IUIForm : OnCover
UIGroup->>IUIForm : OnPause
UIGroup->>UIGroup : Refresh
deactivate UIGroup
UIManager->>IUIForm : OnClose
UIManager->>UIManager : 触发CloseUIFormComplete事件
UIManager->>UIManager : 加入回收队列
UIManager->>Client : 返回
end
deactivate UIManager
```

**Diagram sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L779-L836)
- [UIManager.UIGroup.cs](file://GameFramework/UI/UIManager.UIGroup.cs#L300-L350)

**Section sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L779-L836)
- [UIManager.UIGroup.cs](file://GameFramework/UI/UIManager.UIGroup.cs#L300-L350)

### 回收策略
系统采用延迟回收策略，在`Update`方法中处理回收队列：

```mermaid
flowchart TD
UpdateStart([Update方法开始]) --> CheckRecycle["检查回收队列是否为空"]
CheckRecycle --> |非空| DequeueForm["从队列中取出界面"]
CheckRecycle --> |为空| UpdateGroups["更新所有界面组"]
DequeueForm --> CallOnRecycle["调用界面的OnRecycle()方法"]
CallOnRecycle --> UnspawnInstance["从对象池中卸载实例"]
UnspawnInstance --> CheckRecycle
UpdateGroups --> UpdateEachGroup["遍历每个界面组"]
UpdateEachGroup --> CallGroupUpdate["调用UIGroup.Update()"]
CallGroupUpdate --> UpdateEnd([Update方法结束])
```

这种策略确保了回收操作不会阻塞主线程，同时保证了界面状态的正确清理。

**Diagram sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L210-L223)
- [UIManager.UIFormInstanceObject.cs](file://GameFramework/UI/UIManager.UIFormInstanceObject.cs#L50-L60)

**Section sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L210-L223)
- [UIManager.UIFormInstanceObject.cs](file://GameFramework/UI/UIManager.UIFormInstanceObject.cs#L50-L60)

## 事件系统

### 事件类型
UI管理系统提供了完整的事件通知机制，包括：

```mermaid
classDiagram
class OpenUIFormSuccessEventArgs {
+IUIForm UIForm
+float Duration
+object UserData
+static Create(IUIForm, float, object) OpenUIFormSuccessEventArgs
}
class OpenUIFormFailureEventArgs {
+int SerialId
+string UIFormAssetName
+string UIGroupName
+bool PauseCoveredUIForm
+string ErrorMessage
+object UserData
+static Create(int, string, string, bool, string, object) OpenUIFormFailureEventArgs
}
class CloseUIFormCompleteEventArgs {
+int SerialId
+string UIFormAssetName
+IUIGroup UIGroup
+object UserData
+static Create(int, string, IUIGroup, object) CloseUIFormCompleteEventArgs
}
GameFrameworkEventArgs <|-- OpenUIFormSuccessEventArgs
GameFrameworkEventArgs <|-- OpenUIFormFailureEventArgs
GameFrameworkEventArgs <|-- CloseUIFormCompleteEventArgs
```

**Diagram sources**
- [OpenUIFormSuccessEventArgs.cs](file://GameFramework/UI/OpenUIFormSuccessEventArgs.cs#L12-L76)
- [OpenUIFormFailureEventArgs.cs](file://GameFramework/UI/OpenUIFormFailureEventArgs.cs#L12-L115)
- [CloseUIFormCompleteEventArgs.cs](file://GameFramework/UI/CloseUIFormCompleteEventArgs.cs#L12-L89)

**Section sources**
- [OpenUIFormSuccessEventArgs.cs](file://GameFramework/UI/OpenUIFormSuccessEventArgs.cs#L12-L76)
- [OpenUIFormFailureEventArgs.cs](file://GameFramework/UI/OpenUIFormFailureEventArgs.cs#L12-L115)
- [CloseUIFormCompleteEventArgs.cs](file://GameFramework/UI/CloseUIFormCompleteEventArgs.cs#L12-L89)

### 事件触发条件
不同事件的触发条件如下：

```mermaid
flowchart TD
OpenStart([开始打开界面]) --> CheckInstance["检查实例池"]
CheckInstance --> |实例存在| DirectOpen["直接打开"]
CheckInstance --> |实例不存在| AsyncLoad["异步加载资源"]
DirectOpen --> InternalOpen["调用InternalOpenUIForm"]
InternalOpen --> SuccessFlow["成功流程"]
AsyncLoad --> LoadResult["等待加载结果"]
LoadResult --> |成功| SuccessFlow
LoadResult --> |失败| FailureFlow
SuccessFlow --> CreateSuccessEvent["创建OpenUIFormSuccessEventArgs"]
SuccessFlow --> NotifySuccess["通知OpenUIFormSuccess事件"]
CreateSuccessEvent --> NotifySuccess
FailureFlow --> CreateFailureEvent["创建OpenUIFormFailureEventArgs"]
FailureFlow --> NotifyFailure["通知OpenUIFormFailure事件"]
CreateFailureEvent --> NotifyFailure
CloseStart([开始关闭界面]) --> RemoveFromGroup["从界面组移除"]
RemoveFromGroup --> CallOnClose["调用OnClose方法"]
CallOnClose --> CreateCompleteEvent["创建CloseUIFormCompleteEventArgs"]
CreateCompleteEvent --> NotifyComplete["通知CloseUIFormComplete事件"]
```

**Diagram sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L939-L973)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L975-L1024)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L811-L836)

**Section sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L939-L1024)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L811-L836)

## 序列编号机制

### SerialId生成规则
序列编号（SerialId）是每个UI界面的唯一标识符，其生成规则如下：

```mermaid
flowchart TD
OpenUIForm([调用OpenUIForm]) --> IncrementSerial["m_Serial++"]
IncrementSerial --> AssignSerial["将新值赋给serialId"]
AssignSerial --> StoreSerial["在OpenUIFormInfo中存储serialId"]
StoreSerial --> ReturnSerial["返回serialId给调用者"]
class IncrementSerial m_Serial
class AssignSerial serialId
class StoreSerial OpenUIFormInfo
class ReturnSerial Client
style m_Serial fill:#f9f,stroke:#333
style serialId fill:#bbf,stroke:#333
style OpenUIFormInfo fill:#f96,stroke:#333
style Client fill:#6f9,stroke:#333
```

每次调用`OpenUIForm`时，`m_Serial`字段都会自增1，并将新值作为界面的序列编号返回。这个编号在整个应用程序生命周期内保证唯一。

**Diagram sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L745-L746)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L28-L28)

**Section sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L745-L746)
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L28-L28)

### 序列编号的用途
序列编号在UI管理系统中具有多种用途：

1. **唯一标识**：作为每个UI界面的唯一标识符
2. **状态跟踪**：用于跟踪界面的加载状态
3. **关闭控制**：通过序列编号关闭特定界面
4. **事件关联**：在事件参数中传递，用于关联特定界面

```mermaid
erDiagram
UIForm ||--o{ OpenUIFormInfo : "has"
UIForm ||--o{ CloseUIFormCompleteEventArgs : "generates"
OpenUIFormInfo ||--o{ OpenUIFormSuccessEventArgs : "on success"
OpenUIFormInfo ||--o{ OpenUIFormFailureEventArgs : "on failure"
UIForm {
int SerialId PK
string UIFormAssetName
object Handle
IUIGroup UIGroup
}
OpenUIFormInfo {
int SerialId PK
string UIFormAssetName
UIGroup UIGroup
bool PauseCoveredUIForm
object UserData
}
OpenUIFormSuccessEventArgs {
int SerialId FK
IUIForm UIForm
float Duration
object UserData
}
OpenUIFormFailureEventArgs {
int SerialId PK
string UIFormAssetName
string UIGroupName
bool PauseCoveredUIForm
string ErrorMessage
object UserData
}
CloseUIFormCompleteEventArgs {
int SerialId PK
string UIFormAssetName
IUIGroup UIGroup
object UserData
}
```

**Diagram sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L745-L746)
- [OpenUIFormSuccessEventArgs.cs](file://GameFramework/UI/OpenUIFormSuccessEventArgs.cs#L12-L76)
- [OpenUIFormFailureEventArgs.cs](file://GameFramework/UI/OpenUIFormFailureEventArgs.cs#L12-L115)
- [CloseUIFormCompleteEventArgs.cs](file://GameFramework/UI/CloseUIFormCompleteEventArgs.cs#L12-L89)

**Section sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L745-L746)
- [OpenUIFormSuccessEventArgs.cs](file://GameFramework/UI/OpenUIFormSuccessEventArgs.cs#L12-L76)
- [OpenUIFormFailureEventArgs.cs](file://GameFramework/UI/OpenUIFormFailureEventArgs.cs#L12-L115)
- [CloseUIFormCompleteEventArgs.cs](file://GameFramework/UI/CloseUIFormCompleteEventArgs.cs#L12-L89)

## 异常处理

### 异常处理机制
系统在关键操作中实现了完善的异常处理机制：

```mermaid
flowchart TD
InternalOpenStart([InternalOpenUIForm开始]) --> TryBlock["try块"]
TryBlock --> CreateUIForm["调用UIFormHelper.CreateUIForm"]
CreateUIForm --> CheckNull["检查返回值是否为null"]
CheckNull --> |null| ThrowException["抛出GameFrameworkException"]
CheckNull --> |非null| ContinueFlow["继续流程"]
ContinueFlow --> OnInit["调用OnInit"]
ContinueFlow --> AddToGroup["加入界面组"]
ContinueFlow --> OnOpen["调用OnOpen"]
ContinueFlow --> RefreshGroup["调用Refresh"]
ContinueFlow --> NotifySuccess["通知OpenUIFormSuccess"]
TryBlock --> |异常发生| CatchBlock["catch块"]
CatchBlock --> CreateFailureEvent["创建OpenUIFormFailureEventArgs"]
CatchBlock --> NotifyFailure["通知OpenUIFormFailure事件"]
CatchBlock --> CheckHandler["检查事件处理器是否存在"]
CheckHandler --> |存在| Return["返回，不抛出异常"]
CheckHandler --> |不存在| ReThrow["重新抛出异常"]
```

当在`InternalOpenUIForm`方法中发生异常时，系统会尝试通过`OpenUIFormFailure`事件通知调用者。只有当没有注册该事件的监听器时，才会将异常重新抛出。

**Diagram sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs#L939-L973)

**Section sources**
- [UIManager.cs](file://GameFramework/UI/UIManager.cs