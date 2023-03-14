// ------------------------------------------------------------------------------------------------------
// <copyright file="Arbiscan.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Nomis.Arbiscan.Extensions;
using Nomis.Arbiscan.Interfaces;

namespace Nomis.Arbiscan
{
    /// <summary>
    /// Arbiscan service registrar.
    /// </summary>
    public sealed class Arbiscan :
        IArbitrumOneServiceRegistrar
    {
        /// <inheritdoc/>
        public IServiceCollection RegisterService(
            IServiceCollection services)
        {
            return services
                .AddArbiscanService();
        }
    }
}