using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.Model.Service
{
    using ProtoBuf;

    [ProtoContract]
    [ProtoInclude(99,typeof(RestServiceInfo))]
    public class ServiceBaseInfo
    {
        /// <summary>
        /// 服务名字
        /// 用于服务查询
        /// </summary>
        [ProtoMember(1)]
        public string ServiceName { get; set; }

        /// <summary>
        /// 主机名
        /// </summary>
        [ProtoMember(2)]
        public string HostName { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        [ProtoMember(3)]
        public string Ip { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        [ProtoMember(4)]
        public string Port { get; set; }
    }
}
