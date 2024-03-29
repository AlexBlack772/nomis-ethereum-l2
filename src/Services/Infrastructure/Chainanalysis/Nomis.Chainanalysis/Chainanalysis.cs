﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="Chainanalysis.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Nomis.Chainanalysis.Extensions;
using Nomis.Chainanalysis.Interfaces;

namespace Nomis.Chainanalysis
{
    /// <summary>
    /// Chainanalysis service registrar.
    /// </summary>
    public sealed class Chainanalysis :
        IChainanalysisServiceRegistrar
    {
        /// <inheritdoc/>
        public IServiceCollection RegisterService(
            IServiceCollection services)
        {
            return services
                .AddChainanalysisService();
        }
    }
}