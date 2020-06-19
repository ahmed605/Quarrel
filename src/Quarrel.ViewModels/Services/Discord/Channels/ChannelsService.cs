// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using DiscordAPI.Models.Channels;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Quarrel.ViewModels.Services.Discord.Channels
{
    /// <summary>
    /// Manages the all channels the client has access to.
    /// </summary>
    public class ChannelsService : IChannelsService
    {
        private IDictionary<string, Channel> _allChannels = new ConcurrentDictionary<string, Channel>();
        private IDictionary<string, ChannelOverride> _channelSettings = new ConcurrentDictionary<string, ChannelOverride>();
        private IDictionary<string, ReadState> _readStates = new ConcurrentDictionary<string, ReadState>();
        private IDictionary<string, Permissions> _permissions = new ConcurrentDictionary<string, Permissions>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelsService"/> class.
        /// </summary>
        public ChannelsService()
        {
        }

        /// <inheritdoc/>
        public Channel GetChannel(string channelId)
        {
            if (channelId == null)
            {
                return null;
            }

            return _allChannels.TryGetValue(channelId, out Channel channel) ? channel : null;
        }

        /// <inheritdoc/>
        public void AddOrUpdateChannel(Channel channel)
        {
            _allChannels.AddOrUpdate(channel.Id, channel);

            if (channel is GuildChannel guildChannel)
            {
                Permissions root = null; // TODO: Get guild permissions

                IEnumerable<Overwrite> overwrites =
                    guildChannel.PermissionOverwrites.Where(x =>
                    (x.Type == "role" &&
                    (x.Id == guildChannel.GuildId || GuildsService.GetGuildMember(CurrentUsersService.CurrentUser.Id, GuildId).Roles.Contains(x.Id)))
                    || (x.Type == "member" && x.Id == CurrentUsersService.CurrentUser.Id));

                _permissions.Add(channel.Id, Permissions.CalculatePermissionOverwrites(root, overwrites, Guild.OwnerId == CurrentUsersService.CurrentUser.Id));
            }
        }

        /// <inheritdoc/>
        public void RemoveChannel(string channelId)
        {
            _allChannels.Remove(channelId);
        }

        /// <inheritdoc/>
        public Permissions GetChannelPermissions(string channelId)
        {
            if (channelId == null)
            {
                return null;
            }

            return _permissions.TryGetValue(channelId, out Permissions perms) ? perms : null;
        }

        /// <inheritdoc/>
        public ChannelOverride GetChannelSettings(string channelId)
        {
            if (channelId == null)
            {
                return null;
            }

            return _channelSettings.TryGetValue(channelId, out ChannelOverride channelOverride) ? channelOverride : null;
        }

        /// <inheritdoc/>
        public void AddOrUpdateChannelSettings(string channelId, ChannelOverride channelOverride)
        {
            if (channelId == null)
            {
                return;
            }

            _channelSettings.AddOrUpdate(channelId, channelOverride);
        }

        /// <inheritdoc/>
        public ReadState GetReadState(string channelId)
        {
            if (channelId == null)
            {
                return null;
            }

            return _readStates.TryGetValue(channelId, out ReadState state) ? state : null;
        }

        /// <inheritdoc/>
        public void AddOrUpdateReadState(string channelId, ReadState readState)
        {
            if (channelId == null)
            {
                return;
            }

            _readStates.AddOrUpdate(channelId, readState);
        }

        /// <inheritdoc/>
        public void RemoveReadState(string channelId)
        {
            _readStates.Remove(channelId);
        }
    }
}
