using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.SM.ZooKeeperCore
{
    using ZooKeeperNet;

    /// <summary>
    /// 基于ZooKeeper实现的分布式锁
    /// 主体思路
    /// 1. 在locks节点下创建临时顺序节点node_n
    /// 2. 判断当前创建的节点是否为locks节点下所有子节点中最小的子节点
    /// 3. 是则获取锁，进行业务处理，否则将节点从小到大排序，监听当前节点上一个节点的删除事件
    /// 4. 事件触发后回到步骤2进行判断，直至拿到锁
    /// </summary>
    public class DistributedLock : IWatcher, IDisposable
    {
        private const string LockRootName = "/locks";
        private const int SessionTimeout = 300000;
        private ZooKeeper zooKeeper;

        /// <summary>
        /// 锁节点名字
        /// </summary>
        private string lockName;

        /// <summary>
        /// 当前id
        /// </summary>
        private string currentId;

        /// <summary>
        /// 拿不到锁的时候等待的前一个id
        /// </summary>
        private string waitId;

        private bool connected = false;

        public DistributedLock(ZooKeeper zooKeeper)
        {
            this.zooKeeper = zooKeeper;
            var stat = this.zooKeeper.Exists(LockRootName, false);
            if (stat == null)
            {
                this.zooKeeper.Create(LockRootName, null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }
        }

        /// <summary>
        /// 获取锁
        /// </summary>
        /// <returns></returns>
        public bool GetLock(bool create = true)
        {
            if (this.currentId == null && create)
            {
                this.currentId = this.zooKeeper.Create(LockRootName + "/" + this.lockName + "_", new byte[0], Ids.OPEN_ACL_UNSAFE, CreateMode.EphemeralSequential);
            }
            var childrens = this.zooKeeper.GetChildren(LockRootName, false);
            if (childrens == null || childrens.Count() == 1)
            {
                return true;
            }
            var orderChildrens = childrens.OrderBy(p => p).ToList();
            var index = orderChildrens.FindIndex(p => p.Equals(this.currentId.Replace(LockRootName + "/", "")));
            if (index == 0)
            {
                return true;
            }
            this.waitId = LockRootName + "/" + orderChildrens[index - 1];
            var stat = this.zooKeeper.Exists(this.waitId, this);
            if (stat == null)
            {
                this.GetLock(false);
            }
            return false;
        }

        /// <summary>
        /// 释放锁
        /// 删除节点
        /// </summary>
        public void UnLock()
        {
            if (this.currentId == null)
            {
                return;
            }
            this.zooKeeper.Delete(this.currentId, -1);
            this.currentId = null;
        }

        /// <summary>
        /// 获取当前的节点id
        /// </summary>
        /// <returns></returns>
        public string GetCurrentId()
        {
            return this.currentId;
        }

        /// <summary>
        /// 监听方法
        /// 1. 主要监听删除节点操作
        /// 2. 监听是否建立连接
        /// </summary>
        /// <param name="event"></param>
        public void Process(WatchedEvent @event)
        {
            if (KeeperState.SyncConnected == @event.State)
            {
                this.connected = true;
            }

            if (@event.Type == EventType.NodeDeleted)
            {
                this.GetLock(false);
            }
        }

        public void Dispose()
        {
            this.zooKeeper.Dispose();
        }
    }
}
