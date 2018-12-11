using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.MQ.Consumer.ConsumerProcess
{
    using Newtonsoft.Json;

    using NESB.Model.Enum;
    using NESB.Model.Service;
    using NESB.MQ.Event;
    using NESB.MQ.MqCore;
    using NESB.SM;
    using NESB.SM.ZooKeeperCore;

    using RuanYun.Logger;

    /// <summary>
    /// 服务注册消费者处理逻辑
    /// </summary>
    [QueueConsumerAttribution("ServiceRegisterQueue")]
    public class ServiceRegisterConsumer : ConsumerBase<ServiceRegisterEvent>
    {
        /// <summary>
        /// 进行服务注册的监听方法
        /// </summary>
        /// <param name="eEvent"></param>
        protected override void Receive(ServiceRegisterEvent eEvent)
        {
            if (eEvent.Info == null)
            {
                return;
            }
            if (!eEvent.Info.Validate())
            {
                Log.Write(string.Format("服务注册消费消息异常，服务信息不完整,info:{0}", JsonConvert.SerializeObject(eEvent.Info)), MessageType.Warn);
                return;
            }
            ServiceManage.RegisterService(eEvent.Info, eEvent.RegisterType);
        }
    }
}
