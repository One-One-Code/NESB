using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.SM
{
    using NESB.Model.Enum;
    using NESB.Model.Service;
    using NESB.SM.ZooKeeperCore;

    public class ServiceManage
    {
        /// <summary>
        /// 获取服务的接口信息
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static ServiceBaseInfo GetService(string serviceName)
        {
            using (var bus = new ZooKeeperBus())
            {
                var serviceBaseInfos = bus.GetData<List<ServiceBaseInfo>>(serviceName);
                return serviceBaseInfos.FirstOrDefault();
            }
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="info"></param>
        /// <param name="serviceRegisterType"></param>
        public static void RegisterService(ServiceBaseInfo info, ServiceRegisterTypeEnum serviceRegisterType)
        {
            using (var bus = new ZooKeeperBus())
            {
                var node = info.ServiceName.Replace("/", ".");
                var services = bus.GetData<List<ServiceBaseInfo>>(node) ?? new List<ServiceBaseInfo>();
                var item = services.FirstOrDefault(p => p.Ip.Equals(info.Ip));
                if (item != null)
                {
                    services.Remove(item);
                }
                switch (serviceRegisterType)
                {
                    case ServiceRegisterTypeEnum.Add:
                    case ServiceRegisterTypeEnum.Update:
                        services.Add(info);
                        break;
                    case ServiceRegisterTypeEnum.Remove:
                        break;
                }
                bus.SetDataWithLock(node, services);
            }
        }
    }
}
