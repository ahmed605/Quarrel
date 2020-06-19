// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Services.Discord.Friends
{
    /// <summary>
    /// Manages all relationship status with the current user.
    /// </summary>
    public interface IFriendsService
    {
        /// <summary>
        /// Gets an enumerable list of friends.
        /// </summary>
        IReadOnlyCollection<Friend> Friends { get; }

        /// <summary>
        /// Gets a <see cref="Friend"/> from the friends service.
        /// </summary>
        /// <param name="userId">The friend's user id.</param>
        /// <returns>The <see cref="Friend"/>.</returns>
        Friend GetFriend(string userId);

        /// <summary>
        /// Adds or updates a <see cref="Friend"/> in the friends service.
        /// </summary>
        /// <param name="userId">The friend's user id.</param>
        /// <param name="friend">The new friend object.</param>
        void AddOrUpdateFriend(string userId, Friend friend);

        /// <summary>
        /// Removes a friend from the friends service.
        /// </summary>
        /// <param name="userId">The friend's user id.</param>
        void RemoveFriend(string userId);
    }
}
