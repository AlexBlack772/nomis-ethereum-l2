// ------------------------------------------------------------------------------------------------------
// <copyright file="OptimismHelpers.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Numerics;

using Nomis.Optimism.Interfaces.Models;

namespace Nomis.Optimism.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods for Optimism.
    /// </summary>
    public static class OptimismHelpers
    {
        /// <summary>
        /// Convert Wei value to ETH.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total ETH.</returns>
        public static decimal ToEth(this string valueInWei)
        {
            return BigInteger
                .TryParse(valueInWei, NumberStyles.AllowDecimalPoint, new NumberFormatInfo { CurrencyDecimalSeparator = "." }, out var value)
                ? value.ToEth()
                : 0;
        }

        /// <summary>
        /// Convert Wei value to ETH.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total ETH.</returns>
        public static decimal ToEth(this in BigInteger valueInWei)
        {
            if (valueInWei > new BigInteger(decimal.MaxValue))
            {
                return (decimal)(valueInWei / new BigInteger(100_000_000_000_000_000));
            }

            return (decimal)valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to ETH.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total ETH.</returns>
        public static decimal ToEth(this decimal valueInWei)
        {
            return new BigInteger(valueInWei).ToEth();
        }

        /// <summary>
        /// Get token UID based on it ContractAddress and Id.
        /// </summary>
        /// <param name="token">Token info.</param>
        /// <returns>Returns token UID.</returns>
        public static string GetTokenUid(this IOptimismAccountNftTokenEvent token)
        {
            return token.ContractAddress + "_" + token.TokenId;
        }
    }
}