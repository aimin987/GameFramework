namespace GameFramework.FairyGUI
{
    /// <summary>
    /// FairyGUI界面组接口(存放界面控制器)
    /// </summary>
    public interface IGUIGroup
    {
        /// <summary>
        /// 界面组名称
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 获取或设置界面组深度。
        /// </summary>
        int Depth
        {
            get;
            set;
        }

        /// <summary>
        /// 界面数量
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// 界面组中是否存在界面控制器
        /// </summary>
        /// <param name="viewId">界面编号</param>
        bool HasViewController(int viewId);

        /// <summary>
        /// 界面组中是否存在界面控制器
        /// </summary>
        /// <param name="viewName">界面名称</param>
        bool HasViewController(string viewName);

        /// <summary>
        /// 获取界面控制器
        /// </summary>
        /// <param name="viewId">界面编号</param>
        IViewController GetViewController(int viewId);

        /// <summary>
        /// 获取界面控制器
        /// </summary>
        /// <param name="viewName">界面名称</param>
        IViewController GetViewController(string viewName);

        /// <summary>
        /// 增加界面控制器
        /// </summary>
        /// <param name="viewController">界面控制器</param>
        void AddViewController(IViewController viewController);

        /// <summary>
        /// 移除界面控制器
        /// </summary>
        /// <param name="viewController">界面控制器</param>
        void RemoveViewController(IViewController viewController);

        /// <summary>
        /// 移除界面控制器
        /// </summary>
        /// <param name="viewId">界面编号</param>
        void RemoveViewController(int viewId);

        /// <summary>
        /// 获取界面组里所有界面控制器
        /// </summary>
        IViewController[] GetAllViewControllers();

        /// <summary>
        /// 移除所有
        /// </summary>
        void RemoveAll();
    }
}