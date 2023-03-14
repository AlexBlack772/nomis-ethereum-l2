// ------------------------------------------------------------------------------------------------------
// <copyright file="OptimismSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions.Contracts;

namespace Nomis.Optimism.Settings
{
    /// <summary>
    /// Optimism settings.
    /// </summary>
    internal class OptimismSettings :
        IBlockchainSettings
    {
        /// <summary>
        /// API key for optimism.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://docs.optimism.etherscan.io/getting-started/endpoint-urls"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }

        /// <inheritdoc />
        public BlockchainDescriptor? BlockchainDescriptor { get; set; }
    }
}