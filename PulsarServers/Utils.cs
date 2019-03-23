using Discord;
using ExitGames.Client.Photon.LoadBalancing;
using System.Collections.Generic;

namespace PulsarServers
{
    /// <summary>
    /// One-off helpers for data conversion.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Map of internal ship type strings to human-readable ship type names.
        /// </summary>
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
            { "E_ROLAND", "Roland" },
        };

        // Discord Emojies used in Embeds.
        public static readonly IEmote SPACER = Emote.Parse("<:spacer:516080250732544020>");
        public static readonly IEmote LOCK = new Emoji("🔒");
        public static readonly IEmote MODS = Emote.Parse("<:mods:520444721634607114>");

        /// <summary>
        /// Collection of characters to be escaped from Markdown formatting to prevent Markdown injection.
        /// </summary>
        private static readonly string[] EscapableMarkdownCharacters =
        {
            "\\", "`", "*", "_", "{", "}", "[", "]", "(", ")", "#", "+", "-", "!", "~", "`", ".", ":", "/", "<", ">", "@", "|", "\r", "\n"
        };

        /// <summary>
        /// Escapes special Markdown characters to prevent Markdown injection in user-influenced bot messages.
        /// </summary>
        /// <param name="text">The raw text to escape.</param>
        /// <returns>Markdown-escaped text.</returns>
        private static string EscapeMarkdown(string text)
        {
            foreach(string character in EscapableMarkdownCharacters)
            {
                text = text.Replace(character, $"\\{character}");
            }

            return text;
        }

        /// <summary>
        /// Creates a Markdown-formatted string representation of a Photon game, optimized for one line.
        /// </summary>
        /// <param name="room">Room to be formatted.</param>
        /// <returns>Markdown-formatted string representation of Photon game info.</returns>
        public static string ToMarkdown(this RoomInfo room)
        {
            bool isModded = room.CustomProperties.ContainsKey("isModded") ? (bool)room.CustomProperties["isModded"] : false;

            IEmote lockEmoji = (bool)room.CustomProperties["Private"] ? LOCK : SPACER;
            IEmote modEmoji =  isModded ? MODS : SPACER;

            int totalPlayers = (int)room.CustomProperties["CurrentPlayersPlusBots"];

            if (!shipTypes.TryGetValue((string)room.CustomProperties["Ship_Type"], out string shipType))
            {
                shipType = "N/A";
            }

            return $"{modEmoji} {lockEmoji} **{totalPlayers}/{room.MaxPlayers}** - **{shipType}** - {EscapeMarkdown(room.Name)}";
        }
    }
}
