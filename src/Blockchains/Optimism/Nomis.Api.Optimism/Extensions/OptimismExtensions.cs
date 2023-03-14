// ------------------------------------------------------------------------------------------------------
// <copyright file="OptimismExtensions.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Api.Common.Extensions;
using Nomis.Api.Optimism.Settings;
using Nomis.Optimism.Interfaces;
using Nomis.ScoringService.Interfaces.Builder;

namespace Nomis.Api.Optimism.Extensions
{
    /// <summary>
    /// Optimism extension methods.
    /// </summary>
    public static class OptimismExtensions
    {
        /// <summary>
        /// Add Optimism blockchain.
        /// </summary>
        /// <typeparam name="TServiceRegistrar">The service registrar type.</typeparam>
        /// <param name="optionsBuilder"><see cref="IScoringOptionsBuilder"/>.</param>
        /// <returns>Returns <see cref="IScoringOptionsBuilder"/>.</returns>
        public static IScoringOptionsBuilder WithOptimismBlockchain<TServiceRegistrar>(
            this IScoringOptionsBuilder optionsBuilder)
            where TServiceRegistrar : IOptimismServiceRegistrar, new()
        {
            return optionsBuilder
                .With<OptimismAPISettings, TServiceRegistrar>();
        }
    }
}