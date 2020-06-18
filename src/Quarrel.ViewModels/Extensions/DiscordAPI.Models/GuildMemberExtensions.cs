// Copyright (c) Quarrel. All rights reserved.

namespace DiscordAPI.Models
{
    /// <summary>
    /// Extensions methods on the <see cref="GuildMember"/> class.
    /// </summary>
    internal static class GuildMemberExtensions
    {
        /// <summary>
        /// Gets the display name of the member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>Their displayname</returns>
        public static string DisplayName(this GuildMember member)
        {
            return member.Nick ?? member.User.Username;
        }
    }
}
