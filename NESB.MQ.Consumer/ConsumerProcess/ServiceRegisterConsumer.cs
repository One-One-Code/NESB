using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.MQ.Consumer.ConsumerProcess
{
    using Newtonsoft.Json;

    using NESB.Model.Service;
    using NESB.MQ.Event;
    using NESB.MQ.MqCore;

    /// <summary>
    /// 服务注册消费者处理逻辑
    /// </summary>
    [QueueConsumerAttribution("ServiceRegisterQueue")]
    public class ServiceRegisterConsumer : ConsumerBase<ServiceRegisterEvent>
    {
        protected override void Receive(ServiceRegisterEvent eEvent)
        {
            if (eEvent.ServiceType == ServiceType.Rest)
            {
                var info = JsonConvert.DeserializeObject<RestServiceInfo>(eEvent.Info);
            }

            throw new NotImplementedException();
        }
    }
}
