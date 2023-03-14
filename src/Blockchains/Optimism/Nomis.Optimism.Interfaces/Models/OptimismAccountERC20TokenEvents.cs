// ------------------------------------------------------------------------------------------------------
// <copyright file="OptimismAccountERC20TokenEvents.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nomis.Optimism.Interfaces.Models
{
    /// <summary>
    /// Optimism account ERC-20 token transfer events.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class OptimismAccountERC20TokenEvents :
        IOptimismTransferList<OptimismAccountERC20TokenEvent>
    {
        /// <summary>
        /// Status.
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { get; set; }

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
        public IList<OptimismAccountERC20TokenEvent> Data { get; set; } = new List<OptimismAccountERC20TokenEvent>();
    }
}