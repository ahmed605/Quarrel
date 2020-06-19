// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using DiscordAPI.Models.Channels;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
            if (channelId == null)
            {
                return;
            }

            _allChannels.AddOrUpdate(channelId, channel);
        }

        /// <inheritdoc/>
        public void RemoveChannel(string channelId)
        {
            _allChannels.Remove(channelId);
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
