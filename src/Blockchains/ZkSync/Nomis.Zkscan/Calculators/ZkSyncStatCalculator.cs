// ------------------------------------------------------------------------------------------------------
// <copyright file="ZkSyncStatCalculator.cs" company="Nomis">
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
using Nomis.Snapshot.Interfaces.Calculators;
using Nomis.Snapshot.Interfaces.Models;
using Nomis.Snapshot.Interfaces.Responses;
using Nomis.Utils.Contracts;
using Nomis.Utils.Contracts.Calculators;
using Nomis.Utils.Extensions;
using Nomis.Zkscan.Interfaces.Extensions;
using Nomis.Zkscan.Interfaces.Models;

namespace Nomis.Zkscan.Calculators
{
    /// <summary>
    /// ZkSync wallet stats calculator.
    /// </summary>
    internal sealed class ZkSyncStatCalculator :
        IWalletCommonStatsCalculator<ZkSyncWalletStats, ZkSyncTransactionIntervalData>,
        IWalletNativeBalanceStatsCalculator<ZkSyncWalletStats, ZkSyncTransactionIntervalData>,
        IWalletTokenBalancesStatsCalculator<ZkSyncWalletStats, ZkSyncTransactionIntervalData>,
        IWalletTransactionStatsCalculator<ZkSyncWalletStats, ZkSyncTransactionIntervalData>,
        IWalletTokenStatsCalculator<ZkSyncWalletStats, ZkSyncTransactionIntervalData>,
        IWalletContractStatsCalculator<ZkSyncWalletStats, ZkSyncTransactionIntervalData>,
        IWalletSnapshotStatsCalculator<ZkSyncWalletStats, ZkSyncTransactionIntervalData>,
        IWalletGreysafeStatsCalculator<ZkSyncWalletStats, ZkSyncTransactionIntervalData>,
        IWalletChainanalysisStatsCalculator<ZkSyncWalletStats, ZkSyncTransactionIntervalData>,
        IWalletCyberConnectStatsCalculator<ZkSyncWalletStats, ZkSyncTransactionIntervalData>
    {
        private readonly string _address;
        private readonly IEnumerable<ZkscanAccountNormalTransaction> _transactions;
        private readonly IEnumerable<ZkscanAccountERC20TokenEvent> _erc20TokenTransfers;
        private readonly IEnumerable<TokenBalanceData>? _tokenBalances;
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
        public IList<ZkSyncTransactionIntervalData> TurnoverIntervals
        {
            get
            {
                var turnoverIntervalsDataList =
                    _transactions.Select(x => new TurnoverIntervalsData(
                        x.TimeStamp!.ToDateTime(),
                        BigInteger.TryParse(x.Value, out var value) ? value : 0,
                        x.From?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true));
                return IWalletStatsCalculator<ZkSyncTransactionIntervalData>
                    .GetTurnoverIntervals(turnoverIntervalsDataList, _transactions.Any() ? _transactions.Min(x => x.TimeStamp!.ToDateTime()) : DateTime.MinValue).ToList();
            }
        }

        /// <inheritdoc />
        public decimal NativeBalance { get; }

        /// <inheritdoc />
        public decimal NativeBalanceUSD { get; }

        /// <inheritdoc />
        public decimal BalanceChangeInLastMonth =>
            IWalletStatsCalculator<ZkSyncTransactionIntervalData>.GetBalanceChangeInLastMonth(TurnoverIntervals);

        /// <inheritdoc />
        public decimal BalanceChangeInLastYear =>
            IWalletStatsCalculator<ZkSyncTransactionIntervalData>.GetBalanceChangeInLastYear(TurnoverIntervals);

        /// <inheritdoc />
        public decimal WalletTurnover =>
            _transactions.Sum(x => decimal.TryParse(x.Value, out decimal value) ? value.ToEth() : 0);

        /// <inheritdoc />
        public IEnumerable<TokenBalanceData>? TokenBalances => _tokenBalances?.Any() == true ? _tokenBalances : null;

        /// <inheritdoc />
        public int TokensHolding => _erc20TokenTransfers.Select(x => x.TokenSymbol).Distinct().Count();

        /// <inheritdoc />
        public int DeployedContracts => _transactions.Count(x => !string.IsNullOrWhiteSpace(x.ContractAddress));

        /// <inheritdoc />
        public IEnumerable<SnapshotProposal>? SnapshotProposals { get; }

        /// <inheritdoc />
        public IEnumerable<SnapshotVote>? SnapshotVotes { get; }

        /// <inheritdoc />
        public IEnumerable<GreysafeReport>? GreysafeReports => _greysafeReports?.Any() == true ? _greysafeReports : null;

        /// <inheritdoc />
        public IEnumerable<ChainanalysisReport>? ChainanalysisReports =>
            _chainanalysisReports?.Any() == true ? _chainanalysisReports : null;

        /// <inheritdoc />
        public CyberConnectProfileData? CyberConnectProfile { get; }

        /// <inheritdoc />
        public IEnumerable<CyberConnectLikeData>? CyberConnectLikes => _cyberConnectLikes?.Any() == true ? _cyberConnectLikes : null;

        /// <inheritdoc />
        public IEnumerable<CyberConnectEssenceData>? CyberConnectEssences => _cyberConnectEssences?.Any() == true ? _cyberConnectEssences : null;

        /// <inheritdoc />
        public IEnumerable<CyberConnectSubscribingProfileData>? CyberConnectSubscribings => _cyberConnectSubscribings?.Any() == true ? _cyberConnectSubscribings : null;

        public ZkSyncStatCalculator(
            string address,
            decimal balance,
            decimal usdBalance,
            IEnumerable<ZkscanAccountNormalTransaction> transactions,
            IEnumerable<ZkscanAccountERC20TokenEvent> erc20TokenTransfers,
            SnapshotData? snapshotData,
            IEnumerable<TokenBalanceData>? tokenBalances,
            IEnumerable<GreysafeReport>? greysafeReports,
            IEnumerable<ChainanalysisReport>? chainanalysisReports,
            CyberConnectData? cyberConnectData)
        {
            _address = address;
            NativeBalance = balance.ToEth();
            NativeBalanceUSD = usdBalance;
            _transactions = transactions;
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

        /// <inheritdoc />
        public ZkSyncWalletStats Stats()
        {
            return (this as IWalletStatsCalculator<ZkSyncWalletStats, ZkSyncTransactionIntervalData>).ApplyCalculators();
        }

        /// <inheritdoc />
        IWalletTransactionStats IWalletTransactionStatsCalculator<ZkSyncWalletStats, ZkSyncTransactionIntervalData>.Stats()
        {
            if (!_transactions.Any())
            {
                return new ZkSyncWalletStats
                {
                    NoData = true
                };
            }

            var intervals = IWalletStatsCalculator
                .GetTransactionsIntervals(_transactions.Select(x => x.TimeStamp!.ToDateTime())).ToList();
            if (intervals.Count == 0)
            {
                return new ZkSyncWalletStats
                {
                    NoData = true
                };
            }

            var monthAgo = DateTime.Now.AddMonths(-1);
            var yearAgo = DateTime.Now.AddYears(-1);

            return new ZkSyncWalletStats
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
    }
}