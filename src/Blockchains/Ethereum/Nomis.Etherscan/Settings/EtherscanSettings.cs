// ------------------------------------------------------------------------------------------------------
// <copyright file="EtherscanSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions.Contracts;

namespace Nomis.Etherscan.Settings
{
    /// <summary>
    /// Etherscan settings.
    /// </summary>
    internal class EtherscanSettings :
        IBlockchainSettings
    {
        /// <summary>
        /// API key for etherscan.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Blockchain provider URL.
        /// </summary>
        public string? BlockchainProviderUrl { get; set; }

        /// <inheritdoc cref="BlockchainDescriptor"/>
        public BlockchainDescriptor? BlockchainDescriptor { get; set; }
    }
}