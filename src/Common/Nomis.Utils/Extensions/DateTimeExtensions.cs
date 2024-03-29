﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace Nomis.Utils.Extensions
{
    /// <summary>
    /// Extension methods for converting DateTime.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Convert Unix TimeStamp to DateTime.
        /// </summary>
        /// <param name="unixTimeStamp">Unix TimeStamp in string.</param>
        /// <returns><see cref="DateTime"/>.</returns>
        public static DateTime ToDateTime(this string unixTimeStamp)
        {
            long unixTimeStampLong = long.Parse(unixTimeStamp, NumberStyles.Any, new DateTimeFormatInfo());
            var dateTimeOffSet = DateTimeOffset.FromUnixTimeSeconds(unixTimeStampLong);
            return dateTimeOffSet.DateTime;
        }
    }
}