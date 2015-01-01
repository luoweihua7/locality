using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Locality
{
    public class SchemeService
    {
        public static SchemeList schemeList = new SchemeList();
        private static SchemeList enableList = new SchemeList();

        /// <summary>
        /// 添加用户配置
        /// </summary>
        /// <param name="scheme">配置实体</param>
        public static void Add(Scheme scheme)
        {
            schemeList.Add(scheme);

            if (scheme.Enable == true)
            {
                enableList.Add(scheme);
            }
        }

        /// <summary>
        /// 删除用户配置
        /// </summary>
        /// <param name="scheme">配置实体</param>
        public static void Remove(Scheme scheme)
        {
            schemeList.Remove(scheme);
            enableList.Remove(scheme);
        }

        /// <summary>
        /// 域名是否匹配
        /// </summary>
        /// <param name="host">地址</param>
        /// <returns></returns>
        public static bool IsMatch(string host)
        {


            return false;
        }
    }
}
