// ------------------------------------------------------------------------------------------------------
// <copyright file="Zkscan.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Nomis.Zkscan.Extensions;
using Nomis.Zkscan.Interfaces;

namespace Nomis.Zkscan
{
    /// <summary>
    /// Zkscan service registrar.
    /// </summary>
    public sealed class Zkscan :
        IZkSyncServiceRegistrar
    {
        /// <inheritdoc/>
        public IServiceCollection RegisterService(
            IServiceCollection services)
        {
            return services
                .AddZkscanService();
        }
    }
}