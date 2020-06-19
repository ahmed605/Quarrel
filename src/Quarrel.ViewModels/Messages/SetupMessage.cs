// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Gateway.DownstreamEvents;

namespace Quarrel.ViewModels.Messages
{
    /// <summary>
    /// A message that indicates the final steps of app setup should occur.
    /// </summary>
    public class SetupMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetupMessage"/> class.
        /// </summary>
        /// <param name="ready">The ready message.</param>
        public SetupMessage(Ready ready)
        {
            ReadyMessage = ready;
        }

        /// <summary>
        /// The ready message from the Gateway.
        /// </summary>
        public Ready ReadyMessage { get; }
    }
}
