﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VanillaBot.Services;

namespace VanillaBot
{
    public class VanillaBot
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private IConfiguration _config;

        public VanillaBot()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;

            try
            {
                _config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("config.json")
                    .Build();
            } catch (FileNotFoundException) {
                Console.WriteLine("Please provide a configuration file.");
                Environment.Exit(1);
            }

            _services =  new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_config)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }

        public Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        public async Task Start(string token)
        {
            await _services.GetRequiredService<CommandHandler>().Initialize();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        public async Task StartFromConfig()
        {
            await Start(_config["token"]);
        }
    }
}