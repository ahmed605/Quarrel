// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using DiscordAPI.Models.Channels;
using DiscordAPI.Models.Guilds;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Gateway.Guild;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables.Channels;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Models.Interfaces;
using Quarrel.ViewModels.ViewModels.Messages.Gateway;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Quarrel.ViewModels
{
    /// <summary>
    /// The ViewModel for all data throughout the app.
    /// </summary>
    public partial class MainViewModel
    {
        private RelayCommand<IGuildListItem> _navigateGuild;
        private RelayCommand _navigateAddServerPage;
        private BindableGuild _currentGuild;
        private BindableGuildMember _currentGuildMember;

        /// <summary>
        /// Gets a command that sends Messenger Request to change Guild.
        /// </summary>
        public RelayCommand<IGuildListItem> GuildListItemClicked => _navigateGuild = _navigateGuild ?? new RelayCommand<IGuildListItem>(
            (guildListItem) =>
            {
                if (guildListItem is BindableGuild bGuild)
                {
                    CurrentGuild = bGuild;
                }
                else if (guildListItem is BindableGuildFolder bindableGuildFolder)
                {
                    bool collapsed = !bindableGuildFolder.IsCollapsed;
                    bindableGuildFolder.IsCollapsed = collapsed;
                    foreach (var guildId in bindableGuildFolder.Model.GuildIds)
                    {
                        BindableGuild guild = _guildsService.GetGuild(guildId);
                        if (guild != null)
                        {
                            guild.IsCollapsed = collapsed;
                        }
                    }
                }
            });

        /// <summary>
        /// Gets a command that opens the add server page.
        /// </summary>
        public RelayCommand NavigateAddServerPage => _navigateAddServerPage = new RelayCommand(() =>
        {
            _subFrameNavigationService.NavigateTo("AddServerPage");
        });

        /// <summary>
        /// Gets or sets the currently selected guild.
        /// </summary>
        public BindableGuild CurrentGuild
        {
            get => _currentGuild;
            set
            {
                if (_currentGuild != null)
                {
                    _currentGuild.Selected = false;
                }

                if (Set(ref _currentGuild, value))
                {
                    Task.Run(() =>
                    {
                        MessengerInstance.Send(new GuildNavigateMessage(_currentGuild));
                    });
                }

                if (_currentGuild != null)
                {
                    _currentGuild.Selected = true;
                }
            }
        }

        /// <summary>
        /// Gets the current user's BindableGuildMember in the current guild.
        /// </summary>
        public BindableGuildMember CurrentGuildMember
        {
            get => _currentGuildMember;
            private set => Set(ref _currentGuildMember, value);
        }

        /// <summary>
        /// Gets all Guilds the current member is in.
        /// </summary>
        [NotNull]
        public ObservableRangeCollection<IGuildListItem> BindableGuilds { get; private set; } =
            new ObservableRangeCollection<IGuildListItem>();

        /// <summary>
        /// Gets a hashed collection of guilds, by guild id.
        /// </summary>
        public IDictionary<string, BindableGuild> AllGuilds { get; } = new ConcurrentDictionary<string, BindableGuild>();

        /// <summary>
        /// Gets a hashed collection of guild settings, by guild id.
        /// </summary>
        public IDictionary<string, GuildSetting> GuildSettings { get; } =
            new ConcurrentDictionary<string, GuildSetting>();

        private void RegisterGuildsMessages()
        {
            MessengerInstance.Register<SetupMessage>(this, _ =>
            {
                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    // TODO: Load root UI
                });
            });

            MessengerInstance.Register<GatewayReadyMessage>(this, m =>
            {
                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    BindableGuilds.Clear();
                    BindableGuilds.Add(_guildsService.GetGuild("DM"));
                    foreach (var folder in m.EventData.Settings.GuildFolders)
                    {
                        BindableGuildFolder bindableFolder = new BindableGuildFolder(folder) { IsCollapsed = folder.GuildIds.Count() > 1 };
                        BindableGuilds.Add(bindableFolder);
                        foreach (var guildId in folder.GuildIds)
                        {
                            BindableGuild guild = _guildsService.GetGuild(guildId);
                            guild.FolderId = folder.Id;
                            guild.IsCollapsed = bindableFolder.IsCollapsed;
                            BindableGuilds.Add(guild);
                        }
                    }

                    CurrentGuild = dmGuild;
                });
            });

            MessengerInstance.Register<GuildNavigateMessage>(this, m =>
            {
                BindableChannel channel =
                m.Guild.Channels.FirstOrDefault(x => x.IsTextChannel && x.Permissions.ReadMessages);
                BindableGuildMember currentGuildMember;

                if (!m.Guild.IsDM)
                {
                    currentGuildMember = _guildsService.GetGuildMember(_currentUserService.CurrentUser.Model.Id, m.Guild.Model.Id);
                }
                else
                {
                    currentGuildMember = new BindableGuildMember(
                        new GuildMember()
                        {
                            User = _currentUserService.CurrentUser.Model,
                        },
                        "DM",
                        _currentUserService.CurrentUser.Presence);
                }

                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    CurrentChannel = channel;
                    BindableMessages.Clear();

                    if (m.Guild.IsDM)
                    {
                        CurrentBindableMembers.Clear();
                    }

                    CurrentGuildMember = currentGuildMember;
                });

                if (channel != null)
                {
                    MessengerInstance.Send(new ChannelNavigateMessage(channel));
                }

                _analyticsService.Log(
                    m.Guild.IsDM ?
                    Constants.Analytics.Events.OpenDMs :
                    Constants.Analytics.Events.OpenGuild,
                    ("guild-id", m.Guild.Model.Id));
            });
            MessengerInstance.Register<GatewayGuildCreatedMessage>(this, m =>
            {
                BindableGuild guild = new BindableGuild(m.Guild);
                _guildsService.AddOrUpdateGuild(m.Guild.Id, guild);
                _dispatcherHelper.CheckBeginInvokeOnUi(() => { BindableGuilds.Insert(1, guild); });
            });
            MessengerInstance.Register<GatewayGuildDeletedMessage>(this, m =>
            {
                BindableGuild guild;
                if (_guildsService.GetGuild(m.Guild.GuildId) != null)
                {
                    guild = _guildsService.GetGuild(m.Guild.GuildId);
                    _guildsService.RemoveGuild(m.Guild.GuildId);
                    _dispatcherHelper.CheckBeginInvokeOnUi(() => { BindableGuilds.Remove(guild); });
                }
            });
            MessengerInstance.Register<GatewayChannelCreatedMessage>(this, m =>
            {
                string guildId = "DM";
                if (m.Channel is GuildChannel gChannel)
                {
                    guildId = gChannel.GuildId;
                }

                var bChannel = new BindableChannel(m.Channel);
                if (bChannel.Model.Type != 4 && bChannel.ParentId != null)
                {
                    BindableChannel parentChannel = _channelsService.GetChannel(bChannel.ParentId);
                    bChannel.ParentPostion = parentChannel != null ? parentChannel.Position : 0;
                }
                else if (bChannel.ParentId == null)
                {
                    bChannel.ParentPostion = -1;
                }

                BindableGuild guild = _guildsService.GetGuild(guildId);
                if (guild != null)
                {
                    for (int i = 0; i < guild.Channels.Count; i++)
                    {
                        if (guild.Channels[i].AbsolutePostion > bChannel.AbsolutePostion)
                        {
                            _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                            {
                                guild.Channels.Insert(i, bChannel);
                            });
                            break;
                        }
                    }
                }

                _channelsService.AddOrUpdateChannel(bChannel.Model.Id, bChannel);
            });
            MessengerInstance.Register<GatewayChannelDeletedMessage>(this, m =>
            {
                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    BindableChannel currentChannel = _channelsService.GetChannel(m.Channel.Id);
                    if (currentChannel != null)
                    {
                        BindableGuild guild = _guildsService.GetGuild(currentChannel.GuildId);
                        if (guild != null)
                        {
                            guild.Channels.Remove(currentChannel);
                        }

                        _channelsService.RemoveChannel(m.Channel.Id);
                    }
                });
            });
            MessengerInstance.Register<GatewayGuildChannelUpdatedMessage>(this, m =>
            {
                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    var bChannel = _channelsService.GetChannel(m.Channel.Id ?? "DM");
                    bChannel.Model = m.Channel;

                    if (bChannel.Model.Type != 4 && bChannel.ParentId != null)
                    {
                        BindableChannel parentChannel = _channelsService.GetChannel(bChannel.ParentId);
                        bChannel.ParentPostion = parentChannel != null ? parentChannel.Position : 0;
                    }
                    else if (bChannel.ParentId == null)
                    {
                        bChannel.ParentPostion = -1;
                    }

                    BindableGuild guild = _guildsService.GetGuild(m.Channel.GuildId);
                    if (guild != null)
                    {
                        guild.Channels.Remove(bChannel);
                        for (int i = 0; i < guild.Channels.Count; i++)
                        {
                            if (guild.Channels[i].AbsolutePostion > bChannel.AbsolutePostion)
                            {
                                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                                {
                                    guild.Channels.Insert(i, bChannel);
                                });
                                break;
                            }
                        }
                    }
                });
            });
            MessengerInstance.Register<GatewayUserSettingsUpdatedMessage>(this, m =>
            {
                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    if (m.Settings.GuildFolders != null && m.Settings.GuildOrder != null)
                    {
                        // TODO: Handle guild reorder.
                    }
                });
            });
            MessengerInstance.Register<GatewayGuildMembersChunkMessage>(this, m =>
            {
                _guildsService._guildUsers.TryGetValue(m.GuildMembersChunk.GuildId, out var guild);
                foreach (var member in m.GuildMembersChunk.Members)
                {
                    guild.TryAdd(member.User.Id, new BindableGuildMember(member, m.GuildMembersChunk.GuildId));
                }

                if (m.GuildMembersChunk.Presences != null)
                {
                    foreach (var presence in m.GuildMembersChunk.Presences)
                    {
                        _presenceService.AddOrUpdateUserPrecense(presence.User.Id, presence);
                    }
                }
            });
        }
    }
}
