// ------------------------------------------------------------------------------------------------------
// <copyright file="GnosisscanSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions.Contracts;

namespace Nomis.Gnosisscan.Settings
{
    /// <summary>
    /// Gnosisscan settings.
    /// </summary>
    internal class GnosisscanSettings :
        IBlockchainSettings
    {
        /// <summary>
        /// API key for gnosisscan.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://docs.gnosisscan.io/getting-started/endpoint-urls"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }

        /// <inheritdoc />
        public BlockchainDescriptor? BlockchainDescriptor { get; set; }
    }
}