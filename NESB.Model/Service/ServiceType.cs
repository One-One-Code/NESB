using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESB.Model.Service
{
    /// <summary>
    /// 服务的类型
    /// </summary>
    public enum ServiceType
    {
        /// <summary>
        /// rest服务，使用http或者https协议
        /// </summary>
        Rest = 1,

        /// <summary>
        /// 使用System.ServiceModel调用服务
        /// WCF服务
        /// </summary>
        ServiceModel = 2
    }
}
