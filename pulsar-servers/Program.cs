using Discord;
using Discord.Webhook;
using ExitGames.Client.Photon.LoadBalancing;
using System;
using System.Linq;

namespace pulsar_servers
{
    class Program
    {
        private static readonly string[] availableRegions = { "us", "eu", "jp", "asia", "au" };
        static void Main(string[] args)
        {
            string appId = "***REMOVED***";
            string gameVersion = "Beta 20.2";

            DiscordWebhookClient webhook = new DiscordWebhookClient(0, "");

            Console.WriteLine("Iterating RoomInfos...");
            foreach (string region in availableRegions)
            {
                EmbedBuilder eb = new EmbedBuilder()
                    .WithTitle($"{region.ToUpper()} PULSAR Games")
                    .WithColor(Color.Blue)
                    .WithCurrentTimestamp();

                PhotonClient photon = new PhotonClient(appId, gameVersion, region);
                foreach (RoomInfo room in photon.GetRooms().OrderBy(x => x.ToMarkdown()))
                {
                    eb.Description += room.ToMarkdown();
                }

                Embed embed = eb.Build();
                Console.WriteLine(embed.ToString());
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
