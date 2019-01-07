using Discord;
using ExitGames.Client.Photon.LoadBalancing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulsarServers.Photon
{
    /// <summary>
    /// Creates connections to all Photon regions for a game and provides callbacks for certain Photon events.
    /// </summary>
    public class PhotonService
    {
        /// <summary>
        /// Map of subscribed Discord channel IDs to maps of Photon region codes -> Discord message IDs.
        /// One message is used per region.
        /// </summary>
        private readonly ConcurrentDictionary<ulong, ConcurrentDictionary<string, IUserMessage>> LobbyMessages = new ConcurrentDictionary<ulong, ConcurrentDictionary<string, IUserMessage>>();

        /// <summary>
        /// Map of Photon region codes to Photon client wrappers.
        /// </summary>
        private readonly IDictionary<string, PhotonRegion> regions = new Dictionary<string, PhotonRegion>()
        {
            {"jp", new PhotonRegion("jp", "Japan", "https://i.imgur.com/KWELvdo.png") },
            {"asia", new PhotonRegion("asia", "Asia", "https://i.imgur.com/8ZeuAgm.png") },
            {"au", new PhotonRegion("au", "Australia", "https://i.imgur.com/RhchjcK.png") },
            {"eu", new PhotonRegion("eu", "Europe", "https://i.imgur.com/qSgNsjO.png") },
            {"us", new PhotonRegion("us", "United States", "https://i.imgur.com/91Iyw5n.png") },
        };

        /// <summary>
        /// Discord embed footer icon used next to lobby statistics.
        /// </summary>
        private readonly string statsIconUrl = "https://i.imgur.com/J4GXWfu.png";

        /// <summary>
        /// PhotonService constructor.
        /// </summary>
        /// <param name="appId">Photon application ID, from game or Photon website.</param>
        /// <param name="gameVersion">Game version string.</param>
        public PhotonService(string appId, string gameVersion)
        {
            Console.WriteLine("PhotonService");
            foreach (PhotonRegion region in regions.Values)
            {
                region.StartClient(appId, gameVersion);
                region.PhotonClient.OnGameListUpdated += UpdateLobbyMessage;
            }
        }

        /// <summary>
        /// Creates a new Discord embed containing the list of games for a Photon regional lobby.
        /// Truncates games that do not fit in the Embed's character limit, but displays the
        /// complete game and real player counts as lobby statistics in the footer.
        /// </summary>
        /// <param name="regionCode">Photon lobby's region code.</param>
        /// <param name="roomInfos">List of games to display.</param>
        /// <returns></returns>
        private Embed CreateRegionEmbed(string regionCode, IEnumerable<RoomInfo> roomInfos)
        {
            IEnumerable<String> roomLines = roomInfos
                .OrderBy(r => r.CustomProperties["Private"])
                .ThenBy(r => r.CustomProperties["CurrentPlayersPlusBots"])
                .Select(r => r.ToMarkdown());

            int playerCount = roomInfos.Sum(r => r.PlayerCount);
            int displayCount = 0;

            StringBuilder descBuilder = new StringBuilder(capacity: 2048, maxCapacity: 2048);
            foreach (String room in roomLines)
            {
                if (room.Length + descBuilder.Length + 2 < descBuilder.MaxCapacity)
                {
                    descBuilder.AppendLine(room);
                    displayCount++;
                }
                else
                {
                    break;
                }
            }

            PhotonRegion region = regions[regionCode];

            return new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithAuthor($"{region.Name} Lobby", region.IconURL)
                .WithDescription(descBuilder.ToString())
                .WithFooter($"Showing {displayCount} of {roomInfos.Count()} games totaling {playerCount} real players.", statsIconUrl)
                .WithCurrentTimestamp()
                .Build();
        }

        /// <summary>
        /// Subscribes Discord channel to Photon lobby updates and creates associated Discord messages.
        /// </summary>
        /// <param name="channel">Discord channel to subscribe.</param>
        /// <returns></returns>
        public async Task AddLobbyMessages(IMessageChannel channel)
        {
            string channelMention = MentionUtils.MentionChannel(channel.Id);

            if (!LobbyMessages.ContainsKey(channel.Id))
            {
                // Complete channel level dict before adding to LobbyMessages to avoid trying to update a non-existent region message
                ConcurrentDictionary<string, IUserMessage> temp = new ConcurrentDictionary<string, IUserMessage>();
                foreach (PhotonRegion region in regions.Values)
                {
                    temp[region.Code] = await channel.SendMessageAsync(String.Empty, embed: CreateRegionEmbed(region.Code, region.PhotonClient.RoomInfoList.Values));
                }

                LobbyMessages[channel.Id] = temp;
                // TODO: Save channel ID

                await channel.SendMessageAsync($"Subscribed {channelMention} to Photon lobby.  Updates will be continuously edited into lobby message.");
            }
            else
            {
                await channel.SendMessageAsync($"Failed to subscribe: {channelMention} is already subscribed to this Photon lobby.");
            }
        }

        /// <summary>
        /// Edits an existing Discord message with the latest Photon lobby data.
        /// </summary>
        /// <param name="regionCode">Photon lobby's region code.</param>
        /// <param name="roomInfos">List of games to display.</param>
        /// <returns></returns>
        public async Task UpdateLobbyMessage(string regionCode, IEnumerable<RoomInfo> roomInfos)
        {
            // Update embeds for this region across all subscribed channels
            foreach (ConcurrentDictionary<string, IUserMessage> regionMessages in LobbyMessages.Values)
            {
                await regionMessages[regionCode].ModifyAsync(msg =>
                {
                    msg.Content = String.Empty;
                    msg.Embed = CreateRegionEmbed(regionCode, roomInfos);
                });
            }
        }


        /// <summary>
        /// Unsubscribes Discord channel from Photon lobby updates and deletes associated Discord messages.
        /// </summary>
        /// <param name="channel">Discord channel to unsubscribe.</param>
        /// <returns></returns>
        public async Task DeleteLobbyMessages(IMessageChannel channel)
        {
            string channelMention = MentionUtils.MentionChannel(channel.Id);

            if (LobbyMessages.TryGetValue(channel.Id, out ConcurrentDictionary<string, IUserMessage> regionMessages))
            {
                foreach (string key in regionMessages.Keys)
                {
                    regionMessages.TryRemove(key, out IUserMessage region);
                    await region.DeleteAsync();
                }

                LobbyMessages.TryRemove(channel.Id, out var x);

                await channel.SendMessageAsync($"Unsubscribed {channelMention} from Photon lobby.  Deleting lobby message.");
            }
            else
            {
                await channel.SendMessageAsync($"Failed to unsubscribe: {channelMention} is not yet subscribed to this Photon lobby.");
            }
        }
    }
}
