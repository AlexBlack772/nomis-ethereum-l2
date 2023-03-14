// ------------------------------------------------------------------------------------------------------
// <copyright file="GnosisscanService.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;
using System.Text.Json;

using Microsoft.Extensions.Options;
using Nethereum.Util;
using Nomis.Blockchain.Abstractions;
using Nomis.Blockchain.Abstractions.Contracts;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Chainanalysis.Interfaces;
using Nomis.Chainanalysis.Interfaces.Contracts;
using Nomis.Chainanalysis.Interfaces.Extensions;
using Nomis.Coingecko.Interfaces;
using Nomis.CyberConnect.Interfaces;
using Nomis.CyberConnect.Interfaces.Contracts;
using Nomis.CyberConnect.Interfaces.Extensions;
using Nomis.DefiLlama.Interfaces;
using Nomis.DefiLlama.Interfaces.Contracts;
using Nomis.DefiLlama.Interfaces.Models;
using Nomis.Dex.Abstractions.Contracts;
using Nomis.Dex.Abstractions.Enums;
using Nomis.DexProviderService.Interfaces;
using Nomis.DexProviderService.Interfaces.Extensions;
using Nomis.Domain.Scoring.Entities;
using Nomis.Gnosisscan.Calculators;
using Nomis.Gnosisscan.Interfaces;
using Nomis.Gnosisscan.Interfaces.Extensions;
using Nomis.Gnosisscan.Interfaces.Models;
using Nomis.Gnosisscan.Settings;
using Nomis.Greysafe.Interfaces;
using Nomis.Greysafe.Interfaces.Contracts;
using Nomis.Greysafe.Interfaces.Extensions;
using Nomis.ScoringService.Interfaces;
using Nomis.Snapshot.Interfaces;
using Nomis.Snapshot.Interfaces.Contracts;
using Nomis.Snapshot.Interfaces.Extensions;
using Nomis.SoulboundTokenService.Interfaces;
using Nomis.SoulboundTokenService.Interfaces.Extensions;
using Nomis.Utils.Contracts;
using Nomis.Utils.Contracts.Requests;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Contracts.Stats;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Extensions;
using Nomis.Utils.Wrapper;

namespace Nomis.Gnosisscan
{
    /// <inheritdoc cref="IGnosisScoringService"/>
    internal sealed class GnosisscanService :
        BlockchainDescriptor,
        IGnosisScoringService,
        IHasDefiLlamaIntegration,
        ITransientService
    {
        private readonly IGnosisscanClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;
        private readonly IEvmSoulboundTokenService _soulboundTokenService;
        private readonly ISnapshotService _snapshotService;
        private readonly IDexProviderService _dexProviderService;
        private readonly IDefiLlamaService _defiLlamaService;
        private readonly IGreysafeService _greysafeService;
        private readonly IChainanalysisService _chainanalysisService;
        private readonly ICyberConnectService _cyberConnectService;

        /// <summary>
        /// Initialize <see cref="GnosisscanService"/>.
        /// </summary>
        /// <param name="settings"><see cref="GnosisscanSettings"/>.</param>
        /// <param name="client"><see cref="IGnosisscanClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        /// <param name="soulboundTokenService"><see cref="IEvmSoulboundTokenService"/>.</param>
        /// <param name="snapshotService"><see cref="ISnapshotService"/>.</param>
        /// <param name="dexProviderService"><see cref="IDexProviderService"/>.</param>
        /// <param name="defiLlamaService"><see cref="IDefiLlamaService"/>.</param>
        /// <param name="greysafeService"><see cref="IGreysafeService"/>.</param>
        /// <param name="chainanalysisService"><see cref="IChainanalysisService"/>.</param>
        /// <param name="cyberConnectService"><see cref="ICyberConnectService"/>.</param>
        public GnosisscanService(
            IOptions<GnosisscanSettings> settings,
            IGnosisscanClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService,
            IEvmSoulboundTokenService soulboundTokenService,
            ISnapshotService snapshotService,
            IDexProviderService dexProviderService,
            IDefiLlamaService defiLlamaService,
            IGreysafeService greysafeService,
            IChainanalysisService chainanalysisService,
            ICyberConnectService cyberConnectService)
            : base(settings.Value.BlockchainDescriptor)
        {
            _client = client;
            _coingeckoService = coingeckoService;
            _scoringService = scoringService;
            _soulboundTokenService = soulboundTokenService;
            _snapshotService = snapshotService;
            _dexProviderService = dexProviderService;
            _defiLlamaService = defiLlamaService;
            _greysafeService = greysafeService;
            _chainanalysisService = chainanalysisService;
            _cyberConnectService = cyberConnectService;
        }

        /// <inheritdoc />
        public string DefiLLamaChainId => "xdai";

        /// <inheritdoc />
        public string CoingeckoNativeTokenId => "xdai";

        /// <inheritdoc/>
        public async Task<Result<TWalletScore>> GetWalletStatsAsync<TWalletStatsRequest, TWalletScore, TWalletStats, TTransactionIntervalData>(
            TWalletStatsRequest request,
            CancellationToken cancellationToken = default)
            where TWalletStatsRequest : WalletStatsRequest
            where TWalletScore : IWalletScore<TWalletStats, TTransactionIntervalData>, new()
            where TWalletStats : class, IWalletCommonStats<TTransactionIntervalData>, new()
            where TTransactionIntervalData : class, ITransactionIntervalData
        {
            if (!new AddressUtil().IsValidAddressLength(request.Address) || !new AddressUtil().IsValidEthereumAddressHexFormat(request.Address))
            {
                throw new InvalidAddressException(request.Address);
            }

            string? balanceWei = (await _client.GetBalanceAsync(request.Address).ConfigureAwait(false)).Balance;
            TokenPriceData? priceData = null;
            (await _defiLlamaService.TokensPriceAsync(new List<string?> { $"coingecko:{CoingeckoNativeTokenId}" }).ConfigureAwait(false))?.TokensPrices.TryGetValue($"coingecko:{CoingeckoNativeTokenId}", out priceData);
            decimal usdBalance = (priceData?.Price ?? 0M) * balanceWei?.ToXdai() ?? 0;
            var transactions = (await _client.GetTransactionsAsync<GnosisscanAccountNormalTransactions, GnosisscanAccountNormalTransaction>(request.Address).ConfigureAwait(false)).ToList();
            var internalTransactions = (await _client.GetTransactionsAsync<GnosisscanAccountInternalTransactions, GnosisscanAccountInternalTransaction>(request.Address).ConfigureAwait(false)).ToList();
            var erc20Tokens = (await _client.GetTransactionsAsync<GnosisscanAccountERC20TokenEvents, GnosisscanAccountERC20TokenEvent>(request.Address).ConfigureAwait(false)).ToList();
            var tokens = (await _client.GetTransactionsAsync<GnosisscanAccountERC721TokenEvents, GnosisscanAccountERC721TokenEvent>(request.Address).ConfigureAwait(false)).ToList();

            #region Greysafe scam reports

            var greysafeReportsResponse = await _greysafeService.ReportsAsync(request as IWalletGreysafeRequest).ConfigureAwait(false);

            #endregion Greysafe scam reports

            #region Chainanalysis sanctions reports

            var chainanalysisReportsResponse = await _chainanalysisService.ReportsAsync(request as IWalletChainanalysisRequest).ConfigureAwait(false);

            #endregion Chainanalysis sanctions reports

            #region Snapshot protocol

            var snapshotData = await _snapshotService.DataAsync(request as IWalletSnapshotProtocolRequest, ChainId).ConfigureAwait(false);

            #endregion Snapshot protocol

            #region CyberConnect protocol

            var cyberConnectData = await _cyberConnectService.DataAsync(request as IWalletCyberConnectProtocolRequest, ChainId).ConfigureAwait(false);

            #endregion CyberConnect protocol

            #region Tokens data

            var tokensData = new List<(string TokenContractId, string? TokenContractIdWithBlockchain, BigInteger? Balance)>();
            if ((request as IWalletTokensSwapPairsRequest)?.GetTokensSwapPairs == true
                || (request as IWalletTokensBalancesRequest)?.GetHoldTokensBalances == true)
            {
                foreach (string? erc20TokenContractId in erc20Tokens.Select(x => x.ContractAddress).Distinct())
                {
                    await Task.Delay(250, cancellationToken).ConfigureAwait(false);
                    var tokenBalance = (await _client.GetTokenBalanceAsync(request.Address, erc20TokenContractId!).ConfigureAwait(false)).Balance?.ToBigInteger();
                    if (tokenBalance > 0)
                    {
                        tokensData.Add((erc20TokenContractId!, $"{DefiLLamaChainId}:{erc20TokenContractId}", tokenBalance));
                    }
                }
            }

            #endregion Tokens data

            #region Tokens balances (DefiLlama)

            var tokenBalances = await _dexProviderService
                .TokenBalancesAsync(_defiLlamaService, request as IWalletTokensBalancesRequest, tokensData, (Chain)ChainId).ConfigureAwait(false);

            #endregion Tokens balances

            #region Swap pairs from DEXes

            var dexTokenSwapPairs = new List<DexTokenSwapPairsData>();
            if ((request as IWalletTokensSwapPairsRequest)?.GetTokensSwapPairs == true && tokensData.Any())
            {
                var swapPairsResult = await _dexProviderService.BlockchainSwapPairsAsync(new()
                {
                    Blockchain = (Chain)ChainId,
                    First = (request as IWalletTokensSwapPairsRequest)?.FirstSwapPairs ?? 100,
                    Skip = (request as IWalletTokensSwapPairsRequest)?.Skip ?? 0,
                    FromCache = false
                }).ConfigureAwait(false);
                if (swapPairsResult.Succeeded)
                {
                    dexTokenSwapPairs.AddRange(tokensData.Select(t =>
                        DexTokenSwapPairsData.ForSwapPairs(t.TokenContractId, t.Balance, swapPairsResult.Data, tokenBalances.DexTokensData)));
                    dexTokenSwapPairs.RemoveAll(p => !p.TokenSwapPairs.Any());
                }
            }

            #endregion Swap pairs from DEXes

            var walletStats = new GnosisStatCalculator(
                    request.Address,
                    decimal.TryParse(balanceWei, out decimal wei) ? wei : 0,
                    usdBalance,
                    transactions,
                    internalTransactions,
                    tokens,
                    erc20Tokens,
                    snapshotData,
                    tokenBalances.TokenBalances,
                    dexTokenSwapPairs,
                    greysafeReportsResponse?.Reports,
                    chainanalysisReportsResponse?.Identifications,
                    cyberConnectData)
                .Stats() as TWalletStats;

            double score = walletStats!.CalculateScore<TWalletStats, TTransactionIntervalData>();
            var scoringData = new ScoringData(request.Address, request.Address, ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken).ConfigureAwait(false);

            // getting signature
            ushort mintedScore = (ushort)(score * 10000);
            var signatureResult = await _soulboundTokenService
                .SignatureAsync(request, mintedScore, ChainId, SBTContractAddresses, (request as IWalletGreysafeRequest)?.GetGreysafeData, (request as IWalletChainanalysisRequest)?.GetChainanalysisData).ConfigureAwait(false);
            var messages = signatureResult.Messages;
            messages.Add($"Got {ChainName} wallet {request.ScoreType.ToString()} score.");
            return await Result<TWalletScore>.SuccessAsync(
                new()
                {
                    Address = request.Address,
                    Stats = walletStats,
                    Score = score,
                    MintedScore = mintedScore,
                    Signature = signatureResult.Data.Signature
                }, messages).ConfigureAwait(false);
        }
    }
}