// ------------------------------------------------------------------------------------------------------
// <copyright file="ArbitrumOneExtensions.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Api.ArbitrumOne.Settings;
using Nomis.Api.Common.Extensions;
using Nomis.Arbiscan.Interfaces;
using Nomis.ScoringService.Interfaces.Builder;

namespace Nomis.Api.ArbitrumOne.Extensions
{
    /// <summary>
    /// Arbitrum One extension methods.
    /// </summary>
    public static class ArbitrumOneExtensions
    {
        /// <summary>
        /// Add Arbitrum One blockchain.
        /// </summary>
        /// <typeparam name="TServiceRegistrar">The service registrar type.</typeparam>
        /// <param name="optionsBuilder"><see cref="IScoringOptionsBuilder"/>.</param>
        /// <returns>Returns <see cref="IScoringOptionsBuilder"/>.</returns>
        public static IScoringOptionsBuilder WithArbitrumOneBlockchain<TServiceRegistrar>(
            this IScoringOptionsBuilder optionsBuilder)
            where TServiceRegistrar : IArbitrumOneServiceRegistrar, new()
        {
            return optionsBuilder
                .With<ArbitrumOneAPISettings, TServiceRegistrar>();
        }
    }
}