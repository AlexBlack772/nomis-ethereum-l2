// ------------------------------------------------------------------------------------------------------
// <copyright file="GnosisscanClient.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Net.Http.Json;

using Microsoft.Extensions.Options;
using Nomis.Gnosisscan.Interfaces;
using Nomis.Gnosisscan.Interfaces.Models;
using Nomis.Gnosisscan.Settings;
using Nomis.Utils.Exceptions;

namespace Nomis.Gnosisscan
{
    /// <inheritdoc cref="IGnosisscanClient"/>
    internal sealed class GnosisscanClient :
        IGnosisscanClient
    {
        private const int ItemsFetchLimit = 10000;
        private readonly GnosisscanSettings _gnosisscanSettings;

        private readonly HttpClient _client;

        /// <summary>
        /// Initialize <see cref="GnosisscanClient"/>.
        /// </summary>
        /// <param name="gnosisscanSettings"><see cref="GnosisscanSettings"/>.</param>
        public GnosisscanClient(
            IOptions<GnosisscanSettings> gnosisscanSettings)
        {
            _gnosisscanSettings = gnosisscanSettings.Value;
            _client = new()
            {
                BaseAddress = new(gnosisscanSettings.Value.ApiBaseUrl ??
                                  throw new ArgumentNullException(nameof(gnosisscanSettings.Value.ApiBaseUrl)))
            };
        }

        /// <inheritdoc/>
        public async Task<GnosisscanAccount> GetBalanceAsync(string address)
        {
            string request =
                $"/api?module=account&action=balance&address={address}&tag=latest";
            if (!string.IsNullOrWhiteSpace(_gnosisscanSettings.ApiKey))
            {
                request += $"&apiKey={_gnosisscanSettings.ApiKey}";
            }

            var response = await _client.GetAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GnosisscanAccount>().ConfigureAwait(false) ?? throw new CustomException("Can't get account balance.");
        }

        /// <inheritdoc/>
        public async Task<GnosisscanAccount> GetTokenBalanceAsync(string address, string contractAddress)
        {
            string request =
                $"/api?module=account&action=tokenbalance&address={address}&contractaddress={contractAddress}&tag=latest";
            if (!string.IsNullOrWhiteSpace(_gnosisscanSettings.ApiKey))
            {
                request += $"&apiKey={_gnosisscanSettings.ApiKey}";
            }

            var response = await _client.GetAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GnosisscanAccount>().ConfigureAwait(false) ?? throw new CustomException("Can't get account token balance.");
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TResultItem>> GetTransactionsAsync<TResult, TResultItem>(string address)
            where TResult : IGnosisscanTransferList<TResultItem>
            where TResultItem : IGnosisscanTransfer
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

            if (typeof(TResult) == typeof(GnosisscanAccountNormalTransactions))
            {
                request = $"{request}&action=txlist";
            }
            else if (typeof(TResult) == typeof(GnosisscanAccountInternalTransactions))
            {
                request = $"{request}&action=txlistinternal";
            }
            else if (typeof(TResult) == typeof(GnosisscanAccountERC20TokenEvents))
            {
                request = $"{request}&action=tokentx";
            }
            else if (typeof(TResult) == typeof(GnosisscanAccountERC721TokenEvents))
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

            if (!string.IsNullOrWhiteSpace(_gnosisscanSettings.ApiKey))
            {
                request += $"&apiKey={_gnosisscanSettings.ApiKey}";
            }

            var response = await _client.GetAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResult>().ConfigureAwait(false) ?? throw new CustomException("Can't get account transactions.");
        }
    }
}