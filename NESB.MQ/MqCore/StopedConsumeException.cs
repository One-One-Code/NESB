/*----------------------------------------------------
 * 作者: 黄平
 * 创建时间: 2017-11-06
 * ------------------修改记录-------------------
 * 修改人      	修改日期        		修改目的
 ----------------------------------------------------*/

using System;

namespace NESB.MQ.MqCore
{
    /// <summary>
    /// 已停止消费异常
    /// </summary>
    public class StopedConsumeException : ApplicationException
    {
        public StopedConsumeException()
        {

        }

        public StopedConsumeException(string message)
            : base(message)
        {

        }
        public StopedConsumeException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
