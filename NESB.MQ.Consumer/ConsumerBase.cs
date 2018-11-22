using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.MQ.Consumer
{
    using System.Threading;

    using Newtonsoft.Json;

    using NESB.MQ.MqCore;

    using RuanYun.Logger;

    /// <summary>
    /// 泛型消费者基类
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class ConsumerBase<TEvent> : IEventConsumer<TEvent>
        where TEvent : Event
    {
        private bool _supportStop = true;
        /// <summary>
        /// 支持停止消费
        /// 默认都支持
        /// </summary>
        protected bool SupportStop
        {
            get { return _supportStop; }
            set { _supportStop = value; }
        }

        #region IEventConsumer<TEvent> 成员

        /// <summary>
        /// 执行消息消费的方法
        /// <para>如果程序暂停，则所有消费者将在此等待，等程序继续或停止程序时才能继续后续逻辑</para>
        /// <para>如果程序停止，则所有消费者线程执行休眠，让消息重新回到原消息队列中</para>
        /// </summary>
        /// <param name="eEvent">当前队列的消息</param>
        public void Consume(TEvent eEvent)
        {
            GlobalFeatureToggleHelper.PauseToggle.WaitOne();
            if (_supportStop && GlobalFeatureToggleHelper.StopToggle.IsOn())
            {
                Log.Write(string.Format("服务已停止，接收到新消息，拒绝处理，EventType:{0}，Event：{1}", typeof(TEvent).Name, JsonConvert.SerializeObject(eEvent)), MessageType.Warn);
                // 消费者休眠时间要稍大于服务主线程休眠时间
                // 能执行到移除抛出，消息会回到原队列
                // Sleep过程线程终止，消息也会回到原队列
                Thread.Sleep(5 * 1000);
                throw new StopedConsumeException();
            }

            try
            {
                Receive(eEvent);
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("消息处理出错，Event:{0}", GetType().FullName), MessageType.Error, GetType(), ex);
                throw;
            }
        }

        #endregion

        /// <summary>
        /// 消费者接收消息
        /// </summary>
        /// <param name="eEvent">消息体</param>
        protected abstract void Receive(TEvent eEvent);
    }
}
