// ------------------------------------------------------------------------------------------------------
// <copyright file="IPolygonscanTransferList.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nomis.Polygonscan.Interfaces.Models
{
    /// <summary>
    /// Polygonscan transfer list.
    /// </summary>
    /// <typeparam name="TListItem">Polygonscan transfer.</typeparam>
    public interface IPolygonscanTransferList<TListItem>
        where TListItem : IPolygonscanTransfer
    {
        /// <summary>
        /// List of transfers.
        /// </summary>
        [JsonPropertyName("result")]
        [DataMember(EmitDefaultValue = true)]
        public IList<TListItem>? Data { get; set; }
    }
}