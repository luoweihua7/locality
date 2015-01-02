using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Locality
{
    /// <summary>
    /// 目录文件变化监听类
    /// </summary>
    [Serializable]
    public class FileWatcher
    {
        private FileSystemWatcher fileWatcher;
        private System.Threading.Timer timer;
        private string directory = string.Empty;
        private FileHook fileHook;

        public FileWatcher(FileHook hook)
        {
            directory = hook.Path;
            fileHook = hook;
            fileWatcher = new FileSystemWatcher(directory);
            timer = new System.Threading.Timer(new TimerCallback(GetFiles), null, Timeout.Infinite, Timeout.Infinite);

            fileWatcher.IncludeSubdirectories = true;
            fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.DirectoryName;

            this.Enable();
            this.GetFiles(null); //触发一次获取文件列表
        }

        /// <summary>
        /// 启用监听
        /// </summary>
        public void Enable()
        {
            fileWatcher.Changed += OnChanged;
            fileWatcher.Created += OnChanged;
            fileWatcher.Deleted += OnChanged;
            fileWatcher.Renamed += OnChanged;
            fileWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 禁用监听
        /// </summary>
        public void Disable()
        {
            fileWatcher.EnableRaisingEvents = false;
            fileWatcher.Changed -= OnChanged;
            fileWatcher.Created -= OnChanged;
            fileWatcher.Deleted -= OnChanged;
            fileWatcher.Renamed -= OnChanged;
        }

        public void GetFiles(object state)
        {
            fileHook.Files = UtilService.GetFiles(directory);
        }

        void OnChanged(object sender, FileSystemEventArgs e)
        {
            timer.Change(0x200, Timeout.Infinite);
        }
    }
}
