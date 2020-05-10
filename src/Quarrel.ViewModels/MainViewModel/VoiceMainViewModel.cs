// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using Quarrel.ViewModels.Messages.Voice;
using Quarrel.ViewModels.Models.Bindables.Channels;

namespace Quarrel.ViewModels
{
    /// <summary>
    /// The ViewModel for all data throughout the app.
    /// </summary>
    public partial class MainViewModel
    {
        private BindableChannel _currentVoiceChannel;
        private VoiceState voiceState = new VoiceState();

        /// <summary>
        /// Gets or sets the currently conncted voice channel.
        /// </summary>
        public BindableChannel CurrentVoiceChannel
        {
            get => _currentVoiceChannel;
            set => Set(ref _currentVoiceChannel, value);
        }

        /// <summary>
        /// Gets or sets the current user's voice state.
        /// </summary>
        public VoiceState VoiceState
        {
            get => voiceState;
            set => Set(ref voiceState, value);
        }

        private void RegisterVoiceMessages()
        {
            MessengerInstance.Register<VoiceChannelConnectedMessage>(this, m =>
            {
                CurrentVoiceChannel = m.Channel;
            });
        }
    }
}
