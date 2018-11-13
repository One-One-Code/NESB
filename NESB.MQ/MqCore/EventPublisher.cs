/*
 * 作者:黄平
 * 创建时间:2014-11-24
 * ------------------修改记录-------------------
 * 修改人      修改日期        修改目的
 * 黄平        2014-11-24      创建
 */
using System;
using MassTransit;
using RuanYun.Logger;

namespace NESB.MQ.MqCore
{
    public interface IEventPublisher : IDisposable
    {
        void Publish<T>(T evt) where T : Event;

        /// <summary>
        /// 往消息队列中发送消息
        /// </summary>
        /// <param name="message">发送消息对象，必须是继承自Event的类型</param>
        /// <param name="messageType">发送消息Event类型对象</param>
        void Publish(object message, Type messageType);
    }

    public class EventPublisher : IEventPublisher
    {
        /// <summary>
        /// 发送消息后执行的事件
        /// <para>可通过该事件注入保存发送消息的功能</para>
        /// </summary>
        public static event Action<Event> EventPublishAfter;

        public IServiceBus Bus { get; private set; }

        public EventPublisher(IServiceBus bus)
        {
            Bus = bus;
        }

        public void Publish<T>(T evt) where T : Event
        {
            Bus.Publish(evt);
            if (EventPublishAfter != null)
            {
                try
                {
                    EventPublishAfter(evt);
                }
                catch (Exception ex)
                {
                    Log.Write("发布消息后执行事件出错", MessageType.Error, GetType(), ex);
                }
            }
        }

        /// <summary>
        /// 往消息队列中发送消息
        /// </summary>
        /// <param name="message">发送消息对象，必须是继承自Event的类型</param>
        /// <param name="messageType">发送消息Event类型对象</param>
        public void Publish(object message, Type messageType)
        {
            if (messageType == null || !(message is Event))
            {
                return;
            }

            Bus.Publish(message, messageType);
            var @event = (Event)message;
            if (EventPublishAfter != null)
            {
                try
                {
                    EventPublishAfter(@event);
                }
                catch (Exception ex)
                {
                    Log.Write("发布消息后执行事件出错", MessageType.Error, GetType(), ex);
                }
            }
        }

        public void Dispose()
        {
            Bus.Dispose();
        }
    }
}
