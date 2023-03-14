// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonStatCalculator.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

using Nomis.Aave.Interfaces.Calculators;
using Nomis.Aave.Interfaces.Responses;
using Nomis.Blockchain.Abstractions.Calculators;
using Nomis.Blockchain.Abstractions.Stats;
using Nomis.Chainanalysis.Interfaces.Calculators;
using Nomis.Chainanalysis.Interfaces.Models;
using Nomis.CyberConnect.Interfaces.Calculators;
using Nomis.CyberConnect.Interfaces.Models;
using Nomis.CyberConnect.Interfaces.Responses;
using Nomis.DefiLlama.Interfaces.Models;
using Nomis.Dex.Abstractions.Calculators;
using Nomis.Dex.Abstractions.Contracts;
using Nomis.Greysafe.Interfaces.Calculators;
using Nomis.Greysafe.Interfaces.Models;
using Nomis.Polygonscan.Interfaces.Extensions;
using Nomis.Polygonscan.Interfaces.Models;
using Nomis.Snapshot.Interfaces.Calculators;
using Nomis.Snapshot.Interfaces.Models;
using Nomis.Snapshot.Interfaces.Responses;
using Nomis.Utils.Contracts;
using Nomis.Utils.Contracts.Calculators;
using Nomis.Utils.Extensions;

namespace Nomis.Polygonscan.Calculators
{
    /// <summary>
    /// Polygon wallet stats calculator.
    /// </summary>
    internal sealed class PolygonStatCalculator :
        IWalletCommonStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>,
        IWalletNativeBalanceStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>,
        IWalletTokenBalancesStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>,
        IWalletTransactionStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>,
        IWalletTokenStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>,
        IWalletContractStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>,
        IWalletSnapshotStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>,
        IWalletGreysafeStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>,
        IWalletChainanalysisStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>,
        IWalletCyberConnectStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>,
        IWalletNftStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>,
        IWalletAaveStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>,
        IWalletDexTokenSwapPairsStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>
    {
        private readonly string _address;
        private readonly decimal _balance;
        private readonly IEnumerable<PolygonscanAccountNormalTransaction> _transactions;
        private readonly IEnumerable<PolygonscanAccountInternalTransaction> _internalTransactions;
        private readonly IEnumerable<IPolygonscanAccountNftTokenEvent> _tokenTransfers;
        private readonly IEnumerable<PolygonscanAccountERC20TokenEvent> _erc20TokenTransfers;
        private readonly IEnumerable<TokenBalanceData>? _tokenBalances;
        private readonly IEnumerable<DexTokenSwapPairsData> _dexTokenSwapPairs;
        private readonly IEnumerable<GreysafeReport>? _greysafeReports;
        private readonly IEnumerable<ChainanalysisReport>? _chainanalysisReports;
        private readonly IEnumerable<CyberConnectLikeData>? _cyberConnectLikes;
        private readonly IEnumerable<CyberConnectEssenceData>? _cyberConnectEssences;
        private readonly IEnumerable<CyberConnectSubscribingProfileData>? _cyberConnectSubscribings;

        /// <inheritdoc />
        public int WalletAge => _transactions.Any()
            ? IWalletStatsCalculator.GetWalletAge(_transactions.Select(x => x.TimeStamp!.ToDateTime()))
            : 1;

        /// <inheritdoc />
        public IList<PolygonTransactionIntervalData> TurnoverIntervals
        {
            get
            {
                var turnoverIntervalsDataList =
                    _transactions.Select(x => new TurnoverIntervalsData(
                        x.TimeStamp!.ToDateTime(),
                        BigInteger.TryParse(x.Value, out var value) ? value : 0,
                        x.From?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true));
                return IWalletStatsCalculator<PolygonTransactionIntervalData>
                    .GetTurnoverIntervals(turnoverIntervalsDataList, _transactions.Any() ? _transactions.Min(x => x.TimeStamp!.ToDateTime()) : DateTime.MinValue).ToList();
            }
        }

        /// <inheritdoc />
        public decimal NativeBalance => _balance.ToMatic();

        /// <inheritdoc />
        public decimal NativeBalanceUSD { get; }

        /// <inheritdoc />
        public decimal BalanceChangeInLastMonth =>
            IWalletStatsCalculator<PolygonTransactionIntervalData>.GetBalanceChangeInLastMonth(TurnoverIntervals);

        /// <inheritdoc />
        public decimal BalanceChangeInLastYear =>
            IWalletStatsCalculator<PolygonTransactionIntervalData>.GetBalanceChangeInLastYear(TurnoverIntervals);

        /// <inheritdoc />
        public decimal WalletTurnover =>
            _transactions.Sum(x => decimal.TryParse(x.Value, out decimal value) ? value.ToMatic() : 0);

        /// <inheritdoc />
        public IEnumerable<TokenBalanceData>? TokenBalances
            => _tokenBalances?.Any() == true ? _tokenBalances : null;

        /// <inheritdoc />
        public int TokensHolding
            => _erc20TokenTransfers.Select(x => x.TokenSymbol).Distinct().Count();

        /// <inheritdoc />
        public int DeployedContracts
            => _transactions.Count(x => !string.IsNullOrWhiteSpace(x.ContractAddress));

        /// <inheritdoc />
        public IEnumerable<SnapshotProposal>? SnapshotProposals { get; }

        /// <inheritdoc />
        public IEnumerable<SnapshotVote>? SnapshotVotes { get; }

        /// <inheritdoc />
        public IEnumerable<GreysafeReport>? GreysafeReports
            => _greysafeReports?.Any() == true ? _greysafeReports : null;

        /// <inheritdoc />
        public IEnumerable<ChainanalysisReport>? ChainanalysisReports =>
            _chainanalysisReports?.Any() == true ? _chainanalysisReports : null;

        /// <inheritdoc />
        public CyberConnectProfileData? CyberConnectProfile { get; }

        /// <inheritdoc />
        public IEnumerable<CyberConnectLikeData>? CyberConnectLikes
            => _cyberConnectLikes?.Any() == true ? _cyberConnectLikes : null;

        /// <inheritdoc />
        public IEnumerable<CyberConnectEssenceData>? CyberConnectEssences
            => _cyberConnectEssences?.Any() == true ? _cyberConnectEssences : null;

        /// <inheritdoc />
        public IEnumerable<CyberConnectSubscribingProfileData>? CyberConnectSubscribings
            => _cyberConnectSubscribings?.Any() == true ? _cyberConnectSubscribings : null;

        /// <inheritdoc />
        public AaveUserAccountDataResponse? AaveData { get; }

        /// <inheritdoc />
        public IEnumerable<DexTokenSwapPairsData>? DexTokensSwapPairs => _dexTokenSwapPairs.Any() ? _dexTokenSwapPairs : null;

        public PolygonStatCalculator(
            string address,
            decimal balance,
            decimal usdBalance,
            IEnumerable<PolygonscanAccountNormalTransaction> transactions,
            IEnumerable<PolygonscanAccountInternalTransaction> internalTransactions,
            IEnumerable<IPolygonscanAccountNftTokenEvent> tokenTransfers,
            IEnumerable<PolygonscanAccountERC20TokenEvent> erc20TokenTransfers,
            SnapshotData? snapshotData,
            IEnumerable<TokenBalanceData>? tokenBalances,
            IEnumerable<DexTokenSwapPairsData> dexTokenSwapPairs,
            AaveUserAccountDataResponse? aaveUserAccountData,
            IEnumerable<GreysafeReport>? greysafeReports,
            IEnumerable<ChainanalysisReport>? chainanalysisReports,
            CyberConnectData? cyberConnectData)
        {
            _address = address;
            _balance = balance;
            NativeBalanceUSD = usdBalance;
            _transactions = transactions;
            _internalTransactions = internalTransactions;
            _tokenTransfers = tokenTransfers;
            _erc20TokenTransfers = erc20TokenTransfers;
            SnapshotVotes = snapshotData?.Votes;
            SnapshotProposals = snapshotData?.Proposals;
            _tokenBalances = tokenBalances;
            _dexTokenSwapPairs = dexTokenSwapPairs;
            AaveData = aaveUserAccountData;
            _greysafeReports = greysafeReports;
            _chainanalysisReports = chainanalysisReports;
            _cyberConnectLikes = cyberConnectData?.Likes;
            _cyberConnectEssences = cyberConnectData?.Essences;
            _cyberConnectSubscribings = cyberConnectData?.Subscribings;
            CyberConnectProfile = cyberConnectData?.Profile;
        }

        /// <inheritdoc />
        public PolygonWalletStats Stats()
        {
            return (this as IWalletStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>).ApplyCalculators();
        }

        /// <inheritdoc />
        IWalletTransactionStats IWalletTransactionStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>.Stats()
        {
            if (!_transactions.Any())
            {
                return new PolygonWalletStats
                {
                    NoData = true
                };
            }

            var intervals = IWalletStatsCalculator
                .GetTransactionsIntervals(_transactions.Select(x => x.TimeStamp!.ToDateTime())).ToList();
            if (intervals.Count == 0)
            {
                return new PolygonWalletStats
                {
                    NoData = true
                };
            }

            var monthAgo = DateTime.Now.AddMonths(-1);
            var yearAgo = DateTime.Now.AddYears(-1);

            return new PolygonWalletStats
            {
                TotalTransactions = _transactions.Count(),
                TotalRejectedTransactions = _transactions.Count(t => string.Equals(t.IsError, "1", StringComparison.OrdinalIgnoreCase)),
                MinTransactionTime = intervals.Min(),
                MaxTransactionTime = intervals.Max(),
                AverageTransactionTime = intervals.Average(),
                LastMonthTransactions = _transactions.Count(x => x.TimeStamp!.ToDateTime() > monthAgo),
                LastYearTransactions = _transactions.Count(x => x.TimeStamp!.ToDateTime() > yearAgo),
                TimeFromLastTransaction = (int)((DateTime.UtcNow - _transactions.OrderBy(x => x.TimeStamp).Last().TimeStamp!.ToDateTime()).TotalDays / 30)
            };
        }

        /// <inheritdoc />
        IWalletNftStats IWalletNftStatsCalculator<PolygonWalletStats, PolygonTransactionIntervalData>.Stats()
        {
            var soldTokens = _tokenTransfers.Where(x => x.From?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true).ToList();
            var soldSum = IWalletStatsCalculator
                .TokensSum(soldTokens.Select(x => x.Hash!), _internalTransactions.Select(x => (x.Hash!, BigInteger.TryParse(x.Value, out var amount) ? amount : 0)));

            var soldTokensIds = soldTokens.Select(x => x.GetTokenUid());
            var buyTokens = _tokenTransfers.Where(x => x.To?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true && soldTokensIds.Contains(x.GetTokenUid()));
            var buySum = IWalletStatsCalculator
                .TokensSum(buyTokens.Select(x => x.Hash!), _internalTransactions.Select(x => (x.Hash!, BigInteger.TryParse(x.Value, out var amount) ? amount : 0)));

            var buyNotSoldTokens = _tokenTransfers.Where(x => x.To?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true && !soldTokensIds.Contains(x.GetTokenUid()));
            var buyNotSoldSum = IWalletStatsCalculator
                .TokensSum(buyNotSoldTokens.Select(x => x.Hash!), _internalTransactions.Select(x => (x.Hash!, BigInteger.TryParse(x.Value, out var amount) ? amount : 0)));

            int holdingTokens = _tokenTransfers.Count() - soldTokens.Count;
            decimal nftWorth = buySum == 0 ? 0 : (decimal)soldSum / (decimal)buySum * (decimal)buyNotSoldSum;

            return new PolygonWalletStats
            {
                NftHolding = holdingTokens,
                NftTrading = (soldSum - buySum).ToMatic(),
                NftWorth = nftWorth.ToMatic()
            };
        }
    }
}