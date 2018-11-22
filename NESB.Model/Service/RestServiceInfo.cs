using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.Model.Service
{
    using ProtoBuf;

    /// <summary>
    /// http服务的基本信息
    /// </summary>
    [ProtoContract]
    public class RestServiceInfo : ServiceBaseInfo
    {
        /// <summary>
        /// 使用的协议
        /// </summary>
        [ProtoMember(1)]
        public string Protocol { get; set; }
    }
}
