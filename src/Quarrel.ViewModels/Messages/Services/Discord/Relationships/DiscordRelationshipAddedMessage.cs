// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Services.Discord.Relationships
{
    /// <summary>
    /// A message to mark a relationship being added.
    /// </summary>
    public class DiscordRelationshipAddedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordRelationshipAddedMessage"/> class.
        /// </summary>
        /// <param name="friend">The relationship data.</param>
        public DiscordRelationshipAddedMessage(Friend friend)
        {
            Friend = friend;
        }

        /// <summary>
        /// Gets the new friend.
        /// </summary>
        public Friend Friend { get; }
    }
}
