// ------------------------------------------------------------------------------------------------------
// <copyright file="IGnosisscanTransferList.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nomis.Gnosisscan.Interfaces.Models
{
    /// <summary>
    /// Gnosisscan transfer list.
    /// </summary>
    /// <typeparam name="TListItem">Gnosisscan transfer.</typeparam>
    public interface IGnosisscanTransferList<TListItem>
        where TListItem : IGnosisscanTransfer
    {
        /// <summary>
        /// List of transfers.
        /// </summary>
        [JsonPropertyName("result")]
        [DataMember(EmitDefaultValue = true)]
        public IList<TListItem> Data { get; set; }
    }
}