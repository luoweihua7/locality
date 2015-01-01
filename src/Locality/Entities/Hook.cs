using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Locality
{
    [Serializable]
    public class Hook
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public HookType Type { get; set; }
    }

    /// <summary>
    /// 配置保存对象
    /// </summary>
    [Serializable]
    public class HookCollection : List<Hook>
    {

    }
}
