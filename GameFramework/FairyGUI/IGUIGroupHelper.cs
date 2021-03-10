namespace GameFramework.FairyGUI
{
    /// <summary>
    /// FairyGUI界面组辅助器
    /// </summary>
    public interface IGUIGroupHelper
    {
        /// <summary>
        ///  创建界面组
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="depth">深度</param>
        void CreateGUIGroup(string name, int depth);
    }
}