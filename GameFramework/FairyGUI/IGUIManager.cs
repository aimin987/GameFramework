using GameFramework.ObjectPool;
using GameFramework.Resource;
using System;

namespace GameFramework.FairyGUI
{
    /// <summary>
    /// Fairy界面管理器
    /// </summary>
    public interface IGUIManager
    {
        /// <summary>
        /// 界面组数量
        /// </summary>
        /// <returns></returns>
        int GUIGroupCount { get; }

        /// <summary>
        /// 打开界面成功事件。
        /// </summary>
        event EventHandler<OpenViewSuccessEventArgs> OpenViewSuccess;

        /// <summary>
        /// 关闭界面完成事件。
        /// </summary>
        event EventHandler<CloseViewCompleteEventArgs> CloseViewComplete;

        /// <summary>
        /// 设置界面控制器辅助器。
        /// </summary>
        void SetViewControllerHelper(IViewControllerHelper helper);

        /// <summary>
        /// 设置界面组管理器
        /// </summary>
        /// <param name="helper"></param>
        void SetGUIGroupHelper(IGUIGroupHelper helper);

        /// <summary>
        /// 设置资源管理器
        /// </summary>
        /// <param name="resourceManager"></param>
        void SetResourceManager(IResourceManager resourceManager);

        /// <summary>
        /// 是否存在界面组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        bool HasGUIGroup(string groupName);

        /// <summary>
        /// 获取界面组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        IGUIGroup GetGUIGroup(string groupName);

        /// <summary>
        /// 获取全部界面组
        /// </summary>
        /// <returns></returns>
        IGUIGroup[] GetAllGUIGroup();

        /// <summary>
        /// 增加界面组
        /// </summary>
        /// <param name="groupName">界面组名称</param>
        /// <param name="groupDepth">界面组深度</param>
        /// <returns></returns>
        bool AddGUIGroup(string groupName, int groupDepth);

        /// <summary>
        /// 增加界面组
        /// </summary>
        /// <param name="groupName">界面组名称</param>
        /// <returns></returns>
        bool AddGUIGroup(string groupName);

        /// <summary>
        /// 是否存在界面控制器
        /// </summary>
        /// <param name="viewId">界面Id</param>
        /// <returns></returns>
        bool HasViewController(int viewId);

        /// <summary>
        /// 是否存在界面
        /// </summary>
        /// <param name="viewName">界面名</param>
        /// <returns></returns>
        bool HasViewController(string viewName);

        /// <summary>
        /// 获取界面控制器
        /// </summary>
        /// <param name="viewId">界面编号</param>
        /// <returns></returns>
        IViewController GetViewController(int viewId);

        /// <summary>
        /// 获取界面控制
        /// </summary>
        /// <param name="viewName">界面名称</param>
        /// <returns></returns>
        IViewController GetViewController(string viewName);

        /// <summary>
        /// 获取全部界面控制器
        /// </summary>
        /// <returns></returns>
        IViewController[] GetAllViewControllers();

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="viewId">界面编号</param>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="animated">播放打开动画</param>
        IViewController OpenView(int viewId, object userData, bool animated);

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="viewId">界面编号</param>
        /// <param name="animated">播放关闭动画</param>
        void CloseView(int viewId, bool animated);

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="viewController">界面控制器</param>
        /// <param name="animated">播放关闭动画</param>
        void CloseView(IViewController viewController, bool animated);

        /// <summary>
        /// 关闭所有界面
        /// </summary>
        void CloseAllViews();

        //void AddGUIAsset(string packageName, object asset);

        //void RemoveGUIAsset(string packageName);
    }
}