using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.Model.Service
{
    /// <summary>
    /// http服务的基本信息
    /// </summary>
    public class RestServiceInfo : ServiceBaseInfo
    {
        /// <summary>
        /// 使用的协议
        /// </summary>
        public string Protocol { get; set; }
    }
}
