/*----------------------------------------------------
 * 作者: 黄平
 * 创建时间: 2017-11-06
 * ------------------修改记录-------------------
 * 修改人      	修改日期        		修改目的
 ----------------------------------------------------*/

namespace NESB.MQ.MqCore
{
    /// <summary>
    /// 服务队列配置信息
    /// </summary>
    public class ServerQueueConfig
    {
        /// <summary>
        /// 服务器类型ID
        /// </summary>
        public int ServerTypeId { get; set; }

        /// <summary>
        /// rabbit.mq地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 消息队列名称
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// 使用状态
        /// </summary>
        public int StatusFlag { get; set; }
    }
}
