// ------------------------------------------------------------------------------------------------------
// <copyright file="Optimism.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Nomis.Optimism.Extensions;
using Nomis.Optimism.Interfaces;

namespace Nomis.Optimism
{
    /// <summary>
    /// Optimism service registrar.
    /// </summary>
    public sealed class Optimism :
        IOptimismServiceRegistrar
    {
        /// <inheritdoc/>
        public IServiceCollection RegisterService(
            IServiceCollection services)
        {
            return services
                .AddOptimismService();
        }
    }
}