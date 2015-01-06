using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Locality
{
    public class SchemeService
    {
        public static SchemeList schemeList = new SchemeList();

        /// <summary>
        /// 添加用户配置
        /// </summary>
        /// <param name="scheme">配置实体</param>
        public static void Add(Scheme scheme)
        {
            schemeList.Add(scheme);
        }

        public static void Add(string schemeName, List<string> hosts, bool enable = true)
        {
            Add(new Scheme()
            {
                Name = schemeName,
                Hosts = hosts,
                Enable = enable
            });
        }

        /// <summary>
        /// 删除用户配置
        /// </summary>
        /// <param name="scheme">配置实体</param>
        public static void Remove(Scheme scheme)
        {
            schemeList.Remove(scheme);
        }

        /// <summary>
        /// 根据文件名，删除模式设置
        /// </summary>
        /// <param name="schemeName"></param>
        public static void Remove(string schemeName)
        {
            Scheme scheme = Find(schemeName);

            if (scheme != null)
            {
                schemeList.Remove(scheme);
            }
        }

        /// <summary>
        /// 设置启用或者禁用设置
        /// </summary>
        /// <param name="schemeName"></param>
        /// <param name="enable"></param>
        public static void Update(string schemeName, bool enable)
        {
            Scheme scheme = Find(schemeName);

            if (scheme != null)
            {
                scheme.Enable = enable;
            }
        }

        public static void Update(string schemeName, List<string> hosts, bool enable)
        {
            Update(schemeName, schemeName, hosts, enable);
        }

        public static void Update(string oldName, string newName, List<string> hosts, bool enable)
        {
            Scheme scheme = Find(oldName);

            if (scheme != null)
            {
                scheme.Name = newName;
                scheme.Hosts = hosts;
                scheme.Enable = enable;
            }
        }

        /// <summary>
        /// 查找模式是否存在
        /// </summary>
        /// <param name="schemeName">设置名称</param>
        /// <returns></returns>
        public static Scheme Find(string schemeName)
        {
            return schemeList.FirstOrDefault(item =>
            {
                return item.Name == schemeName;
            });
        }

        /// <summary>
        /// 返回配置列表
        /// </summary>
        /// <returns></returns>
        public static SchemeList Get()
        {
            return schemeList;
        }

        /// <summary>
        /// 域名是否匹配
        /// </summary>
        /// <param name="host">地址</param>
        /// <returns></returns>
        public static bool IsMatch(string hostName)
        {
            var match = schemeList.FirstOrDefault(scheme =>
            {
                if (!scheme.Enable) return false; //禁用的场景跳过

                var hosts = scheme.Hosts;
                var item = hosts.FirstOrDefault(host =>
                {
                    return host == hostName;
                });

                if (item != null)
                {
                    return true;
                }
                return false;
            });

            return match != null; //match!=null时表示有匹配的数据
        }
    }
}
