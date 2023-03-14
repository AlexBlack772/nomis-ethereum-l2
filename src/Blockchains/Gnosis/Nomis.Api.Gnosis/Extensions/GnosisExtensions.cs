// ------------------------------------------------------------------------------------------------------
// <copyright file="GnosisExtensions.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Api.Common.Extensions;
using Nomis.Api.Gnosis.Settings;
using Nomis.Gnosisscan.Interfaces;
using Nomis.ScoringService.Interfaces.Builder;

namespace Nomis.Api.Gnosis.Extensions
{
    /// <summary>
    /// Gnosis extension methods.
    /// </summary>
    public static class GnosisExtensions
    {
        /// <summary>
        /// Add Gnosis blockchain.
        /// </summary>
        /// <typeparam name="TServiceRegistrar">The service registrar type.</typeparam>
        /// <param name="optionsBuilder"><see cref="IScoringOptionsBuilder"/>.</param>
        /// <returns>Returns <see cref="IScoringOptionsBuilder"/>.</returns>
        public static IScoringOptionsBuilder WithGnosisBlockchain<TServiceRegistrar>(
            this IScoringOptionsBuilder optionsBuilder)
            where TServiceRegistrar : IGnosisServiceRegistrar, new()
        {
            return optionsBuilder
                .With<GnosisAPISettings, TServiceRegistrar>();
        }
    }
}