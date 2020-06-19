// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using DiscordAPI.Models.Channels;
using DiscordAPI.Models.Guilds;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway.Voice;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Quarrel.ViewModels.Services.Discord.Guilds
{
    /// <summary>
    /// Manages all guild information.
    /// </summary>
    public class GuildsService : IGuildsService
    {
        private IDictionary<string, Guild> _allGuilds = new ConcurrentDictionary<string, Guild>();

        private IDictionary<string, ConcurrentDictionary<string, GuildMember>> _guildUsers = new ConcurrentDictionary<string, ConcurrentDictionary<string, GuildMember>>();

        private IDictionary<string, GuildSetting> _guildSettings = new ConcurrentDictionary<string, GuildSetting>();

        private IDictionary<string, Permissions> _guildPermissions = new ConcurrentDictionary<string, Permissions>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildsService"/> class.
        /// </summary>
        /// <param name="analyticsService">The app's analytics service.</param>
        /// <param name="cacheService">The app's cache service.</param>
        /// <param name="channelsService">The app's channel service.</param>
        /// <param name="presenceService">The app's presence service.</param>
        /// <param name="dispatcherHelper">The app's dispatcher helper.</param>
        public GuildsService()
        {
        }

        /// <inheritdoc/>
        public Guild GetGuild(string guildId)
        {
            if (guildId == null)
            {
                return null;
            }

            return _allGuilds.TryGetValue(guildId, out Guild guild) ? guild : null;
        }

        /// <inheritdoc/>
        public void AddOrUpdateGuild(Guild guild)
        {
            _allGuilds.AddOrUpdate(guild.Id, guild);
            _guildUsers.Add(guild.Id, new ConcurrentDictionary<string, GuildMember>());
        }

        /// <inheritdoc/>
        public void RemoveGuild(string guildId)
        {
            _allGuilds.Remove(guildId);
        }

        /// <inheritdoc/>
        public Permissions GetGuildPermissions(string guildId)
        {
            if (guildId == null)
            {
                return null;
            }

            return _guildPermissions.TryGetValue(guildId, out Permissions perms) ? perms : null;
        }

        /// <inheritdoc/>
        public void AddOrUpdateChannel(GuildChannel channel)
        {
            Guild guild = _allGuilds[channel.GuildId];
            int index = guild.Channels.IndexOf(x => x.Id == channel.Id);
            if (index == -1)
            {
                guild.Channels.Add(channel);
            }
            else
            {
                guild.Channels.RemoveAt(index);
                guild.Channels.Add(channel);
            }
        }

        /// <inheritdoc/>
        public void RemoveChannel(string channelId, string guildId)
        {
            Guild guild = _allGuilds[guildId];
            int index = guild.Channels.IndexOf(x => x.Id == channelId);
            if (index != -1)
            {
                guild.Channels.RemoveAt(index);
            }
        }

        /// <inheritdoc/>
        public GuildSetting GetGuildSetting(string guildId)
        {
            if (guildId == null)
            {
                return null;
            }

            return _guildSettings.TryGetValue(guildId, out GuildSetting guildSetting) ? guildSetting : null;
        }

        /// <inheritdoc/>
        public void AddOrUpdateGuildSettings(string guildId, GuildSetting guildSetting)
        {
            _guildSettings.AddOrUpdate(guildId, guildSetting);
        }

        /// <inheritdoc/>
        public GuildMember GetGuildMember(string memberId, string guildId)
        {
            if (_guildUsers.TryGetValue(guildId, out var guild) && guild.TryGetValue(memberId, out GuildMember member))
            {
                return member;
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public GuildMember GetGuildMember(string username, string discriminator, string guildId)
        {
            if (_guildUsers.TryGetValue(guildId, out ConcurrentDictionary<string, GuildMember> value))
            {
                return value.Values.FirstOrDefault(x => x.User.Username == username && x.User.Discriminator == discriminator);
            }

            return null;
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, GuildMember> GetAndRequestGuildMembers(IEnumerable<string> memberIds, string guildId)
        {
            Dictionary<string, GuildMember> guildMembers = new Dictionary<string, GuildMember>();
            List<string> guildMembersToBeRequested = new List<string>();
            if (_guildUsers.TryGetValue(guildId, out var guild))
            {
                foreach (string memberId in memberIds)
                {
                    if (guild.TryGetValue(memberId, out GuildMember member))
                    {
                        guildMembers.Add(memberId, member);
                    }
                    else
                    {
                        guildMembersToBeRequested.Add(memberId);
                    }
                }

                if (guildMembersToBeRequested.Count > 0)
                {
                    Messenger.Default.Send(new GatewayRequestGuildMembersMessage(new List<string> { guildId }, guildMembersToBeRequested));
                }

                return guildMembers;
            }

            return null;
        }

        /// <inheritdoc/>
        public void AddOrUpdateGuildMember(GuildMember member, string guildId)
        {
            _guildUsers[guildId].AddOrUpdate(member.User.Id, member);
        }

        /// <inheritdoc/>
        public IEnumerable<GuildMember> QueryGuildMembers(string query, string guildId, int take = 10)
        {
            return _guildUsers[guildId].Values.Where(x => x.DisplayName().ToLower().StartsWith(query.ToLower()) || x.User.Username.ToLower().StartsWith(query.ToLower())).Take(take);
        }
    }
}
