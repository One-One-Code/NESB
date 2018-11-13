/*
 * 作者:黄平
 * 创建时间:2014-11-24
 * ------------------修改记录-------------------
 * 修改人      修改日期        修改目的
 * 黄平        2014-11-24      创建
 */

using System;
using System.Collections.Generic;
using MassTransit.SubscriptionConfigurators;

namespace NESB.MQ.MqCore
{
    public interface IConsumeConfigurator
    {
        void Configure(SubscriptionBusServiceConfigurator cfg, List<Type> consumers);
    }
}
