using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace DomainAgentWebapi.Common
{
    public class ConfigSetting
    {
        public ConfigSetting(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private List<DomainIPMapping> _domainIPMappingList = null;
        public List<DomainIPMapping> DomainIPMappingList
        {
            get
            {
                if (this._domainIPMappingList == null || this._domainIPMappingList.Count == 0)
                {
                    this._domainIPMappingList = this.GetDomainIPMappings();
                }
                return this._domainIPMappingList;
            }
        }

        private List<DomainIPMapping> GetDomainIPMappings()
        {
            List<DomainIPMapping> domainIPMappingList = new List<DomainIPMapping>();

            List<IConfigurationSection> configurationSections = this.Configuration.GetSection("DomainIPMapping").GetChildren().ToList();
            foreach (IConfigurationSection item in configurationSections)
            {
                domainIPMappingList.Add(new DomainIPMapping { DomainName = item.Key, DomainIP = item.Value });
            }
            return domainIPMappingList;
        }

        public string GetValue(string key)
        {
            return this.Configuration.GetValue<string>(key);
        }

        /// <summary>
        /// 本方法只对启动类使用，其他请使用单例模式
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static List<DomainIPMapping> GetDomainIPMappings(IConfiguration configuration)
        {
            List<DomainIPMapping> domainIPMappingList = new List<DomainIPMapping>();

            List<IConfigurationSection> configurationSections = configuration.GetSection("DomainIPMapping").GetChildren().ToList();
            foreach (IConfigurationSection item in configurationSections)
            {
                domainIPMappingList.Add(new DomainIPMapping { DomainName = item.Key, DomainIP = item.Value });
            }
            return domainIPMappingList;
        }



    }

    public class DomainIPMapping
    {
        public string DomainName { get; set; }
        public string DomainIP { get; set; }
    }

}
