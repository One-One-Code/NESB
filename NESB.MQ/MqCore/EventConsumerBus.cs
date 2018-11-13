/*
 * 作者:黄平
 * 创建时间:2014-11-24
 * ------------------修改记录-------------------
 * 修改人      修改日期        修改目的
 * 黄平        2014-11-24      创建
 */
using System;
using MassTransit;

namespace NESB.MQ.MqCore
{
    /// <summary>
    /// 提供消费者服务总线可操作方法
    /// </summary>
    public interface IEventConsumerBus : IDisposable
    {
    }

    /// <summary>
    /// 消费者服务总线
    /// </summary>
    public class EventConsumerBus : IEventConsumerBus
    {
        private bool _disposed;
        public IServiceBus Bus { get; private set; }

        public EventConsumerBus(IServiceBus bus)
        {
            Bus = bus;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if(disposing)
            {
                if(Bus!=null)
                    Bus.Dispose();
            }

            Bus = null;
            _disposed = true;
        }
    }
}
