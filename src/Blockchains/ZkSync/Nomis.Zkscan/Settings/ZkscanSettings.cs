// ------------------------------------------------------------------------------------------------------
// <copyright file="ZkscanSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions.Contracts;

namespace Nomis.Zkscan.Settings
{
    /// <summary>
    /// Zkscan settings.
    /// </summary>
    internal class ZkscanSettings :
        IBlockchainSettings
    {
        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://zksync2-mainnet.zkscan.io/api-docs"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }

        /// <inheritdoc />
        public BlockchainDescriptor? BlockchainDescriptor { get; set; }
    }
}