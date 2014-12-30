using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locality
{
    /// <summary>
    /// 功能设置参数
    /// </summary>
    [Serializable]
    public class ConfigEntity
    {
        /// <summary>
        /// 是否启用插件
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 是否显示右下角的挂载提示窗口
        /// </summary>
        public bool EnableTip { get; set; }

        /// <summary>
        /// 严格路径模式
        /// </summary>
        public bool StrictMode { get; set; }

        /// <summary>
        /// 是否启用配置
        /// </summary>
        public bool EnableScheme { get; set; }

        /// <summary>
        /// 挂载配置(如灰度,全网等)
        /// </summary>
        public SchemeList Schemes { get; set; }

        /// <summary>
        /// 挂载的文件列表
        /// </summary>
        public FileHookList HookFiles { get; set; }

        /// <summary>
        /// 配置类
        /// </summary>
        public ConfigEntity()
        {
            this.Enable = true;
            this.EnableTip = true;
            this.StrictMode = false;
            this.EnableScheme = false;
            this.Schemes = new SchemeList();
            this.HookFiles = new FileHookList();
        }
    }
}
