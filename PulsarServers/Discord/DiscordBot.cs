using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PulsarServers.Photon;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace PulsarServers.Discord
{
    class DiscordBot : DiscordSocketClient
    {
        private CommandService commands;
        private readonly IServiceProvider services;

        /// <summary>
        /// Creates the Discord bot and starts running it indefinitely.
        ///
        /// To get the Discord Bot auth token, visit the Discord Dev Applications site,
        /// choose your bot, open the Bot tab, then Reveal Token.  Website link:
        /// https://discordapp.com/developers/applications/
        /// </summary>
        /// <param name="token">Discord Bot auth token.</param>
        /// <returns></returns>
        public static async Task CreateBot(string token)
        {
            DiscordBot bot = new DiscordBot(token, new DiscordSocketConfig() { LogLevel = LogSeverity.Info });

            await bot.LoginAsync(TokenType.Bot, token);
            await bot.StartAsync();

            await Task.Delay(-1);
        }

        /// <summary>
        /// Constructor of DiscordBot.
        /// </summary>
        /// <param name="token">Discord Bot auth token.</param>
        /// <param name="config">Special Discord.NET framework bot configuration.</param>
        private DiscordBot(string token, DiscordSocketConfig config) : base(config)
        {
            // Subscribe to events
            base.Log += Log;

            // Start services
            services = new ServiceCollection()
                .AddSingleton(new PhotonService(
                    Config.Get("PHOTON_APPID"),
                    Config.Get("GAME_VERSION")
                )).BuildServiceProvider();

            // Automatically find and start all modules
            commands = new CommandService(new CommandServiceConfig() { LogLevel = LogSeverity.Info });
            commands.Log += Log;
            commands.CommandExecuted += CommandExecuted;
            base.MessageReceived += TryRunCommand;
            commands.AddModulesAsync(Assembly.GetExecutingAssembly()).Wait();
        }

        /// <summary>
        /// Attempts to convert user messages into commands.  Only occurs when
        /// users supply the correct prefix (e.g., @mentioning the bot).
        /// </summary>
        /// <param name="messageParam">User message being processed into a command.</param>
        /// <returns></returns>
        private async Task TryRunCommand(SocketMessage messageParam)
        {
            SocketUserMessage message = messageParam as SocketUserMessage;
            if (message == null)
            {
                return;
            }

            int argPos = 0;
            if (!message.HasMentionPrefix(this.CurrentUser, ref argPos))
            {
                return;
            }

            CommandContext cmdContext = new CommandContext(this, message);
            IResult result = await commands.ExecuteAsync(cmdContext, argPos, services);

            if (!result.IsSuccess)
            {
                await cmdContext.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        /// <summary>
        /// Handler for command log events.  Prints messages to console.
        /// </summary>
        /// <param name="info">Command that was called.</param>
        /// <param name="context">Context for where the command was called (e.g., a channel).</param>
        /// <param name="result">Result of command, including success and failure messages.</param>
        /// <returns></returns>
        private Task CommandExecuted(CommandInfo info, ICommandContext context, IResult result)
        {
            Console.WriteLine(result);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handler for general bot log events.  Prints messages to console.
        /// </summary>
        /// <param name="message">Log message to be displayed.</param>
        /// <returns></returns>
        private new Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}
