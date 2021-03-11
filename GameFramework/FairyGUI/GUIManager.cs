using GameFramework.ObjectPool;
using GameFramework.Resource;
using System;
using System.Collections.Generic;

namespace GameFramework.FairyGUI
{
    /// <summary>
    /// FairyGUI管理器
    /// </summary>
    internal sealed partial class GUIManager : GameFrameworkModule, IGUIManager
    {
        private readonly Dictionary<string, GUIGroup> m_GUIGroups;                          // 界面组字典
        private IViewControllerHelper m_ViewControllerHelper;                               // 界面辅助器
        private IGUIGroupHelper m_GUIGroupHelper;                                           // 界面组辅助器
        private IResourceManager m_ResourceManager;                                         // 资源管理器
        // private readonly List<IViewController> m_RecycleQueue;                        // 释放列队

        private EventHandler<OpenViewSuccessEventArgs> m_OpenViewSuccessEventHandler;       // 打开界面成功事件
        private EventHandler<CloseViewCompleteEventArgs> m_CloseViewCompleteEventHandler;   // 关闭界面成功事件

        /// <summary>
        /// 初始化FairyGUI管理器
        /// </summary>
        public GUIManager()
        {
            m_GUIGroups = new Dictionary<string, GUIGroup>();
            m_ViewControllerHelper = null;
            // m_RecycleQueue = new List<IViewController>();
            m_ResourceManager = null;

            m_OpenViewSuccessEventHandler = null;
            m_CloseViewCompleteEventHandler = null;
        }

        /// <summary>
        /// 界面组数量
        /// </summary>
        public int GUIGroupCount
        {
            get
            {
                return m_GUIGroups.Count;
            }
        }

        /// <summary>
        /// 打开界面成功事件
        /// </summary>
        public event EventHandler<OpenViewSuccessEventArgs> OpenViewSuccess
        {
            add
            {
                m_OpenViewSuccessEventHandler += value;
            }
            remove
            {
                m_OpenViewSuccessEventHandler -= value;
            }
        }

        /// <summary>
        /// 关闭界面完成事件
        /// </summary>
        public event EventHandler<CloseViewCompleteEventArgs> CloseViewComplete
        {
            add
            {
                m_CloseViewCompleteEventHandler += value;
            }
            remove
            {
                m_CloseViewCompleteEventHandler -= value;
            }
        }

        /* ============================================={ Module }============================================= */


        /// <summary>
        /// FairyGUI管理器轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (KeyValuePair<string, GUIGroup> item in m_GUIGroups)
            {
                item.Value.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 关闭并清理界面管理器
        /// </summary>
        internal override void Shutdown()
        {
            foreach (var item in m_GUIGroups)
            {
                item.Value.RemoveAll();
            }

            m_GUIGroups.Clear();
            // m_RecycleQueue.Clear();
        }

        /* ============================================={ Interface }============================================= */

        /// <summary>
        /// 设置界面控制器辅助器
        /// </summary>
        /// <param name="helper"></param>
        public void SetViewControllerHelper(IViewControllerHelper helper)
        {
            if (helper == null)
            {
                throw new GameFrameworkException("View controller helper is invalid.");
            }

            m_ViewControllerHelper = helper;
        }

        /// <summary>
        /// 设置界面组辅助器
        /// </summary>
        /// <param name="helper"></param>
        public void SetGUIGroupHelper(IGUIGroupHelper helper)
        {
            if (helper == null)
            {
                throw new GameFrameworkException("GUI group helper is invalid.");
            }

            m_GUIGroupHelper = helper;
        }

        /// <summary>
        /// 设置资源管理器
        /// </summary>
        /// <param name="resourceManager"></param>
        public void SetResourceManager(IResourceManager resourceManager)
        {
            if (resourceManager == null)
            {
                throw new GameFrameworkException("Resource manager is invalid.");
            }

            m_ResourceManager = resourceManager;
        }

        /// <summary>
        /// 是否存在界面组
        /// </summary>
        /// <param name="groupName">界面组名称</param>
        /// <returns></returns>
        public bool HasGUIGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new GameFrameworkException("FairyGUI group name is invalid.");
            }

            return m_GUIGroups.ContainsKey(groupName);
        }

        /// <summary>
        /// 获取界面组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public IGUIGroup GetGUIGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new GameFrameworkException("FairyGUI group name is invalid.");
            }

            GUIGroup group = null;
            if (m_GUIGroups.TryGetValue(groupName, out group))
            {
                return group;
            }

            return null;
        }

        /// <summary>
        /// 获取所有界面组
        /// </summary>
        /// <returns></returns>
        public IGUIGroup[] GetAllGUIGroup()
        {
            int index = 0;
            IGUIGroup[] groups = new IGUIGroup[m_GUIGroups.Count];
            foreach (KeyValuePair<string, GUIGroup> item in m_GUIGroups)
            {
                groups[index] = item.Value;
                index++;
            }

            return groups;
        }

        /// <summary>
        /// 增加界面组
        /// </summary>
        /// <param name="groupName">界面名称</param>
        /// <returns></returns>
        public bool AddGUIGroup(string groupName)
        {
            return this.AddGUIGroup(groupName, 0);
        }

        /// <summary>
        /// 增加界面组
        /// </summary>
        /// <param name="groupName">界面名称</param>
        /// <param name="groupDepth">界面深度</param>
        /// <returns></returns>
        public bool AddGUIGroup(string groupName, int groupDepth)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new GameFrameworkException("FairyGUI group name is invalid.");
            }

            if (this.HasGUIGroup(groupName))
            {
                return false;
            }

            m_GUIGroups.Add(groupName, new GUIGroup(groupName, groupDepth));
            m_GUIGroupHelper.CreateGUIGroup(groupName, groupDepth);

            return true;
        }

        /// <summary>
        /// 是否存在界面控制器
        /// </summary>
        /// <param name="viewId">界面编号</param>
        /// <returns></returns>
        public bool HasViewController(int viewId)
        {
            GUIGroup group = this.GetGUIGroup(viewId) as GUIGroup;
            if (group != null)
            {
                return group.HasViewController(viewId);
            }

            return false;
        }

        /// <summary>
        /// 是否存在界面控制器
        /// </summary>
        /// <param name="viewName">界面名称</param>
        /// <returns></returns>
        public bool HasViewController(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new GameFrameworkException("FairyUI view  name is invalid.");
            }

            GUIGroup group = this.GetGUIGroup(viewName) as GUIGroup;
            if (group != null)
            {
                return group.HasViewController(viewName);
            }

            return false;
        }

        /// <summary>
        /// 获取界面控制器
        /// </summary>
        /// <param name="viewId">界面编号</param>
        /// <returns></returns>
        public IViewController GetViewController(int viewId)
        {
            GUIGroup group = this.GetGUIGroup(viewId);
            if (group != null)
            {
                return group.GetViewController(viewId);
            }

            return null;
        }

        /// <summary>
        /// 获取界面控制器
        /// </summary>
        /// <param name="viewName">界面名称</param>
        /// <returns></returns>
        public IViewController GetViewController(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new GameFrameworkException("FairyUI view  name is invalid.");
            }

            foreach (KeyValuePair<string, GUIGroup> group in m_GUIGroups)
            {
                IViewController viewController = group.Value.GetViewController(viewName);
                if (viewController != null)
                {
                    return viewController;
                }
            }

            return null;
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="viewId">界面编号</param>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="animated">是否播放动画</param>
        /// <returns>当前界面控制器</returns>
        public IViewController OpenView(int viewId, object userData, bool animated)
        {
            GUIGroup group = this.GetGUIGroup(viewId);
            if (group == null)
            {
                throw new GameFrameworkException(string.Format("FairyGUI viewId group '{0}' is not exist.", viewId));
            }

            IViewController viewController = group.GetViewController(viewId);
            if (viewController != null)
            {
                viewController.OnShow(userData);
                viewController.OnOpen(animated);
            }
            else
            {
                viewController = m_ViewControllerHelper.CreateViewController(viewId, userData);
                viewController.OnInit();
                viewController.OnShow(userData);
                viewController.OnOpen(animated);
                group.AddViewController(viewController);
            }

            if (m_OpenViewSuccessEventHandler != null)
            {
                m_OpenViewSuccessEventHandler(this, new OpenViewSuccessEventArgs(viewId, viewController.ViewName, viewController.GroupName, userData));
            }

            return viewController;
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="viewId">界面编号</param>
        /// <param name="animated">是否播放动画</param>
        public void CloseView(int viewId, bool animated)
        {
            IViewController viewController = this.GetViewController(viewId);
            if (viewController == null)
            {
                throw new GameFrameworkException(string.Format("View id: '{0}' is not exist.", viewId));
            }

            this.CloseView(viewController, animated);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="viewController">界面控制器</param>
        /// <param name="animated">是否播放动画</param>
        public void CloseView(IViewController viewController, bool animated)
        {
            if (viewController.IsLocked)
            {
                viewController.OnClose(animated);
            }
            else
            {
                IGUIGroup group = this.GetGUIGroup(viewController.ViewId);
                group.RemoveViewController(viewController);
                viewController.OnClose(animated);
            }

            if (m_CloseViewCompleteEventHandler != null)
            {
                m_CloseViewCompleteEventHandler(this, new CloseViewCompleteEventArgs(viewController.ViewId, viewController.ViewName, viewController.GroupName, null));
            }
        }

        /// <summary>
        /// 关闭所有界面
        /// </summary>
        public void CloseAllViews()
        {
            List<IViewController> list = new List<IViewController>();

            foreach (KeyValuePair<string, GUIGroup> item in m_GUIGroups)
            {
                IViewController[] viewArray = item.Value.GetAllViewControllers();
                for (int i = 0; i < viewArray.Length; i++)
                {
                    this.CloseView(viewArray[i], false);
                }
            }
        }

        /// <summary>
        /// 根据界面编号获取界面组名称
        /// </summary>
        /// <param name="viewId"></param>
        /// <returns></returns>
        private string GetGroupName(int viewId)
        {
            return m_ViewControllerHelper.GetGroupName(viewId);
        }

        /// <summary>
        /// 获取界面组
        /// </summary>
        /// <param name="viewId"></param>
        /// <returns></returns>
        private GUIGroup GetGUIGroup(int viewId)
        {
            string groupName = this.GetGroupName(viewId);
            return this.GetGUIGroup(groupName) as GUIGroup;
        }
    }
}