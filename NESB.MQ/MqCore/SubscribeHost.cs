/*
 * 作者:黄平
 * 创建时间:2014-11-24
 * ------------------修改记录-------------------
 * 修改人      修改日期        修改目的
 * 黄平        2014-11-24      创建
 */
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Practices.ServiceLocation;
using RuanYun.IoC;
using RuanYun.Logger;
using RuanYun.Utility.Helper;

namespace NESB.MQ.MqCore
{
    /// <summary>
    /// RabbitMQ配置对象
    /// </summary>
    public class SubscribeHost
    {
        public string Host { get; set; }
        public string QueueName { get; set; }
        public int CunsumerNum { get; set; }
        public IConsumeConfigurator ConsumeConfigurator { get; set; }
    }

    public class SubscribeHostFactory
    {
        /// <summary>
        /// 通过配置文件进行queue配置
        /// </summary>
        /// <param name="configFile"></param>
        /// <param name="consumerCount">消费者数量</param>
        public static void QueueConfigure(string configFile, int consumerCount)
        {
            var hosts = GetSubscribeHost(configFile);
            if (hosts == null || hosts.Count == 0)
                return;
            var reg = ServiceLocator.Current.GetInstance<IRegistration>();
            foreach (var subscribeHost in hosts)
            {
                reg.BusOnRabbitMq(
           config =>
           config.SubscribeAt(subscribeHost.Host, subscribeHost.QueueName, consumerCount, new DefaultConsumeConfigurator(reg, ServiceLocator.Current)));
            }
        }

        /// <summary>
        /// 注册queue方法
        /// </summary>
        /// <param name="queueConfig">queue配置信息</param>
        /// <param name="consumerCount">消费者数量</param>
        public static void QueueConfigure(List<ServerQueueConfig> queueConfig, int consumerCount)
        {
            var hosts = GetSubscribeHost(queueConfig);
            if (hosts == null || hosts.Count == 0)
                return;
            var reg = ServiceLocator.Current.GetInstance<IRegistration>();
            foreach (var subscribeHost in hosts)
            {
                reg.BusOnRabbitMq(
           config =>
           config.SubscribeAt(subscribeHost.Host, subscribeHost.QueueName, consumerCount, new DefaultConsumeConfigurator(reg, ServiceLocator.Current)));
            }
        }
        /// <summary>
        /// 获取Queue在XML文件的配置
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        private static List<SubscribeHost> GetSubscribeHost(string configFile)
        {
            if (!File.Exists(configFile))
            {
                Log.Write(string.Format("未找到queue的配置文件：Path：{0}", configFile), MessageType.Warn);
                return null;
            }

            var inputString = FileHelper.FileToString(configFile);
            var xmlHelper = new XmlHelper(inputString);
            var qnode = xmlHelper.GetNode("Queues");
            if (qnode == null)
            {
                Log.Write(string.Format("配置文件格式错误：Path：{0}", configFile), MessageType.Warn);
                return null;
            }
            var queueNodes = qnode.ChildNodes;
            var hosts = new List<SubscribeHost>();
            foreach (XmlNode queueNode in queueNodes)
            {
                var node = new SubscribeHost
                {
                    Host = xmlHelper.GetValue(queueNode, "Host"),
                    QueueName = xmlHelper.GetValue(queueNode, "QueueName"),
                };
                hosts.Add(node);
            }
            return hosts;
        }

        /// <summary>
        /// 获取Queue根据数据库读取的配置
        /// </summary>
        /// <param name="serverQueueConfig"></param>
        /// <returns></returns>
        private static List<SubscribeHost> GetSubscribeHost(List<ServerQueueConfig> serverQueueConfig)
        {
            var hosts = new List<SubscribeHost>();
            foreach (var queueConfig in serverQueueConfig)
            {
                var node = new SubscribeHost
                {
                    Host = queueConfig.Host,
                    QueueName = queueConfig.QueueName,
                };
                hosts.Add(node);
            }
            return hosts;
        }
    }
}
