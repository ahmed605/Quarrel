// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models.Guilds;

namespace DiscordAPI.Models
{
    /// <summary>
    /// Extensions on the <see cref="Guilds.Guild"/>.
    /// </summary>
    internal static class GuildExtensions
    {
        /// <summary>
        /// Gets the url for the guild icon.
        /// </summary>
        /// <param name="guild">The guild to get the icon url for.</param>
        /// <returns>The guild's icon's URL.</returns>
        public static string GetIconUrl(this Guild guild)
        {
            return $"https://cdn.discordapp.com/icons/{guild.Id}/{guild.Icon}.png?size=128";
        }
    }
}
