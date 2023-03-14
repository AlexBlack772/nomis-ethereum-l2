// ------------------------------------------------------------------------------------------------------
// <copyright file="CoinbaseSpotPriceData.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Nomis.Etherscan.Interfaces.Models
{
    /// <summary>
    /// Coinbase spot price data.
    /// </summary>
    public class CoinbaseSpotPriceData
    {
        /// <summary>
        /// Base token.
        /// </summary>
        [JsonPropertyName("base")]
        public string? Base { get; set; }

        /// <summary>
        /// Currency.
        /// </summary>
        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        /// <summary>
        /// Amount.
        /// </summary>
        [JsonPropertyName("amount")]
        public string? Amount { get; set; }
    }
}