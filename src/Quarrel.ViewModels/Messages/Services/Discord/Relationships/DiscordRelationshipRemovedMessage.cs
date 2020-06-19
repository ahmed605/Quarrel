// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Services.Discord.Relationships
{
    /// <summary>
    /// A message to mark a relationship being removed.
    /// </summary>
    public class DiscordRelationshipRemovedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordRelationshipRemovedMessage"/> class.
        /// </summary>
        /// <param name="friend">The relationship data.</param>
        public DiscordRelationshipRemovedMessage(Friend friend)
        {
            Friend = friend;
        }

        /// <summary>
        /// Gets the removed friend.
        /// </summary>
        public Friend Friend { get; }
    }
}
