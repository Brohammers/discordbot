using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace discordBot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
        
        private DiscordSocketClient discordSocketClient;
        private CommandService commandService;
        private IServiceProvider serviceProvider;

        public async Task RunBotAsync()
        {
            discordSocketClient = new DiscordSocketClient();
            commandService = new CommandService();
            serviceProvider = new ServiceCollection()
                .AddSingleton(discordSocketClient)
                .AddSingleton(commandService)
                .BuildServiceProvider();

            String botToken = "MzE3NDQ3NjI2NDYxMTUxMjMy.DSbDPg.AdLYOBMP8hJ3QpLbzQkgSPHITog";

            discordSocketClient.Log += Log;

            await RegisterCommandAsync();

            await discordSocketClient.LoginAsync(Discord.TokenType.Bot, botToken);

            await discordSocketClient.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandAsync()
        {
            discordSocketClient.MessageReceived += HandleCommandAsync;
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argumentPosition = 0;

            if (message.HasStringPrefix("!", ref argumentPosition) || message.HasMentionPrefix(discordSocketClient.CurrentUser, ref argumentPosition))
            {
                SocketCommandContext context = new SocketCommandContext(
                    discordSocketClient, message);

                var result =  await commandService.ExecuteAsync(context, argumentPosition, serviceProvider);

                if (!result.IsSuccess)
                {
                    Console.WriteLine("Oops, " + result.ErrorReason);
                }
                
            }

        }
    }
}
