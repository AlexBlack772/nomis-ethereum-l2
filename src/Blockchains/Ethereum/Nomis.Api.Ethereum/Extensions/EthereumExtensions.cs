// ------------------------------------------------------------------------------------------------------
// <copyright file="EthereumExtensions.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Api.Common.Extensions;
using Nomis.Api.Ethereum.Settings;
using Nomis.Etherscan.Interfaces;
using Nomis.ScoringService.Interfaces.Builder;

namespace Nomis.Api.Ethereum.Extensions
{
    /// <summary>
    /// Ethereum extension methods.
    /// </summary>
    public static class EthereumExtensions
    {
        /// <summary>
        /// Add Ethereum blockchain.
        /// </summary>
        /// <typeparam name="TServiceRegistrar">The service registrar type.</typeparam>
        /// <param name="optionsBuilder"><see cref="IScoringOptionsBuilder"/>.</param>
        /// <returns>Returns <see cref="IScoringOptionsBuilder"/>.</returns>
        public static IScoringOptionsBuilder WithEthereumBlockchain<TServiceRegistrar>(
            this IScoringOptionsBuilder optionsBuilder)
            where TServiceRegistrar : IEthereumServiceRegistrar, new()
        {
            return optionsBuilder
                .With<EthereumAPISettings, TServiceRegistrar>();
        }
    }
}