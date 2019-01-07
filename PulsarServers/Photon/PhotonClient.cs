using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PulsarServers.Photon
{
    /// <summary>
    /// Specialized Photon client for connecting to Photon lobbies and retrieving game data.
    /// </summary>
    class PhotonClient : LoadBalancingClient
    {
        public delegate Task GameListUpdate(string region, IEnumerable<RoomInfo> rooms);
        public event GameListUpdate OnGameListUpdated;

        internal int tickLength = 100;
        internal CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// PhotonClient constructor.
        /// </summary>
        /// <param name="appId">Photon application ID, from game or Photon website.</param>
        /// <param name="gameVersion">Game version string.</param>
        /// <param name="regionCode">Photon lobby's region code.</param>
        public PhotonClient(string appId, string gameVersion, string region) : base(ConnectionProtocol.Udp)
        {
            Console.WriteLine("PhotonClient");
            this.AppId = appId;
            this.AppVersion = $"{gameVersion}_{Config.Get("PHOTON_VERSION")}";
            this.NickName = "Discord Webhook";

            this.ConnectToRegionMaster(region);

            StartTicking();
        }

        /// <summary>
        /// Callback triggered by events in Photon.  Propagate Photon events to the rest of the app here.
        /// </summary>
        /// <param name="photonEvent">Photon event corresponding to Photon.LoadBalancing.EventCode</param>
        public override void OnEvent(EventData photonEvent)
        {
            base.OnEvent(photonEvent);

            if (photonEvent.Code == EventCode.GameListUpdate || photonEvent.Code == EventCode.GameList)
            {
                Console.WriteLine($"{photonEvent.ToStringFull()}\n");
                OnGameListUpdated?.Invoke(CloudRegion, RoomInfoList.Values);
            }
        }

        /// <summary>
        /// Infinitely running heartbeat to pump Photon event queue.  Without this, PhotonClient will never
        /// pass information and its connection will time out.
        /// </summary>
        private async void StartTicking()
        {
            /*new Task(() => Tick(), cancellationTokenSource.Token, TaskCreationOptions.LongRunning).Start()*/
            Task tick = Task.Factory.StartNew(() => { Tick(); });
            await tick;
        }

        /// <summary>
        /// A single heartbeat that pumps the Photon event queue.
        /// </summary>
        private void Tick()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                this.Service();
                Thread.Sleep(tickLength);
            }
        }
    }
}
