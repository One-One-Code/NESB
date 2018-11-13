using System;

namespace NESB.MQ.MqCore
{
    /// <summary>
    /// 队列和消费者匹配的Attribute
    /// 用于消费者类
    /// </summary>
    public class QueueConsumerAttribution:Attribute
    {
        public string QueueName { get { return _queueName; } }
        private string _queueName { get; set; }

        public QueueConsumerAttribution(string queueName)
        {
            _queueName = queueName;
        }

    }
}
