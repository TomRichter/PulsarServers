using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using System;
using System.Collections.Generic;
using System.Threading;

namespace pulsar_servers
{
    class PhotonClient : LoadBalancingClient
    {
        internal int updateTick = 10;
        internal int counter = 0;

        public PhotonClient(string appId, string gameVersion, string region) : base(ConnectionProtocol.Udp)
        {
            this.AppId = appId;
            this.AppVersion = gameVersion + "_1.58";
            this.NickName = "George";

            this.ConnectToRegionMaster(region);
        }

        public IEnumerable<RoomInfo> GetRooms()
        {
            // TODO: Timeout in case there really are zero games?
            // Can't get games right away because they aren't instantly available.
            while ((this.RoomInfoList == null || this.RoomInfoList.Count == 0) && counter < 1000)
            {
                this.Service();
                Thread.Sleep(updateTick);
                counter += updateTick;
            }

            return this.RoomInfoList.Values;
        }
    }
}
