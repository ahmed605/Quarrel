// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Services.Licensing
{
    /// <summary>
    /// An implementation of the <see cref="ILicenseService"/> for the debug environment.
    /// </summary>
    public class FakeLicenseService : ILicenseService
    {
        /// <inheritdoc/>
        public bool HasQuarrelPremium => true;
    }
}
