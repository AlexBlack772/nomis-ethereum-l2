// ------------------------------------------------------------------------------------------------------
// <copyright file="OptimismClient.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Net.Http.Json;

using Microsoft.Extensions.Options;
using Nomis.Optimism.Interfaces;
using Nomis.Optimism.Interfaces.Models;
using Nomis.Optimism.Settings;
using Nomis.Utils.Exceptions;

namespace Nomis.Optimism
{
    /// <inheritdoc cref="IOptimismClient"/>
    internal sealed class OptimismClient :
        IOptimismClient
    {
        private const int ItemsFetchLimit = 10000;
        private readonly OptimismSettings _optimismSettings;

        private readonly HttpClient _client;

        /// <summary>
        /// Initialize <see cref="OptimismClient"/>.
        /// </summary>
        /// <param name="optimismSettings"><see cref="OptimismSettings"/>.</param>
        public OptimismClient(
            IOptions<OptimismSettings> optimismSettings)
        {
            _optimismSettings = optimismSettings.Value;
            _client = new()
            {
                BaseAddress = new(optimismSettings.Value.ApiBaseUrl ??
                                  throw new ArgumentNullException(nameof(optimismSettings.Value.ApiBaseUrl)))
            };
        }

        /// <inheritdoc/>
        public async Task<OptimismAccount> GetBalanceAsync(string address)
        {
            string request =
                $"/api?module=account&action=balance&address={address}&tag=latest";
            if (!string.IsNullOrWhiteSpace(_optimismSettings.ApiKey))
            {
                request += $"&apiKey={_optimismSettings.ApiKey}";
            }

            var response = await _client.GetAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OptimismAccount>().ConfigureAwait(false) ?? throw new CustomException("Can't get account balance.");
        }

        /// <inheritdoc/>
        public async Task<OptimismAccount> GetTokenBalanceAsync(string address, string contractAddress)
        {
            string request =
                $"/api?module=account&action=tokenbalance&address={address}&contractaddress={contractAddress}&tag=latest";
            if (!string.IsNullOrWhiteSpace(_optimismSettings.ApiKey))
            {
                request += $"&apiKey={_optimismSettings.ApiKey}";
            }

            var response = await _client.GetAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OptimismAccount>().ConfigureAwait(false) ?? throw new CustomException("Can't get account token balance.");
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TResultItem>> GetTransactionsAsync<TResult, TResultItem>(string address)
            where TResult : IOptimismTransferList<TResultItem>
            where TResultItem : IOptimismTransfer
        {
            var result = new List<TResultItem>();
            var transactionsData = await GetTransactionListAsync<TResult>(address).ConfigureAwait(false);
            result.AddRange(transactionsData.Data ?? new List<TResultItem>());
            while (transactionsData?.Data?.Count >= ItemsFetchLimit)
            {
                transactionsData = await GetTransactionListAsync<TResult>(address, transactionsData.Data.LastOrDefault()?.BlockNumber).ConfigureAwait(false);
                result.AddRange(transactionsData?.Data ?? new List<TResultItem>());
            }

            return result;
        }

        private async Task<TResult> GetTransactionListAsync<TResult>(
            string address,
            string? startBlock = null)
        {
            string request =
                $"/api?module=account&address={address}&sort=asc";

            if (typeof(TResult) == typeof(OptimismAccountNormalTransactions))
            {
                request = $"{request}&action=txlist";
            }
            else if (typeof(TResult) == typeof(OptimismAccountInternalTransactions))
            {
                request = $"{request}&action=txlistinternal";
            }
            else if (typeof(TResult) == typeof(OptimismAccountERC20TokenEvents))
            {
                request = $"{request}&action=tokentx";
            }
            else if (typeof(TResult) == typeof(OptimismAccountERC721TokenEvents))
            {
                request = $"{request}&action=tokennfttx";
            }
            else
            {
                return default!;
            }

            if (!string.IsNullOrWhiteSpace(startBlock))
            {
                request = $"{request}&startblock={startBlock}";
            }
            else
            {
                request = $"{request}&startblock=0";
            }

            request = $"{request}&endblock=999999999";

            if (!string.IsNullOrWhiteSpace(_optimismSettings.ApiKey))
            {
                request += $"&apiKey={_optimismSettings.ApiKey}";
            }

            var response = await _client.GetAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResult>().ConfigureAwait(false) ?? throw new CustomException("Can't get account transactions.");
        }
    }
}