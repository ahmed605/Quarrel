// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables.Users;
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

        private IDictionary<string, GuildMember> _dmUsers = new ConcurrentDictionary<string, GuildMember>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendsService"/> class.
        /// </summary>
        public FriendsService()
        {
            Messenger.Default.Register<GatewayReadyMessage>(this, m =>
            {
                foreach (var channel in m.EventData.PrivateChannels)
                {
                    foreach (var user in channel.Users)
                    {
                        if (!_dmUsers.ContainsKey(user.Id))
                        {
                            _dmUsers.Add(user.Id, new GuildMember() { User = user });
                        }
                    }
                }
            });
        }
    }
}
