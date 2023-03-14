// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonscanAccountERC721TokenEvents.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nomis.Polygonscan.Interfaces.Models
{
    /// <summary>
    /// Polygonscan account ERC-721 token transfer events.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class PolygonscanAccountERC721TokenEvents :
        IPolygonscanTransferList<PolygonscanAccountERC721TokenEvent>
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
        /// Account ERC-721 token event list.
        /// </summary>
        [JsonPropertyName("result")]
        [DataMember(EmitDefaultValue = true)]
        public IList<PolygonscanAccountERC721TokenEvent>? Data { get; set; } = new List<PolygonscanAccountERC721TokenEvent>();
    }
}