// ------------------------------------------------------------------------------------------------------
// <copyright file="ZkSyncWalletStats.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions.Stats;

namespace Nomis.Zkscan.Interfaces.Models
{
    /// <summary>
    /// ZkSync wallet stats.
    /// </summary>
    public sealed class ZkSyncWalletStats :
        BaseEvmWalletStats<ZkSyncTransactionIntervalData>
    {
        /// <inheritdoc/>
        public override string NativeToken => "ETH";
    }
}