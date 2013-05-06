using System;
using System.Configuration;

namespace MvcSTSApplication.Infrastructure
{
    public class AppSettingsConfigurationProvider : IConfigurationProvider
    {
        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            return ConfigurationManager.AppSettings[key];
        }
    }
}