// ------------------------------------------------------------------------------------------------------
// <copyright file="EthereumWalletStats.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Nomis.Aave.Interfaces.Responses;
using Nomis.Aave.Interfaces.Stats;
using Nomis.Blockchain.Abstractions.Stats;
using Nomis.Dex.Abstractions.Contracts;
using Nomis.Dex.Abstractions.Stats;
using Nomis.HapiExplorer.Interfaces.Responses;
using Nomis.HapiExplorer.Interfaces.Stats;
using Nomis.Utils.Contracts.Stats;

// ReSharper disable InconsistentNaming
namespace Nomis.Etherscan.Interfaces.Models
{
    /// <summary>
    /// Ethereum wallet stats.
    /// </summary>
    public sealed class EthereumWalletStats :
        BaseEvmWalletStats<EthereumTransactionIntervalData>,
        IWalletDexTokenSwapPairsStats,
        IWalletNftStats,
        IWalletHapiStats,
        IWalletAaveStats
    {
        /// <inheritdoc/>
        public override string NativeToken => "ETH";

        /// <inheritdoc/>
        [Display(Description = "Total NFTs on wallet", GroupName = "number")]
        public int NftHolding { get; set; }

        /// <inheritdoc/>
        [Display(Description = "NFT trading activity", GroupName = "Native token")]
        public decimal NftTrading { get; set; }

        /// <inheritdoc/>
        [Display(Description = "NFT worth on wallet", GroupName = "Native token")]
        public decimal NftWorth { get; set; }

        /// <inheritdoc/>
        [Display(Description = "The HAPI protocol risk score data", GroupName = "value")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HapiProxyRiskScoreResponse? HapiRiskScore { get; set; }

        /// <inheritdoc/>
        [Display(Description = "DEX tokens balances", GroupName = "collection")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<DexTokenSwapPairsData>? DexTokensSwapPairs { get; set; }

        /// <inheritdoc/>
        [Display(Description = "The Aave protocol user account data", GroupName = "value")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AaveUserAccountDataResponse? AaveData { get; set; }

        /// <inheritdoc cref="BaseEvmWalletStats{TTransactionIntervalData}.AdjustingScoreMultipliers"/>
        [JsonIgnore]
        public override IEnumerable<Func<double>> AdjustingScoreMultipliers => base.AdjustingScoreMultipliers
            .Union(new List<Func<double>>
            {
                () => (this as IWalletHapiStats).CalculateAdjustingScoreMultiplier()
            });

        /// <inheritdoc cref="IWalletCommonStats{TTransactionIntervalData}.ExcludedStatDescriptions"/>
        [JsonIgnore]
        public override IEnumerable<string> ExcludedStatDescriptions =>
            base.ExcludedStatDescriptions
                .Union(new List<string>
                {
                    nameof(DexTokensSwapPairs),
                    nameof(HapiRiskScore),
                    nameof(AaveData)
                });
    }
}