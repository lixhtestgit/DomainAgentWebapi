using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using DomainAgentWebapi.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace DomainAgentWebapi.Ajax
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class DomainMappingMiddleware : BaseMiddleware
    {

        public ConfigSetting ConfigSetting { get; set; }

        public ILogger<DomainMappingMiddleware> Logger { get; set; }

        public HttpClient HttpClient = null;

        private static object _Obj = new object();
        public DomainMappingMiddleware(RequestDelegate next, IConfiguration configuration, IMemoryCache memoryCache, ConfigSetting configSetting, ILogger<DomainMappingMiddleware> logger, IHttpClientFactory clientFactory) : base(next, configuration, memoryCache)
        {
            this.ConfigSetting = configSetting;
            this.Logger = logger;
            this.HttpClient = clientFactory.CreateClient("domainServiceClient");
        }


        public async Task Invoke(HttpContext httpContext)
        {
            string requestUrl = null;
            string requestHost = null;

            string dateFlag = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff");

            requestUrl = httpContext.Request.GetDisplayUrl();

            bool isExistDomain = false;
            bool isLocalWebsite = this.ConfigSetting.GetValue("IsLocalDomainService") == "true";

            if (httpContext.Request.Query.ContainsKey("returnurl"))
            {
                requestUrl = httpContext.Request.Query["returnurl"].ToString();
                requestUrl = HttpUtility.UrlDecode(requestUrl);
                isLocalWebsite = false;
            }

            Match match = Regex.Match(requestUrl, this.ConfigSetting.GetValue("DomainHostRegex"));
            if (match.Success)
            {
                isExistDomain = true;
                requestHost = match.Value;
            }

#if DEBUG
            requestUrl = "http://139.199.128.86:444/?returnurl=https%3A%2F%2F3w.huanqiu.com%2Fa%2Fc36dc8%2F9CaKrnKnonm";
#endif

            if (isExistDomain)
            {
                this.Logger.LogInformation($"{dateFlag}_记录请求地址：{requestUrl},是否存在当前域：{isExistDomain},是否是本地环境：{isLocalWebsite}");

                bool isFile = false;

                //1-设置响应的内容类型
                MediaTypeHeaderValue mediaType = null;

                if (requestUrl.Contains(".js"))
                {
                    mediaType = new MediaTypeHeaderValue("application/x-javascript");
                    //mediaType.Encoding = System.Text.Encoding.UTF8;
                }
                else if (requestUrl.Contains(".css"))
                {
                    mediaType = new MediaTypeHeaderValue("text/css");
                    //mediaType.Encoding = System.Text.Encoding.UTF8;
                }
                else if (requestUrl.Contains(".png"))
                {
                    mediaType = new MediaTypeHeaderValue("image/png");
                    isFile = true;
                }
                else if (requestUrl.Contains(".jpg"))
                {
                    mediaType = new MediaTypeHeaderValue("image/jpeg");
                    isFile = true;
                }
                else if (requestUrl.Contains(".ico"))
                {
                    mediaType = new MediaTypeHeaderValue("image/x-icon");
                    isFile = true;
                }
                else if (requestUrl.Contains(".gif"))
                {
                    mediaType = new MediaTypeHeaderValue("image/gif");
                    isFile = true;
                }
                else if (requestUrl.Contains("/api/") && !requestUrl.Contains("/views"))
                {
                    mediaType = new MediaTypeHeaderValue("application/json");
                }
                else
                {
                    mediaType = new MediaTypeHeaderValue("text/html");
                    mediaType.Encoding = System.Text.Encoding.UTF8;
                }

                //2-获取响应结果

                if (isLocalWebsite)
                {
                    //本地服务器将请求转发到远程服务器
                    requestUrl = this.ConfigSetting.GetValue("MyDomainAgentHost") + "?returnurl=" + HttpUtility.UrlEncode(requestUrl);
                }

                if (isFile == false)
                {
                    string result = await this.HttpClient.MyGet(requestUrl);

                    if (httpContext.Response.HasStarted == false)
                    {
                        this.Logger.LogInformation($"{dateFlag}_请求结束_{requestUrl}_长度{result.Length}");

                        //请求结果展示在客户端，需要重新请求本地服务器，因此需要将https转为http
                        result = result.Replace("https://", "http://");
                        //替换"/a.ico" 为："http://www.baidu.com/a.ico"
                        result = Regex.Replace(result, "\"\\/(?=[a-zA-Z0-9]+)", $"\"{requestHost}/");
                        //替换"//www.baidu.com/a.ico" 为："http://www.baidu.com/a.ico"
                        result = Regex.Replace(result, "\"\\/\\/(?=[a-zA-Z0-9]+)", "\"http://");

                        httpContext.Response.ContentType = mediaType.ToString();

                        await httpContext.Response.WriteAsync(result ?? "");
                    }
                    else
                    {
                        this.Logger.LogInformation($"{dateFlag}_请求结束_{requestUrl}_图片字节流长度{result.Length}_Response已启动");
                    }
                }
                else
                {
                    byte[] result = await this.HttpClient.MyGetFile(requestUrl);

                    if (httpContext.Response.HasStarted == false)
                    {
                        this.Logger.LogInformation($"{dateFlag}_请求结束_{requestUrl}_图片字节流长度{result.Length}");

                        httpContext.Response.ContentType = mediaType.ToString();
                        await httpContext.Response.Body.WriteAsync(result, 0, result.Length);
                    }
                    else
                    {
                        this.Logger.LogInformation($"{dateFlag}_请求结束_{requestUrl}_图片字节流长度{result.Length}_Response已启动");
                    }
                }
            }
        }
    }
}
