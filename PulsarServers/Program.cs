using PulsarServers.Discord;
using System.Configuration;
using System.Threading.Tasks;

namespace PulsarServers
{
    class Program
    {
        public static void Main(string[] args) => new Program().MainAsync(args).GetAwaiter().GetResult();

        /// <summary>
        /// Entry point for the Discord bot.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns></returns>
        public async Task MainAsync(string[] args)
        {
            await DiscordBot.CreateBot(Config.Get("DISCORD_TOKEN"));
        }
    }
}
