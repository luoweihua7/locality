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

            });
        }

        public static void Add()
        {

        }
    }
}
