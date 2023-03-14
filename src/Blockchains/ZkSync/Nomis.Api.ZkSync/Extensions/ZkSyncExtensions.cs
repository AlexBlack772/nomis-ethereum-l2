// ------------------------------------------------------------------------------------------------------
// <copyright file="ZkSyncExtensions.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Api.Common.Extensions;
using Nomis.Api.ZkSync.Settings;
using Nomis.ScoringService.Interfaces.Builder;
using Nomis.Zkscan.Interfaces;

namespace Nomis.Api.ZkSync.Extensions
{
    /// <summary>
    /// ZkSync extension methods.
    /// </summary>
    public static class ZkSyncExtensions
    {
        /// <summary>
        /// Add ZkSync blockchain.
        /// </summary>
        /// <typeparam name="TServiceRegistrar">The service registrar type.</typeparam>
        /// <param name="optionsBuilder"><see cref="IScoringOptionsBuilder"/>.</param>
        /// <returns>Returns <see cref="IScoringOptionsBuilder"/>.</returns>
        public static IScoringOptionsBuilder WithZkSyncBlockchain<TServiceRegistrar>(
            this IScoringOptionsBuilder optionsBuilder)
            where TServiceRegistrar : IZkSyncServiceRegistrar, new()
        {
            return optionsBuilder
                .With<ZkSyncAPISettings, TServiceRegistrar>();
        }
    }
}