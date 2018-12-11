using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TestApi
{
    using System.Configuration;

    using Microsoft.Practices.ServiceLocation;

    using NESB.Configuration;
    using NESB.Model.Enum;
    using NESB.Model.Service;
    using NESB.MQ.Event;
    using NESB.MQ.MqCore;

    using RuanYun.IoC;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            StrapRegister.Bootstrap();
            var reg = ServiceLocator.Current.GetInstance<IRegistration>();
            reg.BusOnRabbitMq(config => config.PublishAt("10.0.3.5", "NESBSync", 4));
            //var @event = new ServiceRegisterEvent
            //{
            //    RegisterType = ServiceRegisterTypeEnum.Add,
            //    Info = new ServiceBaseInfo
            //    {
            //        ServiceName = "demo/processrequest",
            //        HostName = "localhost",
            //        Ip = "10.0.3.5",
            //        Port = "4005",
            //        Protocol = "http",
            //        ServiceType = ServiceType.Rest,
            //        RequestType = RequestTypeEnum.Get,
            //        Source = "api/demo/processrequest"
            //    }
            //};
            //ServiceLocator.Current.GetInstance<IEventPublisher>().Publish(@event);
        }
    }
}
