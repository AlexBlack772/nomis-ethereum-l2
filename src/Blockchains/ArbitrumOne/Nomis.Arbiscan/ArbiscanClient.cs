// ------------------------------------------------------------------------------------------------------
// <copyright file="ArbiscanClient.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Net.Http.Json;

using Microsoft.Extensions.Options;
using Nomis.Arbiscan.Interfaces;
using Nomis.Arbiscan.Interfaces.Models;
using Nomis.Arbiscan.Settings;
using Nomis.Utils.Exceptions;

namespace Nomis.Arbiscan
{
    /// <inheritdoc cref="IArbiscanClient"/>
    internal sealed class ArbiscanClient :
        IArbiscanClient
    {
        private const int ItemsFetchLimit = 10000;
        private readonly ArbiscanSettings _arbiscanSettings;

        private readonly HttpClient _client;

        /// <summary>
        /// Initialize <see cref="ArbiscanClient"/>.
        /// </summary>
        /// <param name="arbiscanSettings"><see cref="ArbiscanSettings"/>.</param>
        public ArbiscanClient(
            IOptions<ArbiscanSettings> arbiscanSettings)
        {
            _arbiscanSettings = arbiscanSettings.Value;
            _client = new()
            {
                BaseAddress = new(arbiscanSettings.Value.ApiBaseUrl ??
                                  throw new ArgumentNullException(nameof(arbiscanSettings.Value.ApiBaseUrl)))
            };
        }

        /// <inheritdoc/>
        public async Task<ArbiscanAccount> GetBalanceAsync(string address)
        {
            string request =
                $"/api?module=account&action=balance&address={address}&tag=latest";
            if (!string.IsNullOrWhiteSpace(_arbiscanSettings.ApiKey))
            {
                request += $"&apiKey={_arbiscanSettings.ApiKey}";
            }

            var response = await _client.GetAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ArbiscanAccount>().ConfigureAwait(false) ?? throw new CustomException("Can't get account balance.");
        }

        /// <inheritdoc/>
        public async Task<ArbiscanAccount> GetTokenBalanceAsync(string address, string contractAddress)
        {
            string request =
                $"/api?module=account&action=tokenbalance&address={address}&contractaddress={contractAddress}&tag=latest";
            if (!string.IsNullOrWhiteSpace(_arbiscanSettings.ApiKey))
            {
                request += $"&apiKey={_arbiscanSettings.ApiKey}";
            }

            var response = await _client.GetAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ArbiscanAccount>().ConfigureAwait(false) ?? throw new CustomException("Can't get account token balance.");
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TResultItem>> GetTransactionsAsync<TResult, TResultItem>(string address)
            where TResult : IArbiscanTransferList<TResultItem>
            where TResultItem : IArbiscanTransfer
        {
            var result = new List<TResultItem>();
            var transactionsData = await GetTransactionListAsync<TResult>(address).ConfigureAwait(false);
            result.AddRange(transactionsData.Data ?? new List<TResultItem>());
            while (transactionsData?.Data?.Count >= ItemsFetchLimit)
            {
                await Task.Delay(100).ConfigureAwait(false);
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

            if (typeof(TResult) == typeof(ArbiscanAccountNormalTransactions))
            {
                request = $"{request}&action=txlist";
            }
            else if (typeof(TResult) == typeof(ArbiscanAccountInternalTransactions))
            {
                request = $"{request}&action=txlistinternal";
            }
            else if (typeof(TResult) == typeof(ArbiscanAccountERC20TokenEvents))
            {
                request = $"{request}&action=tokentx";
            }
            else if (typeof(TResult) == typeof(ArbiscanAccountERC721TokenEvents))
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

            if (!string.IsNullOrWhiteSpace(_arbiscanSettings.ApiKey))
            {
                request += $"&apiKey={_arbiscanSettings.ApiKey}";
            }

            var response = await _client.GetAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResult>().ConfigureAwait(false) ?? throw new CustomException("Can't get account transactions.");
        }
    }
}