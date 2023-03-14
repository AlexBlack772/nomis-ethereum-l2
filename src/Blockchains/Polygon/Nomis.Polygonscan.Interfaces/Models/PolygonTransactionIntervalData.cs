// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonTransactionIntervalData.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;
using System.Text.Json.Serialization;

using Nomis.Polygonscan.Interfaces.Extensions;
using Nomis.Utils.Contracts;

namespace Nomis.Polygonscan.Interfaces.Models
{
    /// <inheritdoc cref="ITransactionIntervalData"/>
    public class PolygonTransactionIntervalData :
        ITransactionIntervalData
    {
        /// <inheritdoc />
        public DateTime StartDate { get; set; }

        /// <inheritdoc />
        public DateTime EndDate { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        public BigInteger AmountSum { get; set; }

        /// <inheritdoc cref="ITransactionIntervalData.AmountSum"/>
        public decimal AmountSumValue => AmountSum.ToMatic();

        /// <inheritdoc />
        [JsonIgnore]
        public BigInteger AmountOutSum { get; set; }

        /// <inheritdoc cref="ITransactionIntervalData.AmountOutSum"/>
        public decimal AmountOutSumValue => AmountOutSum.ToMatic();

        /// <inheritdoc />
        [JsonIgnore]
        public BigInteger AmountInSum { get; set; }

        /// <inheritdoc cref="ITransactionIntervalData.AmountInSum"/>
        public decimal AmountInSumValue => AmountInSum.ToMatic();

        /// <inheritdoc />
        public int Count { get; set; }
    }
}