using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Locality
{
    public class ConfigService
    {
        /// <summary>
        /// 配置的保存目录
        /// </summary>
        public static string ConfigDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Fiddler2";

        /// <summary>
        /// 配置文件的保存路径
        /// </summary>
        public static string ConfigPath = ConfigDir + @"\locality.dat";

        /// <summary>
        /// 右下角窗口的显示时间
        /// <para>超过显示时间会隐藏窗口</para>
        /// </summary>
        public static long DueTime = 3000;

        /// <summary>
        /// 命中规则后,Fiddler请求列表中的request项字体颜色
        /// </summary>
        public static string Color = "#000000";

        /// <summary>
        /// 命中规则后,列表中的请求项背景色
        /// </summary>
        public static string BgColor = "#e9e9e9";

        /// <summary>
        /// 插件名称
        /// </summary>
        public static string AppName = "Locality";

        /// <summary>
        /// 启用时的图标名称
        /// <para>用于控制Fiddler中的图标显示</para>
        /// </summary>
        public static string ActiveIcon = AppName + "_ACTIVE";

        /// <summary>
        /// 禁用时的图标名称
        /// <para>用于控制Fiddler中的图标显示</para>
        /// </summary>
        public static string InactiveIcon = AppName + "_INACTIVE";

        /// <summary>
        /// 插件是否启用
        /// </summary>
        public static bool Enable
        {
            get { return _config.Enable; }
            set { _config.Enable = value; }
        }

        /// <summary>
        /// 是否显示右下角提示窗口
        /// </summary>
        public static bool EnableTip
        {
            get { return _config.EnableTip; }
            set { _config.EnableTip = value; }
        }

        /// <summary>
        /// 是否启用路径的严格模式
        /// <para>启用之后，只有在挂载文件严格匹配URL地址路径的情况下才会返回</para>
        /// <para>即单个文件只有在无目录被挂载的情况下才会命中</para>
        /// </summary>
        public static bool StrictMode
        {
            get { return _config.StrictMode; }
            set { _config.StrictMode = value; }
        }

        /// <summary>
        /// 是否启用场景模式
        /// </summary>
        public static bool EnableScheme
        {
            get { return _config.EnableScheme; }
            set { _config.EnableScheme = value; }
        }

        public static SchemeList Schemes
        {
            get { return _config.Schemes; }
            set { _config.Schemes = value; }
        }

        public static HookCollection Files
        {
            get { return _config.Files; }
            set { _config.Files = value; }
        }

        /// <summary>
        /// 配置项
        /// </summary>
        private static Config _config;
        /// <summary>
        /// 序列化保存用的数据格式化类
        /// </summary>
        private static BinaryFormatter formatter = new BinaryFormatter();

        /// <summary>
        /// 从文件中读取配置信息
        /// <para>如果读取失败，则使用默认配置</para>
        /// </summary>
        public static void Load()
        {
            Config config = new Config();

            if (File.Exists(ConfigPath))
            {
                FileStream fs = new FileStream(ConfigPath, FileMode.Open);
                try
                {
                    formatter.Binder = new ConfigBinder(); //反序列化的时候需要有一个转换器
                    config = (Config)formatter.Deserialize(fs);
                }
                catch (Exception err)
                {
                    LogService.Log(err.Message);
                }
                finally
                {
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                }

                LogService.Log("Config loaded");
            }

            _config = config;
        }

        /// <summary>
        /// 保存配置到文件
        /// </summary>
        public static void Save()
        {
            //从Configs中读取配置并序列化保存到文件
            Config config = new Config()
            {
                Enable = _config.Enable,
                EnableTip = _config.EnableTip,
                StrictMode = _config.StrictMode,
                EnableScheme = _config.EnableScheme,
                Schemes = SchemeService.Get(),
                Files = FileService.GetHookCollection()
            };

            if (!Directory.Exists(ConfigDir))
            {
                Directory.CreateDirectory(ConfigDir);
            }

            FileStream fs = new FileStream(ConfigPath, FileMode.Create); //创建or覆盖
            try
            {
                formatter.Serialize(fs, config);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.Message);
            }
            finally
            {
                fs.Flush();
                fs.Close();
                fs.Dispose();
            }
        }
    }

    public class ConfigBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetType(typeName);
        }
    }
}
