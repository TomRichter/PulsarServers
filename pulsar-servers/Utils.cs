using ExitGames.Client.Photon.LoadBalancing;
using System.Collections.Generic;

namespace pulsar_servers
{
    public static class Utils
    {
        public static readonly Dictionary<string, string> shipTypes = new Dictionary<string, string> {
            { "E_ALCHEMIST", "Alchemist" },
            { "E_BOARDING_DRONE", "W.D. Transport Ship" },
            { "E_CARRIER", "Carrier" },
            { "E_CIVILIAN_FUEL", "Civilian Ship" },
            { "E_CORRUPTED_DRONE", "Ancient Sentry" },
            { "E_WDCRUISER", "W.D. Cruiser" },
            { "E_FLUFFY_DELIVERY", "Fluffy One" },
            { "E_INTREPID_SC", "Intrepid SC" },
            { "E_INTREPID", "Intrepid" },
            { "E_OUTRIDER", "Outrider" },
            { "E_STARGAZER", "Stargazer" },
            { "E_ANNIHILATOR", "W.D. Annihilator" },
            { "E_DESTROYER", "W.D. Destroyer" },
        };

        public static string ToMarkdown(this RoomInfo room)
        {
            string lockEmoji = (bool)room.CustomProperties["Private"] ? "" : ":lock: ";
            int totalPlayers = (int)room.CustomProperties["CurrentPlayersPlusBots"];

            if (!shipTypes.TryGetValue((string)room.CustomProperties["Ship_Type"], out string shipType))
            {
                shipType = "N/A";
            }

            return $"{lockEmoji}**{totalPlayers}/{room.MaxPlayers}** - **{shipType}** - {room.Name}";
        }
    }
}
