// ------------------------------------------------------------------------------------------------------
// <copyright file="Polygonscan.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Nomis.Polygonscan.Extensions;
using Nomis.Polygonscan.Interfaces;

namespace Nomis.Polygonscan
{
    /// <summary>
    /// Polygonscan service registrar.
    /// </summary>
    public sealed class Polygonscan :
        IPolygonServiceRegistrar
    {
        /// <inheritdoc/>
        public IServiceCollection RegisterService(
            IServiceCollection services)
        {
            return services
                .AddPolygonscanService();
        }
    }
}