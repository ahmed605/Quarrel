// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Services.Discord.Relationships
{
    /// <summary>
    /// A message to mark a relationship being updated.
    /// </summary>
    public class DiscordRelationshipUpdatedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordRelationshipUpdatedMessage"/> class.
        /// </summary>
        /// <param name="friend">The relationship data.</param>
        public DiscordRelationshipUpdatedMessage(Friend friend)
        {
            Friend = friend;
        }

        /// <summary>
        /// Gets the updated friend.
        /// </summary>
        public Friend Friend { get; }
    }
}
