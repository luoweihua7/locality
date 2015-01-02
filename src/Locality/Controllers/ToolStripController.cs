using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Locality
{
    public class ToolStripController
    {
        /// <summary>
        /// 获取或设置工具栏组件
        /// </summary>
        public static ToolStrip toolBar;

        /// <summary>
        /// 初始化时将场景配置恢复到工具栏
        /// </summary>
        /// <param name="list"></param>
        public static void Initialize(SchemeList list)
        {
            list.ForEach(item =>
            {
                Add(item.Name, item.Hosts, item.Enable);
            });
        }

        /// <summary>
        /// 添加配置到工具栏
        /// </summary>
        /// <param name="schemeName">配置名称</param>
        /// <param name="enable">是否启用</param>
        public static void Add(string schemeName, List<string> hosts, bool enable = true)
        {
            ToolStripButton button = Get(schemeName);
            if (button == null)
            {
                StringBuilder sb = new StringBuilder();
                hosts.ForEach(host => { sb.AppendLine(host); });

                button = new ToolStripButton();
                button.Text = schemeName;
                button.Name = schemeName;
                button.Checked = enable;
                button.ToolTipText = sb.ToString();
                button.Click += new EventHandler(OnButtonCheckedChange);
                toolBar.Items.Add(button);

                SchemeService.Add(schemeName, hosts, enable);
            }
            else
            {
                button.Checked = enable;
                SchemeService.Update(schemeName, hosts, enable);
            }
        }

        /// <summary>
        /// 根据配置名称删除工具栏项
        /// </summary>
        /// <param name="schemeName"></param>
        public static void Remove(string schemeName)
        {
            ToolStripButton button = Get(schemeName);
            if (button != null)
            {
                toolBar.Items.Remove(button);
                SchemeService.Remove(schemeName);
            }
        }

        /// <summary>
        /// 更新工具栏项，设置对应配置名称，挂载的域名列表，并设置是否启用
        /// <para>用于编辑状态列表时</para>
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="enable"></param>
        public static void Update(string oldName, string newName, List<string> hosts, bool enable = true)
        {
            ToolStripButton button = Get(oldName);
            if (button != null)
            {
                StringBuilder sb = new StringBuilder();
                hosts.ForEach(host => { sb.AppendLine(host); });

                button.Text = newName;
                button.Name = newName;
                button.ToolTipText = sb.ToString();
                button.Checked = enable;

                SchemeService.Update(oldName, newName, hosts, enable);
            }
        }

        /// <summary>
        /// 设置对应配置是否启用
        /// <para>用于界面上点击</para>
        /// </summary>
        /// <param name="schemeName"></param>
        /// <param name="enable"></param>
        public static void Update(string schemeName, bool enable = true)
        {
            ToolStripButton button = Get(schemeName);
            if (button != null)
            {
                button.Checked = enable;

                SchemeService.Update(schemeName, enable);
            }
        }

        /// <summary>
        /// 根据配置名称查找配置是否存在
        /// </summary>
        /// <param name="schemeName"></param>
        /// <returns></returns>
        public static ToolStripButton Get(string schemeName)
        {
            ToolStripButton button = null;

            foreach (ToolStripItem item in toolBar.Items)
            {
                if (item.Name == schemeName)
                {
                    button = (ToolStripButton)item;
                    break;
                }
            }

            return button;
        }

        private static void OnButtonCheckedChange(object sender, EventArgs e)
        {
            ToolStripButton button = (ToolStripButton)sender;

            string schemeName = button.Name;
            bool enable = !button.Checked;
            button.Checked = enable;

            SchemeService.Update(schemeName, enable);
        }
    }
}
