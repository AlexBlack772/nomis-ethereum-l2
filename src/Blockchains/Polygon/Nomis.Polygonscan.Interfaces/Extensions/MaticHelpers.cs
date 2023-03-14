// ------------------------------------------------------------------------------------------------------
// <copyright file="MaticHelpers.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Numerics;

using Nomis.Polygonscan.Interfaces.Models;

namespace Nomis.Polygonscan.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods for polygon.
    /// </summary>
    public static class MaticHelpers
    {
        /// <summary>
        /// Convert Wei value to Matic.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Matic.</returns>
        public static decimal ToMatic(this string valueInWei)
        {
            return BigInteger
                .TryParse(valueInWei, NumberStyles.AllowDecimalPoint, new NumberFormatInfo { CurrencyDecimalSeparator = "." }, out var value)
                ? value.ToMatic()
                : 0;
        }

        /// <summary>
        /// Convert Wei value to Matic.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Matic.</returns>
        public static decimal ToMatic(this in BigInteger valueInWei)
        {
            if (valueInWei > new BigInteger(decimal.MaxValue))
            {
                return (decimal)(valueInWei / new BigInteger(100_000_000_000_000_000));
            }

            return (decimal)valueInWei * 0.000_000_000_000_000_001M;
        }

        /// <summary>
        /// Convert Wei value to Matic.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total Matic.</returns>
        public static decimal ToMatic(this decimal valueInWei)
        {
            return new BigInteger(valueInWei).ToMatic();
        }

        /// <summary>
        /// Convert Wei value to token value.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <param name="multiplier">Value multiplier.</param>
        /// <returns>Returns total token value.</returns>
        public static decimal ToTokenValue(this decimal valueInWei, decimal multiplier)
        {
            return valueInWei * multiplier;
        }

        /// <summary>
        /// Get token UID based on it ContractAddress and Id.
        /// </summary>
        /// <param name="token">Token info.</param>
        /// <returns>Returns token UID.</returns>
        public static string GetTokenUid(this IPolygonscanAccountNftTokenEvent token)
        {
            return token.ContractAddress + "_" + token.TokenId;
        }
    }
}