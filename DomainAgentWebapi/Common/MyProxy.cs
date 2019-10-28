using System;
using System.Net;

namespace DomainAgentWebapi.Common
{
    /// <summary>
    /// 我的代理类
    /// </summary>
    public class MyProxy : IWebProxy
    {
        //代理的地址
        public MyProxy(Uri proxyUri)
        {
            //设置代理请求的票据
            credentials = new NetworkCredential("Administrator", "lixhtxy557%%");
            ProxyUri = proxyUri;
        }
        private NetworkCredential credentials;

        private Uri ProxyUri;

        public ICredentials Credentials { get => credentials; set => throw new NotImplementedException(); }

        //获取代理地址
        public Uri GetProxy(Uri destination)
        {
            return ProxyUri; // your proxy Uri
        }
        //主机host是否绕过代理服务器，设置false即可
        public bool IsBypassed(Uri host)
        {
            return false;
        }
    }
}
