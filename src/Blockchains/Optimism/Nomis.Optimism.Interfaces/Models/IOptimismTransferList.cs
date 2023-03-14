// ------------------------------------------------------------------------------------------------------
// <copyright file="IOptimismTransferList.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nomis.Optimism.Interfaces.Models
{
    /// <summary>
    /// Optimism transfer list.
    /// </summary>
    /// <typeparam name="TListItem">Optimism transfer.</typeparam>
    public interface IOptimismTransferList<TListItem>
        where TListItem : IOptimismTransfer
    {
        /// <summary>
        /// List of transfers.
        /// </summary>
        [JsonPropertyName("result")]
        [DataMember(EmitDefaultValue = true)]
        public IList<TListItem> Data { get; set; }
    }
}