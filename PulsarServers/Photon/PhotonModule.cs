using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace PulsarServers.Photon
{
    /// <summary>
    /// Discord chat commands related to Photon.
    /// </summary>
    [Group("photon")]
    public class PhotonModule : ModuleBase
    {
        private readonly PhotonService service;

        public PhotonModule(PhotonService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Subscribes the current Discord channel to Photon lobby updates.
        /// </summary>
        /// <returns></returns>
        [Command("subscribe", RunMode = RunMode.Async), Summary("Starts displaying game list in channel by editing persistent message.")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        public async Task Start()
        {
            await service.AddLobbyMessages(Context.Channel);
        }

        /// <summary>
        /// Unsubscribes the current Discord channel from Photon lobby updates.
        /// </summary>
        /// <returns></returns>
        [Command("unsubscribe", RunMode = RunMode.Async), Summary("Stops displaying game list in channel and deletes persistent message.")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        public async Task Stop()
        {
            await service.DeleteLobbyMessages(Context.Channel);
        }
    }
}
