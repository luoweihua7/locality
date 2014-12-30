using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Locality
{
    /// <summary>
    /// 挂载文件信息
    /// </summary>
    [Serializable]
    public class FileHook
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public Boolean Enable { get; set; }

        /// <summary>
        /// 文件目录
        /// </summary>
        public string Path
        {
            get;
            set
            {
                var name = Path.Substring(Path.LastIndexOf('/'));

                this.Path = value;
                this.Name = name; //获取文件名，赋值给名称字段
            }
        }

        /// <summary>
        /// 类型
        /// </summary>
        public HookType Type { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FileHookList : List<FileHook>
    {

    }

    /// <summary>
    /// 挂载的文件类型
    /// </summary>
    public enum HookType
    {
        /// <summary>
        /// 文件
        /// </summary>
        File = 0,

        /// <summary>
        /// 文件夹
        /// </summary>
        Folder = 1
    }
}
