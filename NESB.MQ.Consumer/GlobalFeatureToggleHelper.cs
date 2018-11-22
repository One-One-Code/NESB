using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.MQ.Consumer
{
    using System.Threading;

    using NESB.Model.Enum;

    /// <summary>
    /// 提供全局控制的开关切换帮助
    /// </summary>
    public static class GlobalFeatureToggleHelper
    {
        /// <summary>
        /// 暂停开关
        /// <para>用于暂停、继续操作</para>
        /// <para>默认为关闭，可继续执行</para>
        /// </summary>
        public readonly static ManualResetEvent PauseToggle = new ManualResetEvent(true);

        /// <summary>
        /// 停止开关
        /// <para>用于停止、重启操作</para>
        /// <para>默认为关闭，表示未停止</para>
        /// </summary>
        public static SwitchEnum StopToggle = SwitchEnum.Off;

        /// <summary>
        /// 是否为开启状态
        /// </summary>
        /// <param name="switchToggle">开关对象</param>
        public static bool IsOn(this SwitchEnum switchToggle)
        {
            return switchToggle == SwitchEnum.On;
        }
    }
}
