using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuickIndexing.Common
{
    public static class API
    {
        public static IConfigurationRoot configuration;

        public static string GetAPIUrl()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
            configuration = builder.Build();

            var apiUrl = configuration["ApiUrl:Url"].ToString();

            return apiUrl;
        }
    }
}
