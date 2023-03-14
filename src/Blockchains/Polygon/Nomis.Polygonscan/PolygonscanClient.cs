// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonscanClient.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Net.Http.Json;

using Microsoft.Extensions.Options;
using Nomis.Polygonscan.Interfaces;
using Nomis.Polygonscan.Interfaces.Models;
using Nomis.Polygonscan.Settings;
using Nomis.Utils.Exceptions;

namespace Nomis.Polygonscan
{
    /// <inheritdoc cref="IPolygonscanClient"/>
    internal sealed class PolygonscanClient :
        IPolygonscanClient
    {
        private const int ItemsFetchLimit = 10000;
        private readonly PolygonscanSettings _polygonscanSettings;

        private readonly HttpClient _client;

        /// <summary>
        /// Initialize <see cref="PolygonscanClient"/>.
        /// </summary>
        /// <param name="polygonscanSettings"><see cref="PolygonscanSettings"/>.</param>
        public PolygonscanClient(
            IOptions<PolygonscanSettings> polygonscanSettings)
        {
            _polygonscanSettings = polygonscanSettings.Value;
            _client = new()
            {
                BaseAddress = new(polygonscanSettings.Value.ApiBaseUrl ??
                                  throw new ArgumentNullException(nameof(polygonscanSettings.Value.ApiBaseUrl)))
            };
        }

        /// <inheritdoc/>
        public async Task<PolygonscanAccount> GetBalanceAsync(string address)
        {
            var response = await _client.GetAsync($"/api?module=account&action=balance&address={address}&apiKey={_polygonscanSettings.ApiKey}").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PolygonscanAccount>().ConfigureAwait(false) ?? throw new CustomException("Can't get account balance.");
        }

        /// <inheritdoc/>
        public async Task<PolygonscanAccount> GetTokenBalanceAsync(string address, string contractAddress)
        {
            var response = await _client.GetAsync($"/api?module=account&action=tokenbalance&address={address}&contractaddress={contractAddress}&tag=latest&apiKey={_polygonscanSettings.ApiKey}").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PolygonscanAccount>().ConfigureAwait(false) ?? throw new CustomException("Can't get account token balance.");
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TResultItem>> GetTransactionsAsync<TResult, TResultItem>(string address)
            where TResult : IPolygonscanTransferList<TResultItem>
            where TResultItem : IPolygonscanTransfer
        {
            var result = new List<TResultItem>();
            var transactionsData = await GetTransactionList<TResult>(address).ConfigureAwait(false);
            result.AddRange(transactionsData.Data ?? new List<TResultItem>());
            while (transactionsData?.Data?.Count >= ItemsFetchLimit)
            {
                transactionsData = await GetTransactionList<TResult>(address, transactionsData.Data.LastOrDefault()?.BlockNumber).ConfigureAwait(false);
                result.AddRange(transactionsData?.Data ?? new List<TResultItem>());
            }

            return result;
        }

        private async Task<TResult> GetTransactionList<TResult>(
            string address,
            string? startBlock = null)
        {
            string request =
                $"/api?module=account&address={address}&sort=asc&apiKey={_polygonscanSettings.ApiKey}";

            if (typeof(TResult) == typeof(PolygonscanAccountNormalTransactions))
            {
                request = $"{request}&action=txlist";
            }
            else if (typeof(TResult) == typeof(PolygonscanAccountInternalTransactions))
            {
                request = $"{request}&action=txlistinternal";
            }
            else if (typeof(TResult) == typeof(PolygonscanAccountERC20TokenEvents))
            {
                request = $"{request}&action=tokentx";
            }
            else if (typeof(TResult) == typeof(PolygonscanAccountERC721TokenEvents))
            {
                request = $"{request}&action=tokennfttx";
            }
            else if (typeof(TResult) == typeof(PolygonscanAccountERC1155TokenEvents))
            {
                request = $"{request}&action=token1155tx";
            }
            else
            {
                return default!;
            }

            if (!string.IsNullOrWhiteSpace(startBlock))
            {
                request = $"{request}&startblock={startBlock}";
            }

            var response = await _client.GetAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResult>().ConfigureAwait(false) ?? throw new CustomException("Can't get account transactions.");
        }
    }
}