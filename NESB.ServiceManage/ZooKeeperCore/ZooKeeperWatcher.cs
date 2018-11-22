using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.SM.ZooKeeperCore
{
    using ZooKeeperNet;

    /// <summary>
    /// 监视器
    /// </summary>
    public class ZooKeeperWatcher : IWatcher
    {
        private readonly ZooKeeperBus bus;
        public ZooKeeperWatcher(ZooKeeperBus bus)
        {
            this.bus = bus;
        }

        /// <summary>
        /// 监视器处理逻辑
        /// </summary>
        /// <param name="event"></param>
        public void Process(WatchedEvent @event)
        {
            if (KeeperState.SyncConnected == @event.State)
            {
                this.bus.SetConnected();
            }
        }
    }
}
