﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="ITransactionIntervalData.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

namespace Nomis.Utils.Contracts
{
    /// <summary>
    /// Transaction interval data.
    /// </summary>
    public interface ITransactionIntervalData
    {
        /// <summary>
        /// Start date of the interval.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the interval.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Tokens amount sum in the interval.
        /// </summary>
        public BigInteger AmountSum { get; set; }

        /// <summary>
        /// Tokens amount sum in the interval (native token).
        /// </summary>
        public decimal AmountSumValue { get; }

        /// <summary>
        /// Tokens amount sum sent from the wallet in the interval.
        /// </summary>
        public BigInteger AmountOutSum { get; set; }

        /// <summary>
        /// Tokens amount sum sent from the wallet in the interval (native token).
        /// </summary>
        public decimal AmountOutSumValue { get; }

        /// <summary>
        /// Tokens amount sum received to the wallet in the interval.
        /// </summary>
        public BigInteger AmountInSum { get; set; }

        /// <summary>
        /// Tokens amount sum received to the wallet in the interval (native token).
        /// </summary>
        public decimal AmountInSumValue { get; }

        /// <summary>
        /// Transactions count in the interval.
        /// </summary>
        public int Count { get; set; }
    }
}