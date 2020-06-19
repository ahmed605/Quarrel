// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Services.Discord.Friends
{
    /// <summary>
    /// Manages all relationship status with the current user.
    /// </summary>
    public class FriendsService : IFriendsService
    {
        private IDictionary<string, Friend> _friends = new ConcurrentDictionary<string, Friend>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendsService"/> class.
        /// </summary>
        public FriendsService()
        {
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<Friend> Friends => _friends.Values.AsReadOnly();

        /// <inheritdoc/>
        public Friend GetFriend(string userId)
        {
            if (userId == null)
            {
                return null;
            }

            return _friends.TryGetValue(userId, out Friend friend) ? friend : null;
        }

        /// <inheritdoc/>
        public void AddOrUpdateFriend(string userId, Friend friend)
        {
            _friends.AddOrUpdate(userId, friend);
        }

        /// <inheritdoc/>
        public void RemoveFriend(string userId)
        {
            _friends.Remove(userId);
        }
    }
}
