using System;
using System.IO;
using Newtonsoft.Json;
using Speiser.Masterthesis.ConfigurationService.Contracts;

namespace Speiser.Masterthesis.ConfigurationService
{
    internal class ConfigurationReader : IConfigurationService
    {
        public Configuration GetConfiguration(string configPath)
        {
            if (!string.IsNullOrWhiteSpace(configPath) && File.Exists(configPath))
            {
                return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(configPath));
            }

            throw new ArgumentException($"No config file found {configPath}", nameof(configPath));
        }
    }
}
