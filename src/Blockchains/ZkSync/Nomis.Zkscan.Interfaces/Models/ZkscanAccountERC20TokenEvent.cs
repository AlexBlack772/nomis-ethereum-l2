﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="ZkscanAccountERC20TokenEvent.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Nomis.Zkscan.Interfaces.Models
{
    /// <summary>
    /// Zkscan account ERC-20 token transfer event.
    /// </summary>
    public class ZkscanAccountERC20TokenEvent :
        IZkscanTransfer
    {
        /// <summary>
        /// Block number.
        /// </summary>
        [JsonPropertyName("blockNumber")]
        public string? BlockNumber { get; set; }

        /// <summary>
        /// Time stamp.
        /// </summary>
        [JsonPropertyName("timeStamp")]
        public string? TimeStamp { get; set; }

        /// <summary>
        /// Hash.
        /// </summary>
        [JsonPropertyName("hash")]
        public string? Hash { get; set; }

        /// <summary>
        /// From address.
        /// </summary>
        [JsonPropertyName("from")]
        public string? From { get; set; }

        /// <summary>
        /// Contract address.
        /// </summary>
        [JsonPropertyName("contractAddress")]
        public string? ContractAddress { get; set; }

        /// <summary>
        /// To address.
        /// </summary>
        [JsonPropertyName("to")]
        public string? To { get; set; }

        /// <summary>
        /// Value.
        /// </summary>
        [JsonPropertyName("value")]
        public string? Value { get; set; }

        /// <summary>
        /// Token name.
        /// </summary>
        [JsonPropertyName("tokenName")]
        public string? TokenName { get; set; }

        /// <summary>
        /// Token symbol.
        /// </summary>
        [JsonPropertyName("tokenSymbol")]
        public string? TokenSymbol { get; set; }

        /// <summary>
        /// Token decimal.
        /// </summary>
        [JsonPropertyName("tokenDecimal")]
        public string? TokenDecimal { get; set; }

        /// <summary>
        /// Confirmations.
        /// </summary>
        [JsonPropertyName("confirmations")]
        public string? Confirmations { get; set; }
    }
}