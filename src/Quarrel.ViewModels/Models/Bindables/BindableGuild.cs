﻿// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Gateway.Guild;
using Quarrel.ViewModels.Messages.Services.Settings;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Models.Interfaces;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.Services.Clipboard;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.DispatcherHelper;
using Quarrel.ViewModels.Services.Navigation;
using Quarrel.ViewModels.Services.Settings;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Quarrel.ViewModels.Models.Bindables
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="Guild"/> model.
    /// </summary>
    public class BindableGuild : BindableModelBase<Guild>, IGuildListItem
    {
        private ObservableCollection<BindableChannel> _channels;
        private Permissions _permissions = null;
        private int _position;
        private bool _muted;
        private RelayCommand _copyId;
        private RelayCommand _openGuildSettings;
        private RelayCommand _markAsRead;
        private RelayCommand _muteGuild;
        private RelayCommand _addChanneleCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableGuild"/> class.
        /// </summary>
        /// <param name="model">The base <see cref="Guild"/> object.</param>
        public BindableGuild([NotNull] Guild model) : base(model)
        {
            _channels = new ObservableCollection<BindableChannel>();

            MessengerInstance.Register<GatewayGuildUpdatedMessage>(this, m =>
            {
                if (m.Guild.Id == Model.Id)
                {
                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        Model = m.Guild;
                        RaisePropertyChanged(nameof(DisplayText));
                        RaisePropertyChanged(nameof(HasIcon));
                        RaisePropertyChanged(nameof(IconUrl));
                    });
                }
            });

            MessengerInstance.Register<GatewayMessageRecievedMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    RaisePropertyChanged(nameof(NotificationCount));
                    RaisePropertyChanged(nameof(IsUnread));
                    RaisePropertyChanged(nameof(ShowUnread));
                });
            });

            MessengerInstance.Register<GatewayMessageAckMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    RaisePropertyChanged(nameof(NotificationCount));
                    RaisePropertyChanged(nameof(IsUnread));
                    RaisePropertyChanged(nameof(ShowUnread));
                });
            });

            MessengerInstance.Register<GatewayUserGuildSettingsUpdatedMessage>(this, m =>
            {
                if ((m.Settings.GuildId ?? "DM") == Model.Id)
                {
                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        if (GuildsService.GuildSettings.TryGetValue(Model.Id, out var guildSetting))
                        {
                            IsMuted = guildSetting.Muted;
                        }
                    });
                }
            });

            MessengerInstance.Register<SettingChangedMessage<bool>>(this, m =>
            {
                if (m.Key == SettingKeys.ServerMuteIcons)
                {
                    RaisePropertyChanged(nameof(ShowMute));
                }
            });
        }

        /// <summary>
        /// Gets or sets the Guild's position in the guild list.
        /// </summary>
        public int Position
        {
            get => _position;
            set => Set(ref _position, value);
        }

        /// <inheritdoc/>
        public bool IsCollapsed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the guild is muted.
        /// </summary>
        public bool IsMuted
        {
            get => _muted;
            set
            {
                if (Set(ref _muted, value))
                {
                    MuteGuild(value);
                    RaisePropertyChanged(nameof(ShowMute));
                    RaisePropertyChanged(nameof(ShowUnread));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether not the mute icon should be shown in on top of the Guild icon.
        /// </summary>
        public bool ShowMute => IsMuted && SettingsService.Roaming.GetValue<bool>(SettingKeys.ServerMuteIcons);

        /// <summary>
        /// Gets the text to show in the text block if any.
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (IsDM)
                {
                    return "";
                }
                else
                {
                    return string.Concat(Model.Name.Split(' ').Select(s => StringInfo.GetNextTextElement(s, 0)).ToArray());
                }
            }
        }

        /// <summary>
        /// Gets the url for the guild icon.
        /// </summary>
        public string IconUrl => $"https://cdn.discordapp.com/icons/{Model.Id}/{Model.Icon}.png?size=128";

        /// <summary>
        /// Gets a value indicating whether or not the guild has an icon.
        /// </summary>
        public bool HasIcon => !string.IsNullOrEmpty(Model.Icon);

        /// <summary>
        /// Gets a value indicating whether or not the guild constains any unread channels.
        /// </summary>
        public bool IsUnread
        {
            get => Channels.Any(x => x.IsUnread);
        }

        /// <inheritdoc/>
        public bool ShowUnread
        {
            get => Channels.Any(x => x.ShowUnread) && !IsMuted && NotificationCount == 0;
        }

        /// <summary>
        /// Gets the number of notications in the guild.
        /// </summary>
        public int NotificationCount
        {
            get
            {
                int total = 0;
                foreach (var channel in Channels)
                {
                    total += channel.ReadState != null ? channel.ReadState.MentionCount : 0;
                }

                return total;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the guild is the DM guild.
        /// </summary>
        public bool IsDM => Model.Id == "DM";

        /// <summary>
        /// Gets a value indicating whether or not the current user owns the guild.
        /// </summary>
        public bool IsOwner { get => CurrentUsersService.CurrentUser.Model.Id == Model.OwnerId; }

        /// <summary>
        /// Gets the current user's permissions in the guild.
        /// </summary>
        /// <remarks>
        /// Order
        /// Add allows for @everyone role.
        /// Add allows for each role.
        /// </remarks>
        public Permissions Permissions
        {
            get
            {
                if (_permissions != null)
                {
                    return _permissions;
                }

                if (Model.Id == "DM" || Model.OwnerId == CurrentUsersService.CurrentUser.Model.Id)
                {
                    return new Permissions(int.MaxValue);
                }

                // Role Id == Model.Id for @everyone
                Permissions perms = new Permissions(Model.Roles.FirstOrDefault(x => x.Id == Model.Id).Permissions);

                // TODO: Easier access to CurrentGuildMember
                BindableGuildMember member = new BindableGuildMember(Model.Members.FirstOrDefault(x => x.User.Id == CurrentUsersService.CurrentUser.Model.Id), Model.Id);

                if (member == null)
                {
                    return perms;
                }

                if (member.Roles != null)
                {
                    foreach (var role in member.Roles)
                    {
                        perms.AddAllows((GuildPermission)role.Permissions);
                    }
                }

                _permissions = perms;
                return perms;
            }
        }

        /// <summary>
        /// Gets or sets an observable collection of channels in the guild.
        /// </summary>
        public ObservableCollection<BindableChannel> Channels
        {
            get => _channels;
            set => Set(ref _channels, value);
        }

        /// <summary>
        /// Gets a command that navigates to the page for adding channels to this guild.
        /// </summary>
        public RelayCommand AddChannelCommand =>
        _addChanneleCommand ?? (_addChanneleCommand = new RelayCommand(() =>
        {
            SubFrameNavigationService.NavigateTo("AddChannelPage", Model);
        }));

        /// <summary>
        /// Gets a command that mutes this guild.
        /// </summary>
        public RelayCommand MuteGuildCommand => _muteGuild = new RelayCommand(() =>
        {
            MuteGuild(!IsMuted);
        });

        /// <summary>
        /// Gets a command that marks all channels in this guild as read.
        /// </summary>
        public RelayCommand MarkAsRead => _markAsRead = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.AckGuild(Model.Id);
        });

        /// <summary>
        /// Gets a command that opens this guild's settings.
        /// </summary>
        public RelayCommand OpenGuildSettings => _openGuildSettings = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("GuildSettingsPage", this);
            AnalyticsService.Log(Constants.Analytics.Events.OpenGuildSettings);
        });

        /// <summary>
        /// Gets a command that copies the guild id to the clipboard.
        /// </summary>
        public RelayCommand CopyId => _copyId = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<IClipboardService>().CopyToClipboard(Model.Id);
        });

        private IAnalyticsService AnalyticsService { get; } = SimpleIoc.Default.GetInstance<IAnalyticsService>();

        private IDiscordService DiscordService { get; } = SimpleIoc.Default.GetInstance<IDiscordService>();

        private ISettingsService SettingsService { get; } = SimpleIoc.Default.GetInstance<ISettingsService>();

        private ICurrentUserService CurrentUsersService { get; } = SimpleIoc.Default.GetInstance<ICurrentUserService>();

        private IGuildsService GuildsService { get; } = SimpleIoc.Default.GetInstance<IGuildsService>();

        private ISubFrameNavigationService SubFrameNavigationService { get; } = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();

        private IDispatcherHelper DispatcherHelper { get; } = SimpleIoc.Default.GetInstance<IDispatcherHelper>();

        private async void MuteGuild(bool mute)
        {
            GuildSettingModify guildSettingModify = new GuildSettingModify();
            guildSettingModify.GuildId = Model.Id;
            guildSettingModify.Muted = mute;

            await SimpleIoc.Default.GetInstance<IDiscordService>().UserService.ModifyGuildSettings(guildSettingModify.GuildId, guildSettingModify);
        }
    }
}
