// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Services.Licensing
{
    /// <summary>
    /// An <see langword="interface"/> that gets store licensing information.
    /// </summary>
    public interface ILicenseService
    {
        /// <summary>
        /// Gets a value indicating whether or not the user has Quarrel premium.
        /// </summary>
        bool HasQuarrelPremium { get; }
    }
}
