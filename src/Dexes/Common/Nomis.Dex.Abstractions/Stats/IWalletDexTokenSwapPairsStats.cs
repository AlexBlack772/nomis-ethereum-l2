﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="IWalletDexTokenSwapPairsStats.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Dex.Abstractions.Contracts;
using Nomis.Utils.Contracts.Stats;

namespace Nomis.Dex.Abstractions.Stats
{
    /// <summary>
    /// Wallet DEX token swap pairs stats.
    /// </summary>
    public interface IWalletDexTokenSwapPairsStats :
        IWalletStats
    {
        /// <summary>
        /// Set wallet DEX token swap pairs stats.
        /// </summary>
        /// <typeparam name="TWalletStats">The wallet stats type.</typeparam>
        /// <param name="stats">The wallet stats.</param>
        /// <returns>Returns wallet stats with initialized properties.</returns>
        public new TWalletStats FillStatsTo<TWalletStats>(TWalletStats stats)
            where TWalletStats : class, IWalletDexTokenSwapPairsStats
        {
            stats.DexTokensSwapPairs = DexTokensSwapPairs;
            return stats;
        }

        /// <summary>
        /// DEX token swap pairs.
        /// </summary>
        public IEnumerable<DexTokenSwapPairsData>? DexTokensSwapPairs { get; set; }

        /// <summary>
        /// Calculate wallet DEX token balances stats score.
        /// </summary>
        /// <returns>Returns DEX token balances stats score.</returns>
        public new double CalculateScore()
        {
            // TODO - add calculation
            return 0;
        }
    }
}