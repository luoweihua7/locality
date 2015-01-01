using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Locality
{
    public class ListViewController
    {
        /// <summary>
        /// 获取或设置显示挂载的文件列表组件
        /// </summary>
        public static ListView listView;

        /// <summary>
        /// 初始化时将文件恢复到列表
        /// </summary>
        /// <param name="list"></param>
        public static void Initialize(HookCollection list)
        {
            list.ForEach(item =>
            {
                Add(item.Path);
                FileService.Add(item.Path, item.Enable);
            });
        }

        /// <summary>
        /// 将对应路径的文件或者文件夹添加到挂载列表
        /// <para>如果文件已经在列表中，则标记为挂载状态</para>
        /// </summary>
        /// <param name="path">文件或者文件夹的完整路径</param>
        public static void Add(string path)
        {
            ListViewItem item = GetItem(path);
            if (item == null)
            {
                //如果不存在则添加到列表
                item = new ListViewItem();
                item.Checked = true;
                item.Text = UtilService.GetName(path);
                item.SubItems.Add(path);

                listView.Items.Add(item);
            }
            else
            {
                //如果存在则标记为挂载
                item.Checked = true;
            }

            FileService.Add(path);
        }

        /// <summary>
        /// 删除列表中选中的项
        /// </summary>
        public static void RemoveSelected()
        {
            foreach (ListViewItem item in listView.SelectedItems)
            {
                var filePath = item.SubItems[1].Text;

                listView.Items.Remove(item);
                FileService.Remove(filePath);
            }
        }

        /// <summary>
        /// 删除指定路径的项
        /// </summary>
        /// <param name="path"></param>
        public static void Remove(string path)
        {
            ListViewItem item = GetItem(path);
            if (item != null)
            {
                listView.Items.Remove(item);
            }
        }

        /// <summary>
        /// 更新列表中的Checked勾选状态
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="enable"></param>
        public static void Update(string filePath, bool enable)
        {
            FileService.Update(filePath, enable);
        }

        public static void Clear()
        {
            listView.Items.Clear();
            FileService.Clear();
        }

        /// <summary>
        /// 通过给定的路径获取列表中的项
        /// </summary>
        /// <param name="path">路径地址</param>
        /// <returns>列表中的项，如果不存在则返回null</returns>
        private static ListViewItem GetItem(string path)
        {
            ListViewItem item = null;

            foreach (ListViewItem lvi in listView.Items)
            {
                var temp = lvi.SubItems[1].Text;
                if (temp == path)
                {
                    item = lvi;
                    break;
                }
            }

            return item;
        }
    }
}
