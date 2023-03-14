// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonTokenStatCalculator.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

using Nomis.Blockchain.Abstractions.Calculators;
using Nomis.Blockchain.Abstractions.Stats;
using Nomis.Chainanalysis.Interfaces.Calculators;
using Nomis.Chainanalysis.Interfaces.Models;
using Nomis.CyberConnect.Interfaces.Calculators;
using Nomis.CyberConnect.Interfaces.Models;
using Nomis.CyberConnect.Interfaces.Responses;
using Nomis.DefiLlama.Interfaces.Models;
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
    /// Polygon wallet token stats calculator.
    /// </summary>
    internal sealed class PolygonTokenStatCalculator :
        IWalletCommonStatsCalculator<PolygonWalletTokenStats, PolygonTransactionIntervalData>,
        IWalletNativeBalanceStatsCalculator<PolygonWalletTokenStats, PolygonTransactionIntervalData>,
        IWalletScoredTokenStatsCalculator<PolygonWalletTokenStats, PolygonTransactionIntervalData>,
        IWalletTokenBalancesStatsCalculator<PolygonWalletTokenStats, PolygonTransactionIntervalData>,
        IWalletTransactionStatsCalculator<PolygonWalletTokenStats, PolygonTransactionIntervalData>,
        IWalletTokenStatsCalculator<PolygonWalletTokenStats, PolygonTransactionIntervalData>,
        IWalletContractStatsCalculator<PolygonWalletTokenStats, PolygonTransactionIntervalData>,
        IWalletSnapshotStatsCalculator<PolygonWalletTokenStats, PolygonTransactionIntervalData>,
        IWalletGreysafeStatsCalculator<PolygonWalletTokenStats, PolygonTransactionIntervalData>,
        IWalletChainanalysisStatsCalculator<PolygonWalletTokenStats, PolygonTransactionIntervalData>,
        IWalletCyberConnectStatsCalculator<PolygonWalletTokenStats, PolygonTransactionIntervalData>
    {
        private readonly string _address;
        private readonly decimal _balance;
        private readonly decimal _usdBalance;
        private readonly IEnumerable<PolygonscanAccountERC20TokenEvent> _erc20TokenTransfers;
        private readonly IEnumerable<TokenBalanceData>? _tokenBalances;
        private readonly IEnumerable<GreysafeReport>? _greysafeReports;
        private readonly IEnumerable<ChainanalysisReport>? _chainanalysisReports;
        private readonly IEnumerable<CyberConnectLikeData>? _cyberConnectLikes;
        private readonly IEnumerable<CyberConnectEssenceData>? _cyberConnectEssences;
        private readonly IEnumerable<CyberConnectSubscribingProfileData>? _cyberConnectSubscribings;

        /// <inheritdoc />
        public int WalletAge => _erc20TokenTransfers.Any()
            ? IWalletStatsCalculator.GetWalletAge(_erc20TokenTransfers.Select(x => x.TimeStamp!.ToDateTime()))
            : 1;

        /// <inheritdoc />
        public IList<PolygonTransactionIntervalData> TurnoverIntervals
        {
            get
            {
                var turnoverIntervalsDataList =
                    _erc20TokenTransfers.Select(x => new TurnoverIntervalsData(
                        x.TimeStamp!.ToDateTime(),
                        BigInteger.TryParse(x.Value, out var value) ? value : 0,
                        x.From?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true));
                return IWalletStatsCalculator<PolygonTransactionIntervalData>
                    .GetTurnoverIntervals(turnoverIntervalsDataList, _erc20TokenTransfers.Any() ? _erc20TokenTransfers.Min(x => x.TimeStamp!.ToDateTime()) : DateTime.MinValue).ToList();
            }
        }

        /// <inheritdoc />
        public string? Token { get; set; }

        /// <inheritdoc />
        public decimal TokenBalance { get; set; }

        /// <inheritdoc />
        public decimal TokenBalanceUSD { get; set; }

        /// <inheritdoc />
        public decimal NativeBalance { get; }

        /// <inheritdoc />
        public decimal NativeBalanceUSD { get; }

        /// <inheritdoc />
        public decimal BalanceChangeInLastMonth =>
            IWalletStatsCalculator<PolygonTransactionIntervalData>.GetBalanceChangeInLastMonth(TurnoverIntervals);

        /// <inheritdoc />
        public decimal BalanceChangeInLastYear =>
            IWalletStatsCalculator<PolygonTransactionIntervalData>.GetBalanceChangeInLastYear(TurnoverIntervals);

        /// <inheritdoc />
        public decimal WalletTurnover => 0;

        /// <inheritdoc />
        public IEnumerable<TokenBalanceData>? TokenBalances
            => _tokenBalances?.Any() == true ? _tokenBalances?.OrderByDescending(b => b.TotalAmountPrice) : null;

        /// <inheritdoc />
        public int TokensHolding => 1;

        /// <inheritdoc />
        public int DeployedContracts => 0;

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

        public PolygonTokenStatCalculator(
            string address,
            decimal nativeBalance,
            decimal nativeUsdBalance,
            decimal balance,
            decimal usdBalance,
            IEnumerable<PolygonscanAccountERC20TokenEvent> erc20TokenTransfers,
            SnapshotData? snapshotData,
            IEnumerable<TokenBalanceData>? tokenBalances,
            IEnumerable<GreysafeReport>? greysafeReports,
            IEnumerable<ChainanalysisReport>? chainanalysisReports,
            CyberConnectData? cyberConnectData)
        {
            _address = address;
            _balance = balance;
            _usdBalance = usdBalance;
            NativeBalance = nativeBalance;
            NativeBalanceUSD = nativeUsdBalance;
            _erc20TokenTransfers = erc20TokenTransfers;
            SnapshotVotes = snapshotData?.Votes;
            SnapshotProposals = snapshotData?.Proposals;
            _tokenBalances = tokenBalances;
            _greysafeReports = greysafeReports;
            _chainanalysisReports = chainanalysisReports;
            _cyberConnectLikes = cyberConnectData?.Likes;
            _cyberConnectEssences = cyberConnectData?.Essences;
            _cyberConnectSubscribings = cyberConnectData?.Subscribings;
            CyberConnectProfile = cyberConnectData?.Profile;
        }

        /// <summary>
        /// Get blockchain wallet token stats.
        /// </summary>
        /// <param name="tokenName">The token name.</param>
        /// <param name="multiplier">Token balance multiplier.</param>
        /// <returns>Returns <see cref="PolygonWalletTokenStats"/>.</returns>
        public PolygonWalletTokenStats TokenStats(string tokenName, decimal multiplier)
        {
            Token = tokenName;
            TokenBalance = _balance.ToTokenValue(multiplier);
            TokenBalanceUSD = _usdBalance;
            var result = (this as IWalletStatsCalculator<PolygonWalletTokenStats, PolygonTransactionIntervalData>).ApplyCalculators();
            result.WalletTurnover = _erc20TokenTransfers
                .Sum(x => decimal.TryParse(x.Value, out decimal value) ? value : 0).ToTokenValue(multiplier);

            return result;
        }

        public IWalletTransactionStats Stats()
        {
            if (!_erc20TokenTransfers.Any())
            {
                return new PolygonWalletTokenStats
                {
                    NoData = true
                };
            }

            var intervals = IWalletStatsCalculator
                .GetTransactionsIntervals(_erc20TokenTransfers.Select(x => x.TimeStamp!.ToDateTime())).ToList();
            if (intervals.Count == 0)
            {
                return new PolygonWalletTokenStats
                {
                    NoData = true
                };
            }

            var monthAgo = DateTime.Now.AddMonths(-1);
            var yearAgo = DateTime.Now.AddYears(-1);

            return new PolygonWalletTokenStats
            {
                TotalTransactions = _erc20TokenTransfers.Count(),
                TotalRejectedTransactions = 0,
                MinTransactionTime = intervals.Min(),
                MaxTransactionTime = intervals.Max(),
                AverageTransactionTime = intervals.Average(),
                LastMonthTransactions = _erc20TokenTransfers.Count(x => x.TimeStamp!.ToDateTime() > monthAgo),
                LastYearTransactions = _erc20TokenTransfers.Count(x => x.TimeStamp!.ToDateTime() > yearAgo),
                TimeFromLastTransaction = (int)((DateTime.UtcNow - _erc20TokenTransfers.OrderBy(x => x.TimeStamp).Last().TimeStamp!.ToDateTime()).TotalDays / 30)
            };
        }
    }
}