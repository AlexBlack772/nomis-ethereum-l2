// ------------------------------------------------------------------------------------------------------
// <copyright file="OptimismController.cs" company="Nomis">
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
using Nomis.Optimism.Interfaces;
using Nomis.Optimism.Interfaces.Models;
using Nomis.Optimism.Interfaces.Requests;
using Nomis.Utils.Enums;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Optimism
{
    /// <summary>
    /// A controller to aggregate all Optimism-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Optimism blockchain.")]
    public sealed class OptimismController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/optimism";

        /// <summary>
        /// Common tag for Optimism actions.
        /// </summary>
        internal const string OptimismTag = "Optimism";

        private readonly ILogger<OptimismController> _logger;
        private readonly IOptimismScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="OptimismController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="IOptimismScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public OptimismController(
            IOptimismScoringService scoringService,
            ILogger<OptimismController> logger)
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
        ///     GET /api/v1/optimism/wallet/0xc9ee9eE4E1346b50ce08cB6ddB81481cc22E4b9b/score?scoreType=0&amp;nonce=0&amp;deadline=133160867380732039
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetOptimismWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetOptimismWalletScore",
            Tags = new[] { OptimismTag })]
        [ProducesResponseType(typeof(Result<OptimismWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RateLimitResult), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetOptimismWalletScoreAsync(
            [Required(ErrorMessage = "Request should be set")] OptimismWalletStatsRequest request,
            CancellationToken cancellationToken = default)
        {
            switch (request.ScoreType)
            {
                case ScoreType.Finance:
                    return Ok(await _scoringService.GetWalletStatsAsync<OptimismWalletStatsRequest, OptimismWalletScore, OptimismWalletStats, OptimismTransactionIntervalData>(request, cancellationToken));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}