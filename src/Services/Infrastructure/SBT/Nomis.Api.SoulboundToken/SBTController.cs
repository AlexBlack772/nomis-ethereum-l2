﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="SBTController.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nomis.Api.Common.Swagger.Examples;
using Nomis.SoulboundTokenService.Interfaces;
using Nomis.SoulboundTokenService.Interfaces.Models;
using Nomis.SoulboundTokenService.Interfaces.Requests;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

// ReSharper disable InconsistentNaming
namespace Nomis.Api.SoulboundToken
{
    /// <summary>
    /// A controller to aggregate all SBT-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Soulbound token API.")]
    public sealed class SBTController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/sbt";

        /// <summary>
        /// Common tag for SBT actions.
        /// </summary>
        internal const string SBTTag = "SBT";

        private readonly ILogger<SBTController> _logger;
        private readonly IEvmSoulboundTokenService _evmSoulboundTokenService;
        private readonly INonEvmSoulboundTokenService _nonEvmSoulboundTokenService;

        /// <summary>
        /// Initialize <see cref="SBTController"/>.
        /// </summary>
        /// <param name="evmSoulboundTokenService"><see cref="IEvmSoulboundTokenService"/>.</param>
        /// <param name="nonEvmSoulboundTokenService"><see cref="INonEvmSoulboundTokenService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public SBTController(
            IEvmSoulboundTokenService evmSoulboundTokenService,
            INonEvmSoulboundTokenService nonEvmSoulboundTokenService,
            ILogger<SBTController> logger)
        {
            _evmSoulboundTokenService = evmSoulboundTokenService;
            _nonEvmSoulboundTokenService = nonEvmSoulboundTokenService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get the EVM-compatible soulbound token signature.
        /// </summary>
        /// <param name="request">Soulbound token request.</param>
        /// <returns>The EVM-compatible soulbound token signature.</returns>
        /// <response code="200">Returns the EVM-compatible soulbound token signature.</response>
        /// <response code="400">Request is not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpPost("evm-token/signature", Name = "GetEvmSoulboundTokenSignature")]

        // [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetEvmSoulboundTokenSignature",
            Tags = new[] { SBTTag })]
        [ProducesResponseType(typeof(Result<SoulboundTokenSignature>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RateLimitResult), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult GetEvmSoulboundTokenSignature(
            [Required(ErrorMessage = "Request should be set"), FromBody] SoulboundTokenRequest request)
        {
            var result = _evmSoulboundTokenService.GetSoulboundTokenSignature(request);
            return Ok(result);
        }

        /// <summary>
        /// Get the non EVM-compatible soulbound token signature.
        /// </summary>
        /// <param name="request">Soulbound token request.</param>
        /// <returns>The non EVM-compatible soulbound token signature.</returns>
        /// <response code="200">Returns the non EVM-compatible soulbound token signature.</response>
        /// <response code="400">Request is not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpPost("non-evm-token/signature", Name = "GetNonEvmSoulboundTokenSignature")]

        // [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetNonEvmSoulboundTokenSignature",
            Tags = new[] { SBTTag })]
        [ProducesResponseType(typeof(Result<SoulboundTokenSignature>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RateLimitResult), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult GetNonEvmSoulboundTokenSignature(
            [Required(ErrorMessage = "Request should be set"), FromBody] SoulboundTokenRequest request)
        {
            var result = _nonEvmSoulboundTokenService.GetSoulboundTokenSignature(request);
            return Ok(result);
        }
    }
}