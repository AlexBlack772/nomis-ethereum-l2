// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonscanService.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Net;
using System.Numerics;
using System.Text.Json;

using Microsoft.Extensions.Options;
using Nethereum.Util;
using Nomis.Aave.Interfaces;
using Nomis.Aave.Interfaces.Contracts;
using Nomis.Aave.Interfaces.Enums;
using Nomis.Aave.Interfaces.Responses;
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
using Nomis.Polygonscan.Calculators;
using Nomis.Polygonscan.Interfaces;
using Nomis.Polygonscan.Interfaces.Extensions;
using Nomis.Polygonscan.Interfaces.Models;
using Nomis.Polygonscan.Settings;
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
using Nomis.Utils.Enums;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Extensions;
using Nomis.Utils.Wrapper;

namespace Nomis.Polygonscan
{
    /// <inheritdoc cref="IPolygonScoringService"/>
    internal sealed class PolygonscanService :
        BlockchainDescriptor,
        IPolygonScoringService,
        IHasDefiLlamaIntegration,
        ITransientService
    {
        private readonly IPolygonscanClient _client;
        private readonly ICoingeckoService _coingeckoService;
        private readonly IScoringService _scoringService;
        private readonly IEvmSoulboundTokenService _soulboundTokenService;
        private readonly ISnapshotService _snapshotService;
        private readonly IDexProviderService _dexProviderService;
        private readonly IDefiLlamaService _defiLlamaService;
        private readonly IAaveService _aaveService;
        private readonly IGreysafeService _greysafeService;
        private readonly IChainanalysisService _chainanalysisService;
        private readonly ICyberConnectService _cyberConnectService;

        /// <summary>
        /// Initialize <see cref="PolygonscanService"/>.
        /// </summary>
        /// <param name="settings"><see cref="PolygonscanSettings"/>.</param>
        /// <param name="client"><see cref="IPolygonscanClient"/>.</param>
        /// <param name="coingeckoService"><see cref="ICoingeckoService"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        /// <param name="soulboundTokenService"><see cref="IEvmSoulboundTokenService"/>.</param>
        /// <param name="snapshotService"><see cref="ISnapshotService"/>.</param>
        /// <param name="defiLlamaService"><see cref="IDefiLlamaService"/>.</param>
        /// <param name="aaveService"><see cref="IAaveService"/>.</param>
        /// <param name="dexProviderService"><see cref="IDexProviderService"/>.</param>
        /// <param name="greysafeService"><see cref="IGreysafeService"/>.</param>
        /// <param name="chainanalysisService"><see cref="IChainanalysisService"/>.</param>
        /// <param name="cyberConnectService"><see cref="ICyberConnectService"/>.</param>
        public PolygonscanService(
            IOptions<PolygonscanSettings> settings,
            IPolygonscanClient client,
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
            _dexProviderService = dexProviderService;
            _defiLlamaService = defiLlamaService;
            _aaveService = aaveService;
            _greysafeService = greysafeService;
            _chainanalysisService = chainanalysisService;
            _cyberConnectService = cyberConnectService;
        }

        /// <inheritdoc />
        public string DefiLLamaChainId => "polygon";

        /// <inheritdoc />
        public string CoingeckoNativeTokenId => "matic-network";

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

            string tokenName = "Matic";
            string? balanceWei = (await _client.GetBalanceAsync(request.Address).ConfigureAwait(false)).Balance;
            TokenPriceData? priceData = null;
            (await _defiLlamaService.TokensPriceAsync(new List<string?> { $"coingecko:{CoingeckoNativeTokenId}" }).ConfigureAwait(false))?.TokensPrices.TryGetValue($"coingecko:{CoingeckoNativeTokenId}", out priceData);
            decimal usdBalance = (priceData?.Price ?? 0M) * balanceWei?.ToMatic() ?? 0;
            decimal multiplier = 1;
            var tokenTransfers = new List<IPolygonscanAccountNftTokenEvent>();
            var erc20Tokens = (await _client.GetTransactionsAsync<PolygonscanAccountERC20TokenEvents, PolygonscanAccountERC20TokenEvent>(request.Address).ConfigureAwait(false)).ToList();
            await Task.Delay(100, cancellationToken).ConfigureAwait(false);

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

            #region Aave protocol

            AaveUserAccountDataResponse? aaveAccountDataResponse = null;
            if ((request as IWalletAaveProtocolRequest)?.GetAaveProtocolData == true)
            {
                aaveAccountDataResponse = (await _aaveService.GetAaveUserAccountDataAsync(AaveChain.Polygon, request.Address).ConfigureAwait(false)).Data;
            }

            #endregion Aave protocol

            TWalletStats? walletStats;
            switch (request.ScoreType)
            {
                case ScoreType.Finance:
                    await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                    var transactions = (await _client.GetTransactionsAsync<PolygonscanAccountNormalTransactions, PolygonscanAccountNormalTransaction>(request.Address).ConfigureAwait(false)).ToList();
                    await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                    var internalTransactions = (await _client.GetTransactionsAsync<PolygonscanAccountInternalTransactions, PolygonscanAccountInternalTransaction>(request.Address).ConfigureAwait(false)).ToList();
                    await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                    var erc721Tokens = (await _client.GetTransactionsAsync<PolygonscanAccountERC721TokenEvents, PolygonscanAccountERC721TokenEvent>(request.Address).ConfigureAwait(false)).ToList();
                    await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                    var erc1155Tokens = (await _client.GetTransactionsAsync<PolygonscanAccountERC1155TokenEvents, PolygonscanAccountERC1155TokenEvent>(request.Address).ConfigureAwait(false)).ToList();
                    tokenTransfers.AddRange(erc721Tokens);
                    tokenTransfers.AddRange(erc1155Tokens);

                    var financeCalculator = new PolygonStatCalculator(
                        request.Address,
                        decimal.TryParse(balanceWei, out decimal weiBalance) ? weiBalance : 0,
                        usdBalance,
                        transactions,
                        internalTransactions,
                        tokenTransfers,
                        erc20Tokens,
                        snapshotData,
                        tokenBalances.TokenBalances,
                        dexTokenSwapPairs,
                        aaveAccountDataResponse,
                        greysafeReportsResponse?.Reports,
                        chainanalysisReportsResponse?.Identifications,
                        cyberConnectData);
                    walletStats = financeCalculator.Stats() as TWalletStats;

                    break;
                case ScoreType.Token:
                default:
                    if (string.IsNullOrWhiteSpace(request.TokenAddress))
                    {
                        throw new CustomException("Token contract address should be set", statusCode: HttpStatusCode.BadRequest);
                    }

                    erc20Tokens = erc20Tokens.Where(t =>
                        t.ContractAddress?.Equals(request.TokenAddress, StringComparison.InvariantCultureIgnoreCase) == true).ToList();

                    string? tokenBalanceWei = (await _client.GetTokenBalanceAsync(request.Address, request.TokenAddress).ConfigureAwait(false)).Balance;
                    decimal.TryParse(tokenBalanceWei, NumberStyles.AllowDecimalPoint, new NumberFormatInfo { CurrencyDecimalSeparator = "." }, out decimal tokenBalance);
                    var tokenData = await _coingeckoService.GetTokenDataAsync("polygon-pos", request.TokenAddress).ConfigureAwait(false);
                    decimal tokenUsdBalance = 0;
                    if (tokenData != null && tokenData.DetailPlatforms.ContainsKey("polygon-pos") && !string.IsNullOrWhiteSpace(tokenData.Id))
                    {
                        tokenName = tokenData.Name ?? string.Empty;
                        int decimals = tokenData.DetailPlatforms["polygon-pos"].DecimalPlace;
                        multiplier = 1;
                        for (int i = 0; i < decimals; i++)
                        {
                            multiplier /= 10;
                        }

                        priceData = null;
                        (await _defiLlamaService.TokensPriceAsync(new List<string?> { $"coingecko:{tokenData.Id}" }).ConfigureAwait(false))?.TokensPrices.TryGetValue($"coingecko:{tokenData.Id}", out priceData);
                        tokenUsdBalance = (priceData?.Price ?? 0M) * tokenBalance.ToTokenValue(multiplier);
                    }

                    var tokenCalculator = new PolygonTokenStatCalculator(
                        request.Address,
                        decimal.TryParse(balanceWei, out decimal wei1) ? wei1.ToMatic() : 0,
                        usdBalance,
                        decimal.TryParse(tokenBalanceWei, out decimal wei2) ? wei2 : 0,
                        tokenUsdBalance,
                        erc20Tokens,
                        snapshotData,
                        tokenBalances.TokenBalances,
                        greysafeReportsResponse?.Reports,
                        chainanalysisReportsResponse?.Identifications,
                        cyberConnectData);
                    walletStats = tokenCalculator.TokenStats(tokenName, multiplier) as TWalletStats;

                    break;
            }

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