// ------------------------------------------------------------------------------------------------------
// <copyright file="ArbiscanService.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;
using System.Text.Json;

using Microsoft.Extensions.Options;
using Nethereum.Util;
using Nomis.Aave.Interfaces;
using Nomis.Aave.Interfaces.Contracts;
using Nomis.Aave.Interfaces.Enums;
using Nomis.Aave.Interfaces.Responses;
using Nomis.Arbiscan.Calculators;
using Nomis.Arbiscan.Interfaces;
using Nomis.Arbiscan.Interfaces.Extensions;
using Nomis.Arbiscan.Interfaces.Models;
using Nomis.Arbiscan.Settings;
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
using Nomis.Utils.Wrapper;

namespace Nomis.Arbiscan
{
    /// <inheritdoc cref="IArbitrumOneScoringService"/>
    internal sealed class ArbiscanService :
        BlockchainDescriptor,
        IArbitrumOneScoringService,
        IHasDefiLlamaIntegration,
        ITransientService
    {
        private readonly IArbiscanClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;
        private readonly IEvmSoulboundTokenService _soulboundTokenService;
        private readonly ISnapshotService _snapshotService;
        private readonly IAaveService _aaveService;
        private readonly IGreysafeService _greysafeService;
        private readonly IChainanalysisService _chainanalysisService;
        private readonly ICyberConnectService _cyberConnectService;
        private readonly IDexProviderService _dexProviderService;
        private readonly IDefiLlamaService _defiLlamaService;

        /// <summary>
        /// Initialize <see cref="ArbiscanService"/>.
        /// </summary>
        /// <param name="settings"><see cref="ArbiscanSettings"/>.</param>
        /// <param name="client"><see cref="IArbiscanClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        /// <param name="soulboundTokenService"><see cref="IEvmSoulboundTokenService"/>.</param>
        /// <param name="snapshotService"><see cref="ISnapshotService"/>.</param>
        /// <param name="dexProviderService"><see cref="IDexProviderService"/>.</param>
        /// <param name="defiLlamaService"><see cref="IDefiLlamaService"/>.</param>
        /// <param name="aaveService"><see cref="IAaveService"/>.</param>
        /// <param name="greysafeService"><see cref="IGreysafeService"/>.</param>
        /// <param name="chainanalysisService"><see cref="IChainanalysisService"/>.</param>
        /// <param name="cyberConnectService"><see cref="ICyberConnectService"/>.</param>
        public ArbiscanService(
            IOptions<ArbiscanSettings> settings,
            IArbiscanClient client,
            ICoingeckoService coingeckoService,
            IScoringService scoringService,
            IEvmSoulboundTokenService soulboundTokenService,
            ISnapshotService snapshotService,
            IDexProviderService dexProviderService,
            IDefiLlamaService defiLlamaService,
            IAaveService aaveService,
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
            _aaveService = aaveService;
            _greysafeService = greysafeService;
            _chainanalysisService = chainanalysisService;
            _cyberConnectService = cyberConnectService;
            _dexProviderService = dexProviderService;
            _defiLlamaService = defiLlamaService;
        }

        /// <inheritdoc />
        public string DefiLLamaChainId => "arbitrum";

        /// <inheritdoc />
        public string CoingeckoNativeTokenId => "ethereum";

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
            decimal usdBalance = (priceData?.Price ?? 0M) * balanceWei?.ToEth() ?? 0;
            await Task.Delay(100, cancellationToken).ConfigureAwait(false);
            var transactions = (await _client.GetTransactionsAsync<ArbiscanAccountNormalTransactions, ArbiscanAccountNormalTransaction>(request.Address).ConfigureAwait(false)).ToList();
            await Task.Delay(100, cancellationToken).ConfigureAwait(false);
            var internalTransactions = (await _client.GetTransactionsAsync<ArbiscanAccountInternalTransactions, ArbiscanAccountInternalTransaction>(request.Address).ConfigureAwait(false)).ToList();
            await Task.Delay(100, cancellationToken).ConfigureAwait(false);
            var erc20Tokens = (await _client.GetTransactionsAsync<ArbiscanAccountERC20TokenEvents, ArbiscanAccountERC20TokenEvent>(request.Address).ConfigureAwait(false)).ToList();
            await Task.Delay(100, cancellationToken).ConfigureAwait(false);
            var tokens = (await _client.GetTransactionsAsync<ArbiscanAccountERC721TokenEvents, ArbiscanAccountERC721TokenEvent>(request.Address).ConfigureAwait(false)).ToList();

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
                    // TODO - use Polly for 403
                    await Task.Delay(500, cancellationToken).ConfigureAwait(false);
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

            #region Aave protocol

            AaveUserAccountDataResponse? aaveAccountDataResponse = null;
            if ((request as IWalletAaveProtocolRequest)?.GetAaveProtocolData == true)
            {
                aaveAccountDataResponse = (await _aaveService.GetAaveUserAccountDataAsync(AaveChain.ArbitrumOne, request.Address).ConfigureAwait(false)).Data;
            }

            #endregion Aave protocol

            var walletStats = new ArbitrumOneStatCalculator(
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
                    aaveAccountDataResponse,
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