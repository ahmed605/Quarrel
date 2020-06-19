// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;

namespace Quarrel.ViewModels.Services.Discord.Voice
{
    /// <summary>
    /// Manages all voice state data.
    /// </summary>
    public interface IVoiceService
    {
        /// <summary>
        /// Toggles if the user is deafend.
        /// </summary>
        void ToggleDeafen();

        /// <summary>
        /// Toggles if the user is muted.
        /// </summary>
        void ToggleMute();

        /// <summary>
        /// Gets a <see cref="VoiceState"/> from the voice service.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns>The <see cref="VoiceState"/>.</returns>
        VoiceState GetVoiceState(string userId);

        /// <summary>
        /// Adds or updates a <see cref="VoiceState"/> in the voice service.
        /// </summary>
        /// <param name="voiceState">The <see cref="VoiceState"/>.</param>
        void AddOrUpdateVoiceState(VoiceState voiceState);

        /// <summary>
        /// Removes a <see cref="VoiceState"/> from the voice service.
        /// </summary>
        /// <param name="userId">The user id of the <see cref="VoiceState"/>.</param>
        void RemoveVoiceState(string userId);

        /// <summary>
        /// Connects to a voice channel.
        /// </summary>
        /// <param name="data">The voice server data.</param>
        /// <param name="state">The current user's voice state.</param>
        void ConnectToVoiceChannel(VoiceServerUpdate data, VoiceState state = null)
    }
}
