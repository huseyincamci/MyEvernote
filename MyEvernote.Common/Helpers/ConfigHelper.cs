using System;
using System.Configuration;

namespace MyEvernote.Common.Helpers
{
    public class ConfigHelper
    {
        //public static object ConfigurationManager { get; private set; }

        public static T Get<T>(string key)
        {
            return (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
        }
    }
}
