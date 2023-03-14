// ------------------------------------------------------------------------------------------------------
// <copyright file="Gnosisscan.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Nomis.Gnosisscan.Extensions;
using Nomis.Gnosisscan.Interfaces;

namespace Nomis.Gnosisscan
{
    /// <summary>
    /// Gnosisscan service registrar.
    /// </summary>
    public sealed class Gnosisscan :
        IGnosisServiceRegistrar
    {
        /// <inheritdoc/>
        public IServiceCollection RegisterService(
            IServiceCollection services)
        {
            return services
                .AddGnosisscanService();
        }
    }
}