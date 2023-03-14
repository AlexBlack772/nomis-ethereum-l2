// ------------------------------------------------------------------------------------------------------
// <copyright file="GnosisWalletStatsRequest.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Nomis.Blockchain.Abstractions.Requests;
using Nomis.Dex.Abstractions.Contracts;

namespace Nomis.Gnosisscan.Interfaces.Requests
{
    /// <summary>
    /// Request for getting the wallet stats for Gnosis.
    /// </summary>
    public sealed class GnosisWalletStatsRequest :
        BaseEvmWalletStatsRequest,
        IWalletTokensSwapPairsRequest
    {
        /// <inheritdoc />
        /// <example>false</example>
        [FromQuery]
        public bool GetTokensSwapPairs { get; set; } = false;

        /// <inheritdoc />
        /// <example>100</example>
        [FromQuery]
        [Range(typeof(int), "1", "1000")]
        public int FirstSwapPairs { get; set; } = 100;

        /// <inheritdoc />
        /// <example>0</example>
        [FromQuery]
        [Range(typeof(int), "0", "2147483647")]
        public int Skip { get; set; } = 0;
    }
}