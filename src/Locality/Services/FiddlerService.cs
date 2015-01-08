﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fiddler;
using System.IO;

namespace Locality
{
    public abstract class FiddlerService : IFiddlerExtension, IAutoTamper
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
                Uri uri = new System.Uri(oSession.fullUrl);
                if (ConfigService.EnableScheme)
                {
                    //模式匹配，符合域名规则的请求才挂载
                    if (!SchemeService.IsMatch(uri.Host))
                    {
                        return;
                    }
                }


                string filePath = uri.AbsolutePath; //得到如“/api.do”的字符串
                string fileName = Path.GetFileName(filePath).ToLower();  // file.ext
                string localPath = string.Empty;

                if (string.IsNullOrEmpty(fileName)) return; //文件名为空，直接跳过

                if (ConfigService.StrictMode)
                {
                    filePath = filePath.Replace("/", "\\").ToLower(); //将URL的左斜杠替换成文件目录形式的右斜杠
                    localPath = FileService.Exist(filePath, true);
                }
                else
                {
                    localPath = FileService.Exist(fileName);
                }

                if (!string.IsNullOrEmpty(localPath))
                {
                    //标记颜色
                    oSession["ui-color"] = ConfigService.Color;
                    oSession["ui-backcolor"] = ConfigService.BgColor;

                    oSession.utilCreateResponseAndBypassServer();
                    oSession.LoadResponseFromFile(localPath);

                    //匹配成功后调用通知事件
                    OnMatchSession(fileName);
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
