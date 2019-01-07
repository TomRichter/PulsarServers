using System;
using System.Configuration;

namespace PulsarServers
{
    /// <summary>
    /// Simple config file wrapper.
    /// </summary>
    internal static class Config
    {
        /// <summary>
        /// Retrieves the value at the given key from the config file.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Stores the given key-value pair in the config file.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set(string key, object value)
        {
            throw new NotImplementedException();
        }
    }
}
