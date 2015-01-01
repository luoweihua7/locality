using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fiddler;
using System.IO;

namespace Locality
{
    public class FiddlerService : IFiddlerExtension, IAutoTamper
    {
        /// <summary>
        /// 挂载命中处理后的逻辑
        /// <para>如弹窗提示挂载之类的</para>
        /// </summary>
        /// <param name="fileName">命中的文件名</param>
        public virtual void OnMatchSession(string fileName)
        {

        }

        public virtual void OnBeforeUnload()
        {
            //保存配置
            ConfigService.Save();
        }

        public virtual void OnLoad()
        {

        }

        public virtual void AutoTamperRequestAfter(Session oSession)
        {

        }

        public virtual void AutoTamperRequestBefore(Session oSession)
        {
            if (ConfigService.Enable)
            {
                string filePath = new System.Uri("http://127.0.0.1" + oSession.url).AbsolutePath; //得到如“/api.do”的字符串
                string fileName = Path.GetFileName(filePath).ToLower();  // file.ext
                string path = string.Empty;

                if (string.IsNullOrEmpty(fileName)) return; //文件名为空，直接跳过
                
                if (ConfigService.StrictMode)
                {
                    filePath = filePath.Substring(1).Replace("/", "\\"); //去掉第一个左斜杠，并将URL的左斜杠替换成文件目录形式的右斜杠
                    path = FileService.Exist(filePath, true);
                }
                else
                {
                    path = FileService.Exist(fileName);
                }
            }
        }

        public virtual void AutoTamperResponseAfter(Session oSession)
        {

        }

        public virtual void AutoTamperResponseBefore(Session oSession)
        {

        }

        public virtual void OnBeforeReturningError(Session oSession)
        {

        }
    }
}
