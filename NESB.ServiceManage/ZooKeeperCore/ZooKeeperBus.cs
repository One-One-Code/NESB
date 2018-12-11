using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.SM.ZooKeeperCore
{
    using System.IO;

    using ProtoBuf;

    using ZooKeeperNet;

    /// <summary>
    /// ZooKeeper服务相关操作类
    /// </summary>
    public class ZooKeeperBus : IDisposable
    {
        /// <summary>
        /// ZooKeeper对象，注意Session超时时间 
        /// </summary>
        private ZooKeeper zooKeeper;

        /// <summary>
        /// 根节点名称
        /// </summary>
        private readonly string rootPath = "/root";

        /// <summary>
        /// 是否和服务建立连接，true表示已建立连接
        /// </summary>
        private bool Connected = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="server">服务器地址，ip加端口 如：127.0.0.1:2181</param>
        /// <param name="sessionTimeout">Session超时时间，单位秒 </param>
        /// <param name="root">根节点</param>
        /// <param name="watcher">监视器</param>
        public ZooKeeperBus(string server, int sessionTimeout, string root, IWatcher watcher)
        {
            this.rootPath = root;
            this.zooKeeper = new ZooKeeper(server, new TimeSpan(0, 0, 0, sessionTimeout), new ZooKeeperWatcher(this));
            while (!this.Connected)
            {

            }
            var stat = this.zooKeeper.Exists(this.rootPath, true);
            if (stat == null)
            {
                this.zooKeeper.Create(this.rootPath, null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ZooKeeperBus()
            : this("127.0.0.1:2181", 60, "/root", null)
        {

        }

        /// <summary>
        /// 设置和服务器已建立连接
        /// </summary>
        public void SetConnected()
        {
            this.Connected = true;
        }

        /// <summary>
        /// 给ZooKeeper节点添加数据
        /// 不加锁，如果是分布式系统请使用SetDataWithLock方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public void SetData<T>(string key, T obj)
        {
            var bytes = this.GetBytes(obj);
            var path = string.Format("{0}/{1}", this.rootPath, key);
            var stat = this.zooKeeper.Exists(path, true);
            if (stat == null)
            {
                this.zooKeeper.Create(path, null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            }
            this.zooKeeper.SetData(path, bytes, -1);
        }

        /// <summary>
        /// 给ZooKeeper节点添加数据
        /// 使用锁更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="lockTimeOut">获取锁的等待时间，单位秒</param>
        public void SetDataWithLock<T>(string key, T obj, int lockTimeOut = 3)
        {
            var distributeKeeper = new DistributedLock(this.zooKeeper);
            bool getLock = false;
            var inTime = DateTime.Now;
            while (!getLock)
            {
                if (DateTime.Now.Subtract(inTime).TotalSeconds > lockTimeOut)
                {
                    throw new Exception("获取锁超时");
                }
                getLock = distributeKeeper.GetLock();
            }
            this.SetData(key, obj);
            distributeKeeper.UnLock();
        }

        /// <summary>
        /// 获取节点的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetData<T>(string key)
        {
            var path = string.Format("{0}/{1}", this.rootPath, key);
            var stat = this.zooKeeper.Exists(path, true);
            if (stat == null)
            {
                return default(T);
            }
            var data = this.zooKeeper.GetData(path, true, null);
            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }

        }

        /// <summary>
        /// 获取对象的byte数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private byte[] GetBytes<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                ms.Flush();
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        public void Dispose()
        {
            this.zooKeeper.Dispose();

        }

        ~ZooKeeperBus()
        {
            this.zooKeeper.Dispose();
        }
    }
}
