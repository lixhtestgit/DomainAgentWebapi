using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace DomainAgentWebapi.Ajax
{
    /// <summary>
    /// 中间件基类
    /// </summary>
    public abstract class BaseMiddleware
    {
        /// <summary>
        /// 等同于ASP.NET里面的WebCache(HttpRuntime.Cache)
        /// </summary>
        protected IMemoryCache MemoryCache { get; set; }

        /// <summary>
        /// 获取配置文件里面的配置内容
        /// </summary>
        protected IConfiguration Configuration { get; set; }
        
        /// <summary>
        /// 下一个中间件
        /// </summary>
        protected RequestDelegate Next { get; set; }

        public BaseMiddleware(RequestDelegate next, params object[] @params)
        {
            this.Next = next;
            foreach (var item in @params)
            {
                if (item is IMemoryCache)
                {
                    this.MemoryCache = (IMemoryCache)item;
                }
                else if (item is IConfiguration)
                {
                    this.Configuration = (IConfiguration)item;
                }
            }
        }

    }
}
