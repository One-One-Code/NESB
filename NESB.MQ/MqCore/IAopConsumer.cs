/*
 * 作者:黄平
 * 创建时间:2014-11-24
 * ------------------修改记录-------------------
 * 修改人      修改日期        修改目的
 * 黄平        2014-11-24      创建
 */

namespace NESB.MQ.MqCore
{
    /// <summary>
    /// Consumer接收消息之前执行的操作
    /// </summary>
    public interface IBeforeConsumer
    {
        /// <summary>
        /// 操作
        /// </summary>
        /// <param name="event">Consumer接收的数据对象</param>
        void Execute(Event @event);
    }

    /// <summary>
    /// Consumer接收消息并处理之后执行的操作
    /// </summary>
    public interface IAfterConsumer
    {
        /// <summary>
        /// 操作
        /// </summary>
        /// <param name="event">Consumer接收的数据对象</param>
        void Execute(Event @event);
    }
}
