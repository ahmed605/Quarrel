// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Services.Discord.Presence;

namespace Quarrel.ViewModels.Services.Discord.CurrentUser
{
    /// <summary>
    /// Manages the all information directly pertaining to the current user.
    /// </summary>
    public class CurrentUsersService : ICurrentUserService
    {
        private readonly IPresenceService _presenceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUsersService"/> class.
        /// </summary>
        /// <param name="presenceService">The app's presence service.</param>
        public CurrentUsersService(IPresenceService presenceService)
        {
            _presenceService = presenceService;

            Messenger.Default.Register<GatewayReadyMessage>(this, m =>
            {
                CurrentUser = m.EventData.User;
                DiscordAPI.Models.Presence presence = new DiscordAPI.Models.Presence()
                {
                    User = null,
                    Game = null,
                    GuildId = null,
                    Roles = null,
                    Status = m.EventData.Settings.Status,
                };

                CurrentUserSettings = m.EventData.Settings;
                _presenceService.UpdateUserPrecense(CurrentUser.Id, presence);
            });
            Messenger.Default.Register<GatewayUserSettingsUpdatedMessage>(this, m =>
            {
                CurrentUserSettings = m.Settings;

                if (!string.IsNullOrEmpty(m.Settings.Status))
                {
                    var newPresence = new DiscordAPI.Models.Presence()
                    {
                        Status = m.Settings.Status,
                    };
                    _presenceService.UpdateUserPrecense(CurrentUser.Id, newPresence);
                }
            });
        }

        /// <inheritdoc/>
        public User CurrentUser { get; private set; }

        /// <inheritdoc/>
        public UserSettings CurrentUserSettings { get; private set; } = new UserSettings();
    }
}
