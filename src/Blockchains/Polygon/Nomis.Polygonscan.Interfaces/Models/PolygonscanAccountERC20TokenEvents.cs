// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonscanAccountERC20TokenEvents.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nomis.Polygonscan.Interfaces.Models
{
    /// <summary>
    /// Polygonscan account ERC-20 token transfer events.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class PolygonscanAccountERC20TokenEvents :
        IPolygonscanTransferList<PolygonscanAccountERC20TokenEvent>
    {
        /// <summary>
        /// Status.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Message.
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Account ERC-20 token event list.
        /// </summary>
        [JsonPropertyName("result")]
        [DataMember(EmitDefaultValue = true)]
        public IList<PolygonscanAccountERC20TokenEvent>? Data { get; set; } = new List<PolygonscanAccountERC20TokenEvent>();
    }
}