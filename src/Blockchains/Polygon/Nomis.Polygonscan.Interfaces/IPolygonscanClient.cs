// ------------------------------------------------------------------------------------------------------
// <copyright file="IPolygonscanClient.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Polygonscan.Interfaces.Models;

namespace Nomis.Polygonscan.Interfaces
{
    /// <summary>
    /// Polygonscan client.
    /// </summary>
    public interface IPolygonscanClient
    {
        /// <summary>
        /// Get the account balance in Wei.
        /// </summary>
        /// <param name="address">Account address.</param>
        /// <returns>Returns <see cref="PolygonscanAccount"/>.</returns>
        Task<PolygonscanAccount> GetBalanceAsync(string address);

        /// <summary>
        /// Get the account token balance in Wei.
        /// </summary>
        /// <param name="address">Account address.</param>
        /// <param name="contractAddress">Token contract address.</param>
        /// <returns>Returns <see cref="PolygonscanAccount"/>.</returns>
        Task<PolygonscanAccount> GetTokenBalanceAsync(string address, string contractAddress);

        /// <summary>
        /// Get list of specific transactions/transfers of the given account.
        /// </summary>
        /// <typeparam name="TResult">The type of returned response.</typeparam>
        /// <typeparam name="TResultItem">The type of returned response data items.</typeparam>
        /// <param name="address">Account address.</param>
        /// <returns>Returns list of specific transactions/transfers of the given account.</returns>
        Task<IEnumerable<TResultItem>> GetTransactionsAsync<TResult, TResultItem>(string address)
            where TResult : IPolygonscanTransferList<TResultItem>
            where TResultItem : IPolygonscanTransfer;
    }
}