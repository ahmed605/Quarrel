// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Messages.Services.Discord.Messages
{
    /// <summary>
    /// A message sent when a message is deleted.
    /// </summary>
    public class DiscordMessageDeletedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordMessageDeletedMessage"/> class.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="messageId">The message's id.</param>
        public DiscordMessageDeletedMessage(string channelId, string messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }

        /// <summary>
        /// Gets the id of the channel the message was in.
        /// </summary>
        public string ChannelId { get; }

        /// <summary>
        /// Gets the id of the deleted message.
        /// </summary>
        public string MessageId { get; }
    }
}
