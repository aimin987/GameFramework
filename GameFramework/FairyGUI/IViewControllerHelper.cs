

namespace GameFramework.FairyGUI
{
    /// <summary>
    /// FairyGUI界面辅助器接口
    /// </summary>
    public interface IViewControllerHelper
    {
        /// <summary>
        /// 创建界面控制器
        /// </summary>
        /// <param name="viewId">界面编号</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns></returns>
        IViewController CreateViewController(int viewId, object userData);

        /// <summary>
        /// 获取界面名称
        /// </summary>
        /// <param name="viewId">界面编号</param>
        /// <returns></returns>
        string GetViewName(int viewId);

        /// <summary>
        /// 获取界面组名称
        /// </summary>
        /// <param name="viewId">界面编号</param>
        /// <returns></returns>
        string GetGroupName(int viewId);

        /// <summary>
        /// 激活界面控制器
        /// </summary>
        /// <param name="viewController"></param>
        void ActivateViewController(IViewController viewController);

        /// <summary>
        /// 使界面控制器失效
        /// </summary>
        /// <param name="viewController"></param>
        void DeactivateViewController(IViewController viewController);

        /// <summary>
        /// 移除包
        /// </summary>
        /// <param name="packageName"></param>
        void RemovePackage(string packageName);
    }
}