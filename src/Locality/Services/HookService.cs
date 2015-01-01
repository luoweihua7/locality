using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Locality
{
    public class HookService
    {
        private static FileHookCollection fileList = new FileHookCollection();

        /// <summary>
        /// 添加挂载的文件或者目录
        /// </summary>
        /// <param name="filePath">文件或者目录的地址</param>
        public static void Add(string filePath, bool enable = true)
        {
            //检查看看是否已经在列表中
            FileHook fileHook = Get(filePath);

            if (fileHook != null)
            {
                //已经存在
                fileHook.Enable = true;
                return;
            }
            else
            {
                fileHook = new FileHook(filePath, enable);
                fileList.Add(fileHook);
            }
        }

        /// <summary>
        /// 移除挂载对象
        /// </summary>
        /// <param name="filePath"></param>
        public static void Remove(string filePath)
        {
            var fileHook = Get(filePath);
            if (fileHook != null)
            {
                fileList.Remove(fileHook);
            }
        }

        public static FileHook Get(string filePath)
        {
            //检查看看是否已经在列表中
            return fileList.FirstOrDefault(item =>
            {
                if (item.Path == filePath)
                {
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// 匹配文件，获取本地文件的文件路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string Match(string fileName)
        {


            return null;
        }
    }
}
