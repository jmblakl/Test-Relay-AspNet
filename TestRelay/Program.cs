using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestRelay
{
    public class Program
    {
        //this would go in a config somewhere
        private const string ConnectionString = "[replace with full connection string]";
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseAzureRelay(options =>
                        {
                            options.UrlPrefixes.Add(ConnectionString);
                        });
                    webBuilder.UseContentRoot(Path.GetFullPath(@"."));
                    webBuilder.UseWebRoot(Path.GetFullPath(@".\wwwroot"));
                });
    }
}
