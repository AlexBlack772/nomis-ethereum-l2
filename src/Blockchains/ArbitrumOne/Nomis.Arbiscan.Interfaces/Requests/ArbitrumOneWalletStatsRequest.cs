// ------------------------------------------------------------------------------------------------------
// <copyright file="ArbitrumOneWalletStatsRequest.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Nomis.Aave.Interfaces.Contracts;
using Nomis.Blockchain.Abstractions.Requests;

namespace Nomis.Arbiscan.Interfaces.Requests
{
    /// <summary>
    /// Request for getting the wallet stats for Arbitrum One.
    /// </summary>
    public sealed class ArbitrumOneWalletStatsRequest :
        BaseEvmWalletStatsRequest,
        IWalletAaveProtocolRequest
    {
        /// <inheritdoc />
        /// <example>false</example>
        [FromQuery]
        public bool GetAaveProtocolData { get; set; } = false;
    }
}