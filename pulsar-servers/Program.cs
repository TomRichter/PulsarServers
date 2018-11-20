using ExitGames.Client.Photon.LoadBalancing;
using System;

namespace pulsar_servers
{
    class Program
    {
        private static readonly string[] availableRegions = { "us", "eu", "jp", "asia", "au" };
        static void Main(string[] args)
        {
            string appId = "***REMOVED***";
            string gameVersion = "Beta 20.2";

            Console.WriteLine("Iterating RoomInfos...");
            foreach (string region in availableRegions)
            {
                PhotonClient photon = new PhotonClient(appId, gameVersion, region);

                Console.WriteLine(String.Format("===== Region: {0} =====", region));
                foreach (RoomInfo room in photon.GetRooms())
                {
                    Console.WriteLine(room.ToMarkdown());
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
