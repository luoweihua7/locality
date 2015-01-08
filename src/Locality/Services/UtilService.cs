using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Locality
{
    /// <summary>
    /// 通用工具类
    /// </summary>
    public class UtilService
    {
        /// <summary>
        /// 通过目录获取目录下的所有文件
        /// </summary>
        /// <param name="folder">目录地址，要求全小写（Windows不区分大小写）</param>
        /// <param name="relative">是否相对路径，即不包含目录地址</param>
        /// <returns></returns>
        public static List<string> GetFiles(string folder, bool relative = false)
        {
            List<string> allFiles = new List<string>();

            if (Directory.Exists(folder))
            {
                //获取目录和子目录中的所有文件
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(folder);
                    FileInfo[] infos = dir.GetFiles("*.*", SearchOption.AllDirectories);
                    foreach (var info in infos)
                    {
                        var file = info.FullName.ToLower();
                        if (relative) file = file.Replace(folder, string.Empty); //获取相对路径
                        allFiles.Add(file); //URL不区分大小写
                    }
                }
                catch (Exception ex)
                {
                    LogService.Log("[ERROR]" + ex.Message);
                }
            }

            return allFiles;
        }

        /// <summary>
        /// 通过路径获取指向的文件或者文件夹
        /// </summary>
        /// <param name="path">文件或者文件夹的路径</param>
        /// <returns></returns>
        public static string GetName(string path)
        {
            var fileName = string.Empty;

            if (Directory.Exists(path))
            {
                DirectoryInfo info = new DirectoryInfo(path);
                fileName = info.Name;
            }

            if (File.Exists(path))
            {
                FileInfo info = new FileInfo(path);
                fileName = info.Name;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Path.GetFileName(path);
            }

            return fileName;
        }
    }
}
