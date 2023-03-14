// ------------------------------------------------------------------------------------------------------
// <copyright file="ZkscanAccount.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Nomis.Zkscan.Interfaces.Models
{
    /// <summary>
    /// Zkscan account.
    /// </summary>
    public class ZkscanAccount
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
        /// Balance.
        /// </summary>
        [JsonPropertyName("result")]
        public string? Balance { get; set; }
    }
}