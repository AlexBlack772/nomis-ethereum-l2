// ------------------------------------------------------------------------------------------------------
// <copyright file="GnosisController.cs" company="Nomis">
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
using Nomis.Gnosisscan.Interfaces;
using Nomis.Gnosisscan.Interfaces.Models;
using Nomis.Gnosisscan.Interfaces.Requests;
using Nomis.Utils.Enums;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Gnosis
{
    /// <summary>
    /// A controller to aggregate all Gnosis-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Gnosis blockchain.")]
    public sealed class GnosisController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/gnosis";

        /// <summary>
        /// Common tag for Gnosis actions.
        /// </summary>
        internal const string GnosisTag = "Gnosis";

        private readonly ILogger<GnosisController> _logger;
        private readonly IGnosisScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="GnosisController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="IGnosisScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public GnosisController(
            IGnosisScoringService scoringService,
            ILogger<GnosisController> logger)
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
        ///     GET /api/v1/gnosis/wallet/0x7a9e4543f98aA366faC8Bb4Ca679d63fF5dB9faB/score?scoreType=0&amp;nonce=0&amp;deadline=133160867380732039
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetGnosisWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetGnosisWalletScore",
            Tags = new[] { GnosisTag })]
        [ProducesResponseType(typeof(Result<GnosisWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RateLimitResult), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetGnosisWalletScoreAsync(
            [Required(ErrorMessage = "Request should be set")] GnosisWalletStatsRequest request,
            CancellationToken cancellationToken = default)
        {
            switch (request.ScoreType)
            {
                case ScoreType.Finance:
                    return Ok(await _scoringService.GetWalletStatsAsync<GnosisWalletStatsRequest, GnosisWalletScore, GnosisWalletStats, GnosisTransactionIntervalData>(request, cancellationToken));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}