/*
 * 作者:黄平
 * 创建时间:2014-11-24
 * ------------------修改记录-------------------
 * 修改人      修改日期        修改目的
 * 黄平        2014-11-24      创建
 */

namespace NESB.MQ.MqCore
{
    public interface IConsumer
    {
    }

    public interface IEventConsumer<in T> : IConsumer
        where T : Event
    {
        void Consume(T eEvent);
    }
}
