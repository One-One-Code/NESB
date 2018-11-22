using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace NESB.SyncService
{
    using System.Configuration;
    using System.IO;

    using Microsoft.Practices.ServiceLocation;

    using NESB.Configuration;
    using NESB.Model.Service;
    using NESB.MQ.Consumer;
    using NESB.MQ.Consumer.ConsumerProcess;
    using NESB.MQ.Event;
    using NESB.MQ.MqCore;

    using RuanYun.IoC;

    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();

        }

        /// <summary>
        /// 调试启动方法
        /// </summary>
        public void Debug()
        {
            this.InitSetting();
        }

        /// <summary>
        /// 服务初始化配置
        /// </summary>
        private void InitSetting()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            StrapRegister.UseLog4Net(new FileInfo(path + "Configs\\logger.config"));
            StrapRegister.Bootstrap(this.RegisterIoc);
            this.StartListeners();
        }

        /// <summary>
        /// 启动RabbitMQ监听和发布配置
        /// 使用xml文件进行queue配置
        /// </summary>
        private void StartListeners()
        {
            var server = ConfigurationManager.AppSettings["PublishServers"];
            var reg = ServiceLocator.Current.GetInstance<IRegistration>();
            reg.BusOnRabbitMq(config => config.PublishAt(server, "NESBSync", 4));

            SubscribeHostFactory.QueueConfigure(AppDomain.CurrentDomain.BaseDirectory + "Configs\\QueueConfigure.xml", 4);
        }

        /// <summary>
        /// 注册Consumer
        /// </summary>
        /// <param name="locator">Ioc定位器</param>
        private void RegisterIoc(IRegistration locator)
        {
            locator.ScanAssembly<ServiceRegisterConsumer>();

            locator.Map<IAfterConsumer, AfterConsumer>().Singleton();
        }

        /// <summary>
        /// 服务启动
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            this.InitSetting();
            base.OnStart(args);
        }

        /// <summary>
        /// 服务停止
        /// </summary>
        protected override void OnStop()
        {
            base.OnStop();
        }

        /// <summary>
        /// 服务暂停
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
        }

        /// <summary>
        /// 服务继续
        /// </summary>
        protected override void OnContinue()
        {
            base.OnContinue();
        }
    }
}
