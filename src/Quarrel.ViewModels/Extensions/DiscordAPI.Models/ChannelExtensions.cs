// Copyright (c) Quarrel. All rights reserved.

namespace DiscordAPI.Models.Channels
{
    /// <summary>
    /// Extensions for the <see cref="Channel"/> class.
    /// </summary>
    public static class ChannelExtensions
    {
        /// <summary>
        /// Gets a value indicating whether or not the channel is a standard text channel.
        /// </summary>
        /// <param name="channel">The channel to check.</param>
        /// <returns>Whether or not the channel is a standard text channel.</returns>
        public static bool IsTextChannel(this Channel channel)
        {
            return channel.Type == 0;
        }

        /// <summary>
        /// Gets a value indicating whether or not the channel is a dm channel.
        /// </summary>
        /// <param name="channel">The channel to check.</param>
        /// <returns>Whether or not the channel is a dm channel.</returns>
        public static bool IsDirectChannel(this Channel channel)
        {
            return channel.Type == 1;
        }

        /// <summary>
        /// Gets a value indicating whether or not the channel is a voice channel.
        /// </summary>
        /// <param name="channel">The channel to check.</param>
        /// <returns>Whether or not the channel is a voice channel.</returns>
        public static bool IsVoiceChannel(this Channel channel)
        {
            return channel.Type == 2;
        }

        /// <summary>
        /// Gets a value indicating whether or not the channel is a group channel.
        /// </summary>
        /// <param name="channel">The channel to check.</param>
        /// <returns>Whether or not the channel is a group channel.</returns>
        public static bool IsGroupChannel(this Channel channel)
        {
            return channel.Type == 3;
        }

        /// <summary>
        /// Gets a value indicating whether or not the channel is a category.
        /// </summary>
        /// <param name="channel">The channel to check.</param>
        /// <returns>Whether or not the channel is a category.</returns>
        public static bool IsCategory(this Channel channel)
        {
            return channel.Type == 4;
        }

        /// <summary>
        /// Gets the GuildId of the channel.
        /// </summary>
        /// <param name="channel">The channel to check.</param>
        /// <returns>The guild id of the channel.</returns>
        public static string GuildId(this Channel channel)
        {
            if (channel is GuildChannel guildChannel)
            {
                return guildChannel.GuildId;
            }

            return "DM";
        }
    }
}
