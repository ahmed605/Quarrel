// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Services.Settings;
using Quarrel.ViewModels.Services.Licensing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Services.Store;

namespace Quarrel.Services.Licensing
{
    /// <summary>
    /// A service that gets store licensing information.
    /// </summary>
    public class LicenseService : ILicenseService
    {
        private StoreContext _storeContext = StoreContext.GetDefault();
        private bool purchasedPremium;
        private bool legacyPurchase;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseService"/> class.
        /// </summary>
        public LicenseService()
        {
            Initialize();
        }

        /// <inheritdoc/>
        public bool HasQuarrelPremium => purchasedPremium || legacyPurchase;

        private async void Initialize()
        {
            purchasedPremium = await HasPurchasedProduct(Constants.Meta.Store.QuarrelPremiumIAPId);
            legacyPurchase = await HasPurchasedAnyProduct(
                new string[]
                {
                    Constants.Meta.Store.RemoveAdsIAPId,
                    Constants.Meta.Store.RemoveAdsIAPId2,
                    Constants.Meta.Store.PoliteDontationIAPId,
                    Constants.Meta.Store.OMGTHXDonationIAPId,
                    Constants.Meta.Store.SignificantDonationIAPId,
                    Constants.Meta.Store.RidiculousDonationIAPId,
                });
        }

        private async Task<bool> HasPurchasedProduct(string productId)
        {
            StoreAppLicense license = await _storeContext.GetAppLicenseAsync();
            return license.AddOnLicenses[productId].IsActive;
        }

        private async Task<bool> HasPurchasedAnyProduct(IEnumerable<string> productIds)
        {
            StoreAppLicense license = await _storeContext.GetAppLicenseAsync();
            foreach (var id in productIds)
            {
                if (license.AddOnLicenses.ContainsKey(id) && license.AddOnLicenses[id].IsActive)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
