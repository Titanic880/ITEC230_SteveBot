﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SteveBot
{
    class main
    {
        //the bot cannot run within a static main function, so we build one with
        //Async and awaiter built into it for ease of use as a bot
        public static void Main(string[] args)
            => new main().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            //Not 100% sure what this does in its entirity; .AddSingleton == static
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            //This shall never be seen by anyone but me... (Sign in token for the bot)
            var token = File.ReadAllText("../../auth.json");

            _client.Log += _client_Log;
            await RegisterCommandsAsync();

            //logs the bot into discord
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            //tells the bot to wait for input
            await Task.Delay(-1);
        }

        //outputs to Console
        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        //Adds commands to the bot
        public async Task RegisterCommandsAsync()
        {
            //registers anything tagged as 'Task' to the set of commands that can be called
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        //Command Handler
        private async Task HandleCommandAsync(SocketMessage arg)
        {
            //Stores user message and initilizes message position
            var message = arg as SocketUserMessage;
            int argPos = 0;
            if (message == null)
                return;
            //Checks for prefix or specified passthrough commands
            if (message.HasStringPrefix("!", ref argPos)
             || message.Content.ToLower() == "ping"
             || message.Content.ToLower() == "help"
             || message.Content.ToLower() == "calculator")
            {
                //generates an object from the user message
                var context = new SocketCommandContext(_client, message);

                //checks to see if the user is a bot
                if (message.Author.IsBot)
                    return;

                //Attempts to run the command and outputs accordingly
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                    await message.Channel.SendMessageAsync(result.ErrorReason);
                }
                if (result.Error.Equals(CommandError.UnmetPrecondition))
                    await message.Channel.SendMessageAsync(result.ErrorReason);
            }
            //if something fails the Prefix check it just returns
            else
                return;
        }
    }
}