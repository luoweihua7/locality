using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Locality
{
    /// <summary>
    /// 挂载文件信息
    /// </summary>
    public class FileHook : Hook
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public new string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public new bool Enable
        {
            get
            {
                return this._Enable;
            }
            set
            {
                if (this.Watcher != null)
                {
                    //文件夹状态下，有一个文件变动监听类
                    if (value)
                    {
                        this.Watcher.Enable();
                    }
                    else
                    {
                        this.Watcher.Disable();
                    }
                }

                this._Enable = value;
            }
        }
        private bool _Enable { get; set; }

        /// <summary>
        /// 文件目录
        /// </summary>
        public new string Path
        {
            get
            {
                return this._Path;
            }
            set
            {
                var path = value;
                var name = string.Empty;
                var dir = string.Empty;
                HookType type = HookType.File;

                if (File.Exists(path))
                {
                    FileInfo fileInfo = new FileInfo(path);
                    type = HookType.File;
                    name = fileInfo.Name;
                    dir = fileInfo.DirectoryName;
                }

                if (Directory.Exists(path))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(path);
                    type = HookType.Folder;
                    name = dirInfo.Name;
                    dir = path;
                }

                this.Type = type;
                this._Path = path;
                this.Name = name; //获取文件名，赋值给名称字段
            }
        }
        private string _Path { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public new HookType Type { get; set; }

        /// <summary>
        /// 文件路径列表
        /// </summary>
        public List<string> Files { get; set; }

        /// <summary>
        /// 文件变化监听器
        /// </summary>
        private FileWatcher Watcher;

        public FileHook(string path, bool enable = true)
        {
            path = path.ToLower(); //转换成全小写，URL不区分大小写

            this.Path = path;
            this.Files = new List<string>();

            if (this.Type == HookType.Folder)
            {
                this.Watcher = new FileWatcher(this);
            }
            else
            {
                Files.Add(System.IO.Path.GetFileName(path)); //文件模式，列表中只有一个文件
            }
            this.Enable = enable;
        }
    }

    /// <summary>
    /// 可序列化保存的挂载文件列表
    /// </summary>
    [Serializable]
    public class FileHookCollection : List<FileHook>
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
