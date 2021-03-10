using System.Collections.Generic;

namespace GameFramework.FairyGUI
{
    internal partial class GUIManager
    {
        /// <summary>
        /// FairyGUI界面组
        /// </summary>
        private sealed partial class GUIGroup : IGUIGroup
        {
            private readonly string m_Name;                                         // 界面组名称
            private int m_Depth;                                                    // 界面组深度

            //private readonly LinkedList<IViewController> m_ViewControllerList;      // 界面控制器列表
            private readonly Dictionary<int, IViewController> m_ViewControllerDict; // 界面控制器字典

            /// <summary>
            /// 初始化界面组
            /// </summary>
            /// <param name="name">界面组名称</param>
            /// <param name="depth">界面组深度</param>
            public GUIGroup(string name, int depth)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new GameFrameworkException("GUI group name is invalid.");
                }

                m_Name = name;
                m_Depth = depth;
                //m_ViewControllerList = new LinkedList<IViewController>();
                m_ViewControllerDict = new Dictionary<int, IViewController>();
            }

            /// <summary>
            /// 获取界面组名称
            /// </summary>
            /// <returns></returns>
            public string Name
            {
                get
                {
                    return m_Name;
                }
            }

            /// <summary>
            /// 界面组深度
            /// </summary>
            /// <returns></returns>
            public int Depth
            {
                get { return m_Depth; }
                set
                {
                    if (m_Depth == value)
                    {
                        return;
                    }
                    m_Depth = value;
                }
            }

            /// <summary>
            /// 界面控制数量
            /// </summary>
            /// <returns></returns>
            public int Count
            {
                get
                {
                    return m_ViewControllerDict.Count;
                }
            }

            /// <summary>
            /// FairyGUI界面组轮询。
            /// </summary>
            /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
            /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                // LinkedListNode<IViewController> current = m_ViewControllerList.First;
                // while (current != null)
                // {
                //     LinkedListNode<IViewController> next = current.Next;
                //     current.Value.OnUpdate(elapseSeconds, realElapseSeconds);
                //     current = next;
                // }

                foreach (var item in m_ViewControllerDict)
                {
                    if (item.Value.IsActived)
                    {
                        item.Value.OnUpdate(elapseSeconds, realElapseSeconds);
                    }
                }
            }

            /// <summary>
            /// 是否存在界面控制器
            /// </summary>
            /// <param name="viewId"></param>
            /// <returns></returns>
            public bool HasViewController(int viewId)
            {
                // foreach (IViewController item in m_ViewControllerList)
                // {
                //     if (item.ViewId == viewId)
                //     {
                //         return true;
                //     }
                // }

                // return false;
                return m_ViewControllerDict.ContainsKey(viewId);
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
                    throw new GameFrameworkException("FairyGUI view name is invalid.");
                }

                foreach (var item in m_ViewControllerDict)
                {
                    if (item.Value.ViewName == viewName)
                    {
                        return true;
                    }
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
                /* foreach (IViewController item in m_ViewControllerList)
                {
                    if (item.ViewId == viewId)
                    {
                        return item;
                    }
                }

                return null; */

                IViewController viewController = null;
                if (m_ViewControllerDict.TryGetValue(viewId, out viewController))
                {
                    return viewController;
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
                    throw new GameFrameworkException("FairyGUI view name is invalid.");
                }

                foreach (var item in m_ViewControllerDict)
                {
                    if (item.Value.ViewName == viewName)
                    {
                        return item.Value;
                    }
                }

                return null;
            }

            /// <summary>
            /// 增加界面控制器
            /// </summary>
            /// <param name="viewController">界面控制器</param>
            public void AddViewController(IViewController viewController)
            {
                // m_ViewControllerList.AddFirst(viewController);
                m_ViewControllerDict.Add(viewController.ViewId, viewController);
            }


            /// <summary>
            /// 移除界面控制器
            /// </summary>
            /// <param name="viewId">界面编号</param>
            public void RemoveViewController(int viewId)
            {
                // IViewController viewController = this.GetViewController(viewId);
                // if (viewController == null)
                // {
                //     throw new GameFrameworkException("View controller is invalid.");
                // }

                if (!this.HasViewController(viewId))
                {
                    throw new GameFrameworkException("View controller is invalid.");
                }

                m_ViewControllerDict.Remove(viewId);
                //this.RemoveViewController(viewController);
            }

            /// <summary>
            /// 移除界面控制器
            /// </summary>
            /// <param name="viewName">界面名称</param>
            public void RemoveViewController(string viewName)
            {
                if (string.IsNullOrEmpty(viewName))
                {
                    throw new GameFrameworkException("FairyGUI view name is invalid.");
                }

                IViewController viewController = this.GetViewController(viewName);
                if (viewController == null)
                {
                    throw new GameFrameworkException("View controller is invalid.");
                }

                this.RemoveViewController(viewController);
            }

            /// <summary>
            /// 移除界面控制器
            /// </summary>
            /// <param name="viewController"></param>
            public void RemoveViewController(IViewController viewController)
            {
                if (!m_ViewControllerDict.ContainsKey(viewController.ViewId))
                {
                    throw new GameFrameworkException("View controller is invalid.");
                }

                m_ViewControllerDict.Remove(viewController.ViewId);
            }

            /// <summary>
            /// 获取所有界面控制器
            /// </summary>
            /// <returns></returns>
            public IViewController[] GetAllViewControllers()
            {
                List<IViewController> forms = new List<IViewController>();
                foreach (var item in m_ViewControllerDict)
                {
                    forms.Add(item.Value);
                }
                return forms.ToArray();
            }

            /// <summary>
            /// 移动除所有界面控制器
            /// </summary>
            public void RemoveAll()
            {
                m_ViewControllerDict.Clear();
            }
        }
    }
}