// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonController.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nomis.Api.Common.Swagger.Examples;
using Nomis.Polygonscan.Interfaces;
using Nomis.Polygonscan.Interfaces.Models;
using Nomis.Polygonscan.Interfaces.Requests;
using Nomis.Utils.Contracts.Requests;
using Nomis.Utils.Enums;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Polygon
{
    /// <summary>
    /// A controller to aggregate all Polygon-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Polygon blockchain.")]
    public sealed class PolygonController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/polygon";

        /// <summary>
        /// Common tag for Polygon actions.
        /// </summary>
        internal const string PolygonTag = "Polygon";

        private readonly ILogger<PolygonController> _logger;
        private readonly IPolygonScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="PolygonController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="IPolygonScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public PolygonController(
            IPolygonScoringService scoringService,
            ILogger<PolygonController> logger)
        {
            _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get Nomis Score for given wallet address.
        /// </summary>
        /// <param name="request">Request for getting the wallet stats.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns>An Nomis Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/polygon/wallet/0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae/score?scoreType=1&amp;tokenAddress=0x4e78011Ce80ee02d2c3e649Fb657E45898257815&amp;nonce=0&amp;deadline=133160867380732039
        ///
        /// <para>Eco tokens:</para>
        /// <para>0x4e78011Ce80ee02d2c3e649Fb657E45898257815 - KLIMA</para>
        /// <para>0x6AcdA5E7EB1117733DC7Cb6158fc67f226b32022 - ZRO</para>
        /// <para>0x2f800db0fdb5223b3c3f354886d907a671414a7f - BCT</para>
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetPolygonWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetPolygonWalletScore",
            Tags = new[] { PolygonTag })]
        [ProducesResponseType(typeof(Result<PolygonWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RateLimitResult), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetPolygonWalletScoreAsync(
            [Required(ErrorMessage = "Request should be set")] PolygonWalletStatsRequest request,
            CancellationToken cancellationToken = default)
        {
            switch (request.ScoreType)
            {
                case ScoreType.Finance:
                    return Ok(await _scoringService.GetWalletStatsAsync<PolygonWalletStatsRequest, PolygonWalletScore, PolygonWalletStats, PolygonTransactionIntervalData>(request, cancellationToken));
                case ScoreType.Token:
                    return Ok(await _scoringService.GetWalletStatsAsync<WalletStatsRequest, PolygonWalletTokenScore, PolygonWalletTokenStats, PolygonTransactionIntervalData>(request, cancellationToken));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}