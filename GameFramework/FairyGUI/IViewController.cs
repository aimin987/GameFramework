namespace GameFramework.FairyGUI
{
    /// <summary>
    /// 界面控制器接口
    /// </summary>
    public interface IViewController
    {
        /// <summary>
        /// 界面编号
        /// </summary>
        int ViewId { get; }

        /// <summary>
        /// 界面名称
        /// </summary>
        string ViewName { get; }

        /// <summary>
        /// 界面组名称
        /// </summary>
        /// <returns></returns>
        string GroupName { get; }

        /// <summary>
        /// 资源包名
        /// </summary>
        string PackageName { get; }

        /// <summary>
        /// 是否锁定(锁定不销毁)
        /// </summary>
        /// <returns></returns>
        bool IsLocked { get; }

        /// <summary>
        /// 是否在关闭时自动移除资源包(配置)
        /// </summary>
        /// <returns></returns>
        bool AutoRemovePackage { get; }

        /// <summary>
        /// 激活
        /// </summary>
        /// <value></value>
        bool IsActived { get; set; }

        /// <summary>
        /// 初始化界面
        /// </summary>
        void OnInit();

        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="userData"></param>
        void OnShow(object userData);

        /// <summary>
        /// 关闭界面
        /// </summary>
        void OnHide(bool destory);

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="animated">是否动画</param>
        void OnOpen(bool animated);

        /// <summary>
        /// 界面关闭
        /// </summary>
        /// <param name="animated">界面关闭</param>
        void OnClose(bool animated);

        /// <summary>
        /// 界面销毁。
        /// </summary>
        void OnDestroy();

        /// <summary>
        /// 界面轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        void OnUpdate(float elapseSeconds, float realElapseSeconds);
    }
}