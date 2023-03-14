// ------------------------------------------------------------------------------------------------------
// <copyright file="CoinbaseSpotPriceResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Etherscan.Interfaces.Models;

namespace Nomis.Etherscan.Interfaces.Responses
{
    /// <summary>
    /// Coinbase spot price response.
    /// </summary>
    public class CoinbaseSpotPriceResponse
    {
        /// <inheritdoc cref="CoinbaseSpotPriceData"/>
        [JsonPropertyName("data")]
        public CoinbaseSpotPriceData? Data { get; set; }
    }
}