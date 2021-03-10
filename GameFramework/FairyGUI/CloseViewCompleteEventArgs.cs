

namespace GameFramework.FairyGUI
{
    /// <summary>
    /// 关闭界面成功事件
    /// </summary>
    public sealed class CloseViewCompleteEventArgs : GameFrameworkEventArgs
    {
        /// <summary>
        /// 界面编号
        /// </summary>
        /// <returns></returns>
        public int ViewId { get; private set; }

        /// <summary>
        /// 界面名称
        /// </summary>
        /// <returns></returns>
        public string ViewName { get; private set; }

        /// <summary>
        /// 界面组名称
        /// </summary>
        /// <returns></returns>
        public string GroupName { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        /// <returns></returns>
        public object UserData { get; private set; }

        /// <summary>
        /// 初始化事件
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="viewName"></param>
        /// <param name="groupName"></param>
        /// <param name="userData"></param>
        public CloseViewCompleteEventArgs(int viewId, string viewName, string groupName, object userData)
        {
            this.ViewId = viewId;
            this.ViewName = viewName;
            this.GroupName = groupName;
            this.UserData = userData;
        }

        /// <summary>
        /// 清理
        /// </summary>
        public override void Clear()
        {
            this.ViewId = 0;
            this.ViewName = null;
            this.GroupName = null;
            this.UserData = null;
        }
    }
}