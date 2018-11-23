using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.Model.Service
{
    using ProtoBuf;

    [ProtoContract]
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

        /// <summary>
        /// 使用的协议
        /// </summary>
        [ProtoMember(5)]
        public string Protocol { get; set; }

        /// <summary>
        /// 服务类型
        /// </summary>
        [ProtoMember(7)]
        public ServiceType ServiceType { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(ServiceName) || !System.Enum.IsDefined(typeof(ServiceType), ServiceType)
                                                  || string.IsNullOrEmpty(Ip))
            {
                return false;
            }

            return true;
        }
    }
}
