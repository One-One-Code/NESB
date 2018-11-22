using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.MQ.Consumer
{
    using NESB.MQ.MqCore;

    /// <summary>
    /// 消息处理完成后的操作
    /// </summary>
    public class AfterConsumer : IAfterConsumer
    {
        public void Execute(Event @event)
        {
            
        }
    }
}
