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
using Magnum.Reflection;
using MassTransit;
using MassTransit.Configurators;
using MassTransit.SubscriptionConfigurators;
using Microsoft.Practices.ServiceLocation;
using RuanYun.IoC;
using RuanYun.Logger;

namespace NESB.MQ.MqCore
{
    /// <summary>
    /// Event类型事件的消费者的配置
    /// </summary>
    public class DefaultConsumeConfigurator : IConsumeConfigurator
    {
        /// <summary>
        /// ioc注册对象
        /// </summary>
        private readonly IRegistration _reg;

        /// <summary>
        /// ioc定位器
        /// </summary>
        private readonly IServiceLocator _locator;

        /// <summary>
        /// 接收消息之前执行的对象
        /// </summary>
        private IBeforeConsumer _beforeConsumer;

        /// <summary>
        /// 接收消息之后执行的对象
        /// </summary>
        private IAfterConsumer _afterConsumer;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reg">ioc注册对象</param>
        /// <param name="locator">ioc定位器</param>
        public DefaultConsumeConfigurator(IRegistration reg, IServiceLocator locator)
        {
            _reg = reg;
            _locator = locator;
        }

        /// <summary>
        /// 初始化aop
        /// </summary>
        private void InitAOPConsumer()
        {
            try
            {
                _beforeConsumer = _locator.GetInstance<IBeforeConsumer>();
            }
            catch (Exception)
            {
            }

            try
            {
                _afterConsumer = _locator.GetInstance<IAfterConsumer>();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 配置consumer
        /// </summary>
        /// <param name="cfg">配置</param>
        /// <param name="consumers">consumer类型集合</param>
        public void Configure(SubscriptionBusServiceConfigurator cfg, List<Type> consumers)
        {
            InitAOPConsumer();
            Log.Write(string.Format("消费类型{0}个", consumers.Count), MessageType.Info);
            foreach (var consumer in consumers)
            {
                Log.Write(consumer.FullName, MessageType.Debug);
                if (consumer.IsGenericType)
                {
                    Log.Write(string.Format("跳过{0}因为是泛型", consumer.FullName), MessageType.Warn);
                    continue;
                }
                var consumerTypes = consumer.GetInterfaces()
                    .Where(d => d.IsGenericType && d.GetGenericTypeDefinition() == typeof(IEventConsumer<>))
                    .Select(d => d.GetGenericArguments().Single())
                    .Distinct();


                foreach (var eventConsumerType in consumerTypes)
                {
                    try
                    {
                        Log.Write(string.Format("{0} map {1}", consumer.FullName, eventConsumerType.FullName),
                                  MessageType.Debug);
                        var type = consumer;
                        this.FastInvoke(new[] { eventConsumerType, consumer },
                                        x => x.ConsumerTo<Event, IEventConsumer<Event>>(cfg, type), cfg,
                                        consumer);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex.Message, MessageType.Error, this.GetType(), ex);
                    }
                }
            }
            cfg.Success("配置Consumer成功");
        }

        /// <summary>
        /// 添加consumer
        // 消费者停止消费消息后，将异常抛出，让消息回滚至原队列中
        // 等待下次处理
        /// </summary>
        /// <typeparam name="TEvent">consumer的接收事件参数</typeparam>
        /// <typeparam name="TConsumer">consumer类型</typeparam>
        /// <param name="cfg">配置对象</param>
        /// <param name="handlerType">处理类型</param>
        protected void ConsumerTo<TEvent, TConsumer>(SubscriptionBusServiceConfigurator cfg, Type handlerType)
            where TConsumer : IEventConsumer<TEvent>
            where TEvent : Event
        {
            cfg.Handler<TEvent>(evnt =>
                {
                    try
                    {
                        if (_beforeConsumer != null)
                            _beforeConsumer.Execute(evnt);
                        _locator.GetInstance<TConsumer>().Consume(evnt);
                        if (_afterConsumer != null)
                            _afterConsumer.Execute(evnt);
                    }
                    catch (StopedConsumeException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Log.Write(string.Format("执行{0}错误", typeof(TConsumer)), MessageType.Error, this.GetType(), ex);
                    }
                }).Permanent();
        }
    }
}
