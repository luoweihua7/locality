using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Locality
{
    public class FileService
    {
        /// <summary>
        /// 挂载文件列表
        /// </summary>
        private static FileHookCollection fileHookList = new FileHookCollection();

        /// <summary>
        /// 初始化文件挂载列表数据
        /// </summary>
        /// <param name="coll"></param>
        public static void Initialize(HookCollection coll)
        {
            coll.ForEach(hook =>
            {
                Add(hook.Path, hook.Enable);
            });
        }

        /// <summary>
        /// 添加文件到挂载列表
        /// <para>如果已经存在列表中，则标记为启用</para>
        /// </summary>
        /// <param name="filePath">文件或者文件夹路径</param>
        /// <param name="enable">是否挂载</param>
        public static void Add(string filePath, bool enable = true)
        {
            FileHook fileHook = Exists(filePath);
            if (fileHook == null)
            {
                fileHook = new FileHook(filePath, enable);
                Add(fileHook);
            }
            else
            {
                fileHook.Enable = enable;
            }
        }

        /// <summary>
        /// 添加挂载对象到挂载列表
        /// </summary>
        /// <param name="fileHook"></param>
        public static void Add(FileHook fileHook)
        {
            fileHookList.Add(fileHook);
        }

        /// <summary>
        /// 从挂载列表中删除挂载对象
        /// </summary>
        /// <param name="fileHook"></param>
        public static void Remove(FileHook fileHook)
        {
            fileHook.Enable = false; //禁用掉文件目录的变化监听
            fileHookList.Remove(fileHook);
        }

        /// <summary>
        /// 删除挂载的文件或者文件夹
        /// </summary>
        /// <param name="filePath"></param>
        public static void Remove(string filePath)
        {
            var fileHook = fileHookList.FirstOrDefault(item =>
            {
                if (item.Path == filePath) return true;
                return false;
            });

            if (fileHook != null)
            {
                fileHook.Destory();
                Remove(fileHook);
            }
        }

        /// <summary>
        /// 设置挂载对象为启用或禁用
        /// </summary>
        /// <param name="path">挂载中的文件路径</param>
        /// <param name="enable">是否启用</param>
        public static void Update(string path, bool enable)
        {
            FileHook fileHook = Exists(path);
            if (fileHook != null)
            {
                fileHook.Enable = enable;
            }
        }

        /// <summary>
        /// 清理所有文件挂载列表
        /// </summary>
        public static void Clear()
        {
            fileHookList.ForEach(fileHook =>
            {
                fileHook.Enable = false; //将所有标记清除，以停止文件更新事件
            });
            fileHookList.Clear();
        }

        /// <summary>
        /// 在挂载列表中查找文件
        /// </summary>
        /// <param name="fileName">需要查找的文件名称</param>
        /// <param name="strictMode">是否严格模式<para>严格模式匹配相对路径，且优先匹配挂载的文件夹，在无匹配的情况下再尝试匹配挂载的文件</para></param>
        /// <returns></returns>
        public static string Exist(string fileName, bool strictMode = false)
        {
            string filePath = string.Empty;

            if (strictMode)
            {
                /**
                 * 匹配文件过程如下：
                 * 严格模式匹配引入了匹配度的概念
                 * 遍历数据时，获取最大匹配度的一个文件，即匹配路径与挂载文件下的路径最符合的文件
                 * 在严格匹配无结果的情况下，通过文件名匹配符合条件的文件
                 * 为了解决在第一次便利数据时无匹配的条件下，需要遍历第二次的问题，所以在遍历第一次的时候，就尝试匹配文件名
                 */

                //严格模式下，fileName是URL的pathname
                List<string> single = new List<string>();
                string name = UtilService.GetName(fileName); //获取文件名，在严格路径匹配不到时，或者可以通过匹配文件名得到
                double matchQuality = 0; //引入匹配度概念，全部匹配，获取最大匹配度的文件
                string bestMatch = string.Empty;
                string fileMatch = string.Empty; //文件名匹配,即匹配挂载的单文件
                string searchName = Path.GetFileName(fileName);

                fileHookList.ForEach(fileHook =>
                {
                    var files = fileHook.Files;
                    var dir = fileHook.Path;

                    if (!fileHook.Enable) return;
                    if (fileHook.Type != HookType.Folder)
                    {
                        if (string.IsNullOrEmpty(fileMatch) && Path.GetFileName(fileHook.Path) == searchName)
                        {
                            //顺便匹配出单文件，减少遍历成本
                            if (File.Exists(fileHook.Path))
                            {
                                fileMatch = fileHook.Path;
                            }
                        }
                        return;
                    }

                    files.ForEach(file =>
                    {
                        if (Path.GetFileName(file) == searchName && fileName.EndsWith(file)) //先决条件：文件名必须匹配
                        {
                            double quality = ((double)file.Length) / fileName.Length;
                            if (quality > matchQuality)
                            {
                                //匹配度较高时，保存路径和匹配度
                                string fullPath = dir + file;
                                if (File.Exists(fullPath)) //FileSystemWatcher无法监听到“被监听目录”被删除的事件，只能监听到“被监听目录”下的子目录和文件的变更事件
                                {
                                    matchQuality = quality;
                                    bestMatch = fullPath; //恢复完整路径
                                }
                            }
                        }
                    });
                });

                if (!string.IsNullOrEmpty(bestMatch))
                {
                    filePath = bestMatch; //取最佳匹配
                }
                else
                {
                    //最佳匹配无结果时，匹配单文件的挂载
                    filePath = fileMatch;
                }
            }
            else
            {
                foreach (var fileHook in fileHookList)
                {
                    if (!fileHook.Enable) continue;

                    var files = fileHook.Files; //临时对象指向列表，防止在查询期间有数据更新。
                    var path = files.FirstOrDefault(file =>
                    {
                        //文件名必须匹配，所以不使用EndsWith
                        return Path.GetFileName(file) == Path.GetFileName(fileName);
                    });

                    if (!string.IsNullOrEmpty(path))
                    {
                        if (fileHook.Type == HookType.File)
                        {
                            if (!File.Exists(fileHook.Path)) continue;

                            //文件挂载，Path就是完整路径
                            filePath = fileHook.Path;

                        }
                        else
                        {
                            //目录挂载，拼接完整模式
                            filePath = fileHook.Path + path;
                        }
                        break;
                    }
                }
            }

            return filePath;
        }

        /// <summary>
        /// 保存配置时，仅获取基础字段
        /// </summary>
        /// <returns></returns>
        public static HookCollection GetHookCollection()
        {
            HookCollection coll = new HookCollection();
            fileHookList.ForEach(fileHook =>
            {
                coll.Add(new Hook()
                {
                    Enable = fileHook.Enable,
                    Name = fileHook.Name,
                    Path = fileHook.Path,
                    Type = fileHook.Type
                });
            });

            return coll;
        }

        /// <summary>
        /// 判断文件是否已经存在于挂载列表中
        /// </summary>
        /// <param name="path">文件的完整路径</param>
        /// <returns></returns>
        private static FileHook Exists(string path)
        {
            FileHook fileHook = null;

            foreach (var item in fileHookList)
            {
                if (item.Path == path)
                {
                    fileHook = item;
                    break;
                }
            }
            return fileHook;
        }
    }
}
