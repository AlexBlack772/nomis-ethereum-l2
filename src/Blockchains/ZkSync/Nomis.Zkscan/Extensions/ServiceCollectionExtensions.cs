// ------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nomis.Chainanalysis.Interfaces;
using Nomis.Coingecko.Interfaces;
using Nomis.DefiLlama.Interfaces;
using Nomis.Greysafe.Interfaces;
using Nomis.ScoringService.Interfaces;
using Nomis.SoulboundTokenService.Interfaces;
using Nomis.Utils.Extensions;
using Nomis.Zkscan.Interfaces;
using Nomis.Zkscan.Settings;

namespace Nomis.Zkscan.Extensions
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Zkscan service.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        /// <returns>Returns <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddZkscanService(
            this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            serviceProvider.GetRequiredService<ICoingeckoService>();
            serviceProvider.GetRequiredService<IScoringService>();
            serviceProvider.GetRequiredService<IEvmSoulboundTokenService>();
            serviceProvider.GetRequiredService<IDefiLlamaService>();
            serviceProvider.GetRequiredService<IChainanalysisService>();
            serviceProvider.GetRequiredService<IGreysafeService>();
            services.AddSettings<ZkscanSettings>(configuration);
            return services
                .AddTransient<IZkscanClient, ZkscanClient>()
                .AddTransientInfrastructureService<IZkSyncScoringService, ZkscanService>();
        }
    }
}