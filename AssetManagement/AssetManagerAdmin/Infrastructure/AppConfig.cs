using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Classes;
using AssetManagerAdmin.Properties;

namespace AssetManagerAdmin
{
    public class AppConfig
    {
        public static IList<ServerConfig> GetServersList()
        {
            var result = new List<ServerConfig>();

            Settings.Default.ServersList.Cast<string>().ToList().ForEachWithIndex((s, i) =>
            {
                var serverConfig = GetServerSetting(i);
                result.Add(serverConfig);
            });

            return result;
        }

        public static ServerConfig GetServerSetting(int index)
        {
            var serversList = Settings.Default.ServersList.Cast<string>().ToList();
            
            var settings = serversList[index].Split(';');

            var config = new ServerConfig();

            foreach (var setting in settings)
            {
                var settingParts = setting.Split('=');

                if (settingParts.Length != 2)
                    throw new ArgumentException(string.Format(
                        "Setting is missing a value: {0}", setting),
                        "serverSetting");

                switch (settingParts[0].ToLower())
                {
                    case "name":
                        config.Name = settingParts[1];
                        break;
                    case "auth":
                        config.AuthUrl = settingParts[1];
                        break;
                    case "admin":
                        config.AdminUrl = settingParts[1];
                        break;
                    case "api":
                        config.ApiUrl = settingParts[1];
                        break;
                    case "site":
                        config.SiteUrl = settingParts[1];
                        break;
                }
            }

            return config;
        }
    }
}
