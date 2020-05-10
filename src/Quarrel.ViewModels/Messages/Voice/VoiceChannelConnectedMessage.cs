// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Channels;

namespace Quarrel.ViewModels.Messages.Voice
{
    /// <summary>
    /// A message that indicates a user has connected to a voice channel.
    /// </summary>
    public sealed class VoiceChannelConnectedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChannelConnectedMessage"/> class.
        /// </summary>
        /// <param name="channel">The connected channel.</param>
        public VoiceChannelConnectedMessage(BindableChannel channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// Gets speaking status.
        /// </summary>
        public BindableChannel Channel { get; private set; }
    }
}
