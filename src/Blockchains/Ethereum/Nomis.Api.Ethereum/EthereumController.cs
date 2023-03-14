// ------------------------------------------------------------------------------------------------------
// <copyright file="EthereumController.cs" company="Nomis">
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
using Nomis.Etherscan.Interfaces;
using Nomis.Etherscan.Interfaces.Models;
using Nomis.Etherscan.Interfaces.Requests;
using Nomis.Utils.Enums;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Ethereum
{
    /// <summary>
    /// A controller to aggregate all Ethereum-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Ethereum blockchain.")]
    public sealed class EthereumController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/ethereum";

        /// <summary>
        /// Common tag for Ethereum actions.
        /// </summary>
        internal const string EthereumTag = "Ethereum";

        private readonly ILogger<EthereumController> _logger;
        private readonly IEthereumScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="EthereumController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="IEthereumScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public EthereumController(
            IEthereumScoringService scoringService,
            ILogger<EthereumController> logger)
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
        ///     GET /api/v1/ethereum/wallet/0xF696AB3E4F9d52482B8350fFae67D21fda78e601/score?scoreType=0&amp;nonce=0&amp;deadline=133160867380732039
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetEthereumWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetEthereumWalletScore",
            Tags = new[] { EthereumTag })]
        [ProducesResponseType(typeof(Result<EthereumWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RateLimitResult), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetEthereumWalletScoreAsync(
            [Required(ErrorMessage = "Request should be set")] EthereumWalletStatsRequest request,
            CancellationToken cancellationToken = default)
        {
            switch (request.ScoreType)
            {
                case ScoreType.Finance:
                    return Ok(await _scoringService.GetWalletStatsAsync<EthereumWalletStatsRequest, EthereumWalletScore, EthereumWalletStats, EthereumTransactionIntervalData>(request, cancellationToken));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}