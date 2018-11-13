/*
 * 作者:黄平
 * 创建时间:2014-11-24
 * ------------------修改记录-------------------
 * 修改人      修改日期        修改目的
 * 黄平        2014-11-24      创建
 */
using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
using Microsoft.Practices.ServiceLocation;
using RuanYun.IoC;

namespace NESB.MQ.MqCore
{
    public static class SubscriptionAdapt
    {
        public static IBusConfiguration BusOnRabbitMq(this IRegistration reg, Action<IBusConfiguration> config)
        {
            var busConfig = new BusConfiguration(reg);
            config(busConfig);
            return busConfig;
        }
    }

    public class BusConfiguration : IBusConfiguration
    {
        private readonly IRegistration _reg;

        public BusConfiguration(IRegistration reg)
        {
            _reg = reg;
        }

        /// <summary>
        /// 需要发送Event时
        /// </summary>
        /// <param name="host"></param>
        /// <param name="queue"></param>
        /// <param name="concurrentConsumers"></param>
        public void PublishAt(string host, string queue, int concurrentConsumers)
        {
            var endpoint = string.Format("rabbitmq://{0}/{1}", host, queue);
            var rabbitEndpoint = new[] { host };
            var bus = ServiceBusFactory.New(sbc =>
            {
                sbc.ReceiveFrom(endpoint);

                sbc.UseRabbitMq(
                    x =>
                    {
                        x.UseRoundRobinConnectionPolicy(rabbitEndpoint);
                        x.PersistMessages(true);
                    }
                );

                sbc.SetConcurrentConsumerLimit(concurrentConsumers);

            });

            var publisher = new EventPublisher(bus);
            _reg.Inject<IEventPublisher>(publisher);
        }

        /// <summary>
        /// 需要消费Event
        /// </summary>
        /// <param name="host">服务器ip</param>
        /// <param name="queue">Queue名称</param>
        /// <param name="concurrentConsumers">消费者数量</param>
        /// <param name="configurator">消费者的配置对象</param>
        public void SubscribeAt(string host, string queue, int concurrentConsumers, IConsumeConfigurator configurator)
        {
            var endpoint = string.Format("rabbitmq://{0}/{1}", host, queue);
            var rabbitEndpoint = new[] { host };
            var consumers = GetQueueConsumers(queue);
            var bus = ServiceBusFactory.New(sbc =>
            {
                sbc.ReceiveFrom(endpoint);
                
                sbc.UseRabbitMq(
                    x =>
                    {
                        x.UseRoundRobinConnectionPolicy(rabbitEndpoint);
                        x.PersistMessages(true);
                    }
                );

                sbc.SetConcurrentConsumerLimit(concurrentConsumers);
                sbc.Subscribe(x => configurator.Configure(x, consumers));
            });

            //var publisher = new EventPublisher(bus);
            //_reg.Inject<IEventPublisher>(publisher);
           
            // 记录消费者服务总线对象 
            var consumerBus = new EventConsumerBus(bus);
            _reg.Inject<IEventConsumerBus>(consumerBus);
        }

        /// <summary>
        /// 需要消费Event
        /// </summary>
        /// <param name="host">队列配置对象</param>
        /// <param name="configurator"></param>
        public void SubscribeAt(SubscribeHost host, IConsumeConfigurator configurator)
        {
            if(host == null)
                throw new ArgumentNullException("host");
            SubscribeAt(host.Host, host.QueueName, host.CunsumerNum, configurator);
        }

        /// <summary>
        /// 根据Queue的名字查找相应的消费Class
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        private List<Type> GetQueueConsumers(string queueName)
        {
            var result = new List<Type>();
            var a = ServiceLocator.Current.GetAllInstances<IConsumer>();
            var consumers =_reg.GetAllImplementingRegisteredTypesOf<IConsumer>(x => !x.IsGenericType).ToList();
            foreach (var consumer in consumers)
            {
                var attributions = consumer.GetCustomAttributes(typeof(QueueConsumerAttribution), false);
                if (attributions.Length <= 0)
                {
                    result.Add(consumer);
                    continue;
                }
                var attribution = attributions[0] as QueueConsumerAttribution;
                if (attribution == null || attribution.QueueName.Equals(queueName, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(consumer);
                }
            }
            return result;
        }
    }



    public interface IBusConfiguration
    {
        void SubscribeAt(string host, string queue, int concurrentConsumers, IConsumeConfigurator configurator);
        void SubscribeAt(SubscribeHost host, IConsumeConfigurator configurator);
        void PublishAt(string host, string queue, int concurrentConsumers);
    }

}
