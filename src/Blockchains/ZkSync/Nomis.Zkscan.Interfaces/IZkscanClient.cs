// ------------------------------------------------------------------------------------------------------
// <copyright file="IZkscanClient.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Zkscan.Interfaces.Models;

namespace Nomis.Zkscan.Interfaces
{
    /// <summary>
    /// Zkscan client.
    /// </summary>
    public interface IZkscanClient
    {
        /// <summary>
        /// Get the account balance in Wei.
        /// </summary>
        /// <param name="address">Account address.</param>
        /// <returns>Returns <see cref="ZkscanAccount"/>.</returns>
        Task<ZkscanAccount> GetBalanceAsync(string address);

        /// <summary>
        /// Get the account token balance in Wei.
        /// </summary>
        /// <param name="address">Account address.</param>
        /// <param name="contractAddress">Token contract address.</param>
        /// <returns>Returns <see cref="ZkscanAccount"/>.</returns>
        Task<ZkscanAccount> GetTokenBalanceAsync(string address, string contractAddress);

        /// <summary>
        /// Get list of specific transactions/transfers of the given account.
        /// </summary>
        /// <typeparam name="TResult">The type of returned response.</typeparam>
        /// <typeparam name="TResultItem">The type of returned response data items.</typeparam>
        /// <param name="address">Account address.</param>
        /// <returns>Returns list of specific transactions/transfers of the given account.</returns>
        Task<IEnumerable<TResultItem>> GetTransactionsAsync<TResult, TResultItem>(string address)
            where TResult : IZkscanTransferList<TResultItem>
            where TResultItem : IZkscanTransfer;
    }
}