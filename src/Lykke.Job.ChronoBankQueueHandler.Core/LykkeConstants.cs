﻿using System;

namespace Lykke.Job.ChronoBankQueueHandler.Core
{
    public static class LykkeConstants
    {
        public const string BitcoinAssetId = "BTC";
        public const string LykkeAssetId = "LKK";

        public const string UsdAssetId = "USD";
        public const string EurAssetId = "EUR";
        public const string ChfAssetId = "CHF";
        public const string GbpAssetId = "GBP";
        public const string EthAssetId = "ETH";
        public const string SolarAssetId = "SLR";
        public const string ChronoBankAssetId = "TIME";
        public const string QuantaAssetId = "QNT";

        public const string LKKUSDPairId = "LKKUSD";

        public const string DefaultAssetSetting = "Default";

        public const int TotalLykkeAmount = 1250000000;

        public const int MinPwdLength = 6;
        public const int MaxPwdLength = 100;

        public const int DefaultRefundTimeoutDays = 30;

        public static readonly string[] MarginAccounts = { "USD", "CHF", "EUR" };
        public const double DefaultMarginCall = 0.8;
        public const double DefaultStopOut = 0.95;

        public static readonly TimeSpan SessionLifetime = TimeSpan.FromDays(3);
        public static readonly TimeSpan SessionRefreshPeriod = TimeSpan.FromDays(1);

        #region Cache keys

        public const string LastAskBidForAssetOnPeriod = "__Asset_{0}_Last_ask{1}_{2}__";

        #endregion

        public static string GetLastAskForAssetOnPeriodKey(string assetPairId, string period, bool ask)
        {
            return string.Format(LastAskBidForAssetOnPeriod, assetPairId, ask, period);
        }
    }
}