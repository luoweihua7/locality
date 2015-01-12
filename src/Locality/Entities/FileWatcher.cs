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
    public class FileWatcher
    {
        private FileSystemWatcher fileWatcher;
        private FileHook fileHook;
        private System.Threading.Timer timer;
        private string directory = string.Empty;
        private int delay = 0x200;

        public FileWatcher(FileHook hook)
        {
            directory = hook.Path;
            fileHook = hook;
            fileWatcher = new FileSystemWatcher(directory);
            timer = new System.Threading.Timer(new TimerCallback(GetFiles), null, delay, Timeout.Infinite);

            fileWatcher.IncludeSubdirectories = true;
            fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.DirectoryName;

            this.Enable();
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
            fileHook.Files = UtilService.GetFiles(directory, true);
        }

        void OnChanged(object sender, FileSystemEventArgs e)
        {
            timer.Change(delay, Timeout.Infinite);
        }
    }
}
