using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.MQ.Event
{
    using NESB.Model.Service;
    using NESB.MQ.MqCore;

    /// <summary>
    /// 服务开始注册的消息
    /// </summary>
    public class ServiceRegisterEvent: Event
    {
        /// <summary>
        /// 服务信息
        /// json格式
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 服务类型
        /// </summary>
        public ServiceType ServiceType { get; set; }
    }
}
