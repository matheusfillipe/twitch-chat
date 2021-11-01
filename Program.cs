﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TwitchBot.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitchBot.Services;

namespace TwitchBot
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var host = CreateHostBuilder(args);

            await host.RunConsoleAsync();
            return Environment.ExitCode;
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();

                    IHostEnvironment env = hostingContext.HostingEnvironment;

                    configuration
                        .AddJsonFile("appsettings.json", true, true);

                    IConfigurationRoot configurationRoot = configuration.Build();

                    TwitchSettings options = new();
                    configurationRoot.GetSection(nameof(TwitchSettings))
                                     .Bind(options);
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddSingleton<ITwitchService, TwitchService>();
                    services.AddHostedService<Worker>();

                    var settings = hostingContext.Configuration
                        .GetSection(nameof(TwitchSettings))
                        .Get<TwitchSettings>();
                    services.AddSingleton<TwitchSettings>(settings);
                });
    }
}
