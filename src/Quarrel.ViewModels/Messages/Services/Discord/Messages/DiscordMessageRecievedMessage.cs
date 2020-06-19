// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models.Messages;

namespace Quarrel.ViewModels.Messages.Services.Discord.Messages
{
    /// <summary>
    /// A message sent when a message is recieved.
    /// </summary>
    public class DiscordMessageRecievedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordMessageRecievedMessage"/> class.
        /// </summary>
        /// <param name="message">The message recieved.</param>
        public DiscordMessageRecievedMessage(Message message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the message recieved.
        /// </summary>
        public Message Message { get; }
    }
}
