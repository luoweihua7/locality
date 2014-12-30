using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locality
{
    [Serializable]
    public class Scheme
    {
        /// <summary>
        /// 方案名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public Boolean Enable { get; set; }

        /// <summary>
        /// 域名列表
        /// </summary>
        public List<String> Hosts { get; set; }
    }

    [Serializable]
    public class SchemeList : List<Scheme>
    {

    }
}
