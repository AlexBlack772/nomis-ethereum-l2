// ------------------------------------------------------------------------------------------------------
// <copyright file="IZkscanTransferList.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nomis.Zkscan.Interfaces.Models
{
    /// <summary>
    /// Zkscan transfer list.
    /// </summary>
    /// <typeparam name="TListItem">Zkscan transfer.</typeparam>
    public interface IZkscanTransferList<TListItem>
        where TListItem : IZkscanTransfer
    {
        /// <summary>
        /// List of transfers.
        /// </summary>
        [JsonPropertyName("result")]
        [DataMember(EmitDefaultValue = true)]
        public IList<TListItem> Data { get; set; }
    }
}