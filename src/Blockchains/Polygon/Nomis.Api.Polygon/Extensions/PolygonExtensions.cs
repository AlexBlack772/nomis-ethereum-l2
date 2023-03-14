// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonExtensions.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Api.Common.Extensions;
using Nomis.Api.Polygon.Settings;
using Nomis.Polygonscan.Interfaces;
using Nomis.ScoringService.Interfaces.Builder;

namespace Nomis.Api.Polygon.Extensions
{
    /// <summary>
    /// Polygon extension methods.
    /// </summary>
    public static class PolygonExtensions
    {
        /// <summary>
        /// Add Polygon blockchain.
        /// </summary>
        /// <typeparam name="TServiceRegistrar">The service registrar type.</typeparam>
        /// <param name="optionsBuilder"><see cref="IScoringOptionsBuilder"/>.</param>
        /// <returns>Returns <see cref="IScoringOptionsBuilder"/>.</returns>
        public static IScoringOptionsBuilder WithPolygonBlockchain<TServiceRegistrar>(
            this IScoringOptionsBuilder optionsBuilder)
            where TServiceRegistrar : IPolygonServiceRegistrar, new()
        {
            return optionsBuilder
                .With<PolygonAPISettings, TServiceRegistrar>();
        }
    }
}