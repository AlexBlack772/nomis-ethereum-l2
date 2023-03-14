// ------------------------------------------------------------------------------------------------------
// <copyright file="ZkSyncController.cs" company="Nomis">
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
using Nomis.Utils.Enums;
using Nomis.Utils.Wrapper;
using Nomis.Zkscan.Interfaces;
using Nomis.Zkscan.Interfaces.Models;
using Nomis.Zkscan.Interfaces.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.ZkSync
{
    /// <summary>
    /// A controller to aggregate all ZkSync-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("ZkSync Era blockchain.")]
    public sealed class ZkSyncController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/zk-sync-era";

        /// <summary>
        /// Common tag for ZkSync actions.
        /// </summary>
        internal const string ZkSyncTag = "ZkSync";

        private readonly ILogger<ZkSyncController> _logger;
        private readonly IZkSyncScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="ZkSyncController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="IZkSyncScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public ZkSyncController(
            IZkSyncScoringService scoringService,
            ILogger<ZkSyncController> logger)
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
        ///     GET /api/v1/zk-sync-era/wallet/0x98E9D288743839e96A8005a6B51C770Bbf7788C0/score?scoreType=0&amp;nonce=0&amp;deadline=133160867380732039
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetZkSyncWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetZkSyncWalletScore",
            Tags = new[] { ZkSyncTag })]
        [ProducesResponseType(typeof(Result<ZkSyncWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RateLimitResult), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetZkSyncWalletScoreAsync(
            [Required(ErrorMessage = "Request should be set")] ZkSyncWalletStatsRequest request,
            CancellationToken cancellationToken = default)
        {
            switch (request.ScoreType)
            {
                case ScoreType.Finance:
                    return Ok(await _scoringService.GetWalletStatsAsync<ZkSyncWalletStatsRequest, ZkSyncWalletScore, ZkSyncWalletStats, ZkSyncTransactionIntervalData>(request, cancellationToken));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}