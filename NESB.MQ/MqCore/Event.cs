/*
 * 作者:黄平
 * 创建时间:2014-11-24
 * ------------------修改记录-------------------
 * 修改人      修改日期        修改目的
 * 黄平        2014-11-24      创建
 * 黄平        2017-09-06      将该类移到ServiceBus类库以便复用整个ServiceBus
 */
using System;


namespace NESB.MQ.MqCore
{
    public interface IEvent
    {
        /// <summary>
        /// 事件唯一码
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; }

        /// <summary>
        /// 发送事件的服务器名
        /// </summary>
        string Machine { get; }

        /// <summary>
        /// 通过那台
        /// </summary>
        string FromApplication { get; }
    }

    /// <summary>
    /// 发送RabbitMQ的事件的基类
    /// </summary>
    public abstract class Event : IEvent
    {
        public static readonly string DefaultApplicationName =
            System.Configuration.ConfigurationManager.AppSettings["AppName"];

        public static readonly string DefaultMachineName = Environment.MachineName;

        protected Event()
        {
            Id = Guid.NewGuid();
            CreateTime = DateTime.Now;
            Machine = DefaultMachineName;
            FromApplication = DefaultApplicationName;
        }

        public Guid Id { get; set; }

       
        public DateTime CreateTime { get; set; }


        public string Machine { get; set; }

 
        public string FromApplication { get; set; }

        public int RequestFromType { get; set; }
    }
}
