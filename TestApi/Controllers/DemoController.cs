using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestApi.Controllers
{
    using NESB.Model.Service;
    using NESB.SM;

    using RestSharp;

    public class DemoController : ApiController
    {
        [HttpGet]
        public ResponseObj ProcessRequest(string pars)
        {
            return new ResponseObj() { obj = pars };
        }

        [HttpGet]
        public string ReceiveRequest()
        {
            var server = ServiceManage.GetService("demo.processrequest");
            var client = new RestClient(string.Format("{0}://{1}:{2}", server.Protocol, server.HostName, server.Port));
            var result = new RestRequest();
            var data = new Dictionary<string, string>();
            result.Resource = server.Source;
            result.Method = Method.GET;
            result.AddParameter("pars", "test222");
            result.AddParameter("Content-Type", "application/json", ParameterType.HttpHeader);
            var r = client.Execute<ResponseObj>(result);
            return r.Data.obj;
        }


    }

    public class ResponseObj
    {
        public string obj { get; set; }
    }
}
