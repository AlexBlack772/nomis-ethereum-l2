﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="ScoringDataUpdatedEvent.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Domain.Abstractions;
using Nomis.Domain.Scoring.Entities;
using Nomis.Utils.Contracts.Common;

namespace Nomis.Domain.Scoring.Events
{
    /// <summary>
    /// Update scoring data domain event.
    /// </summary>
    public class ScoringDataUpdatedEvent :
        DomainEvent<ScoringData>
    {
        /// <summary>
        /// Initialize <see cref="ScoringDataUpdatedEvent"/>.
        /// </summary>
        public ScoringDataUpdatedEvent()
            : base(Guid.Empty, string.Empty, null)
        {
            RequestAddress = string.Empty;
            Blockchain = 0;
            StatData = string.Empty;
        }

        /// <summary>
        /// Initialize <see cref="ScoringDataUpdatedEvent"/>.
        /// </summary>
        /// <param name="scoringData">Scoring data.</param>
        /// <param name="eventDescription">Event description.</param>
        public ScoringDataUpdatedEvent(
            ScoringData scoringData,
            string eventDescription)
            : base(
                scoringData.Id,
                eventDescription,
                scoringData.Version)
        {
            Id = scoringData.Id;
            RequestAddress = scoringData.RequestAddress;
            ResolvedAddress = scoringData.ResolvedAddress;
            Blockchain = scoringData.Blockchain;
            Score = scoringData.Score;
            StatData = scoringData.StatData;
        }

        /// <inheritdoc cref="IEntity{TEntityId}.Id"/>
        [JsonInclude]
        public Guid Id { get; private set; }

        /// <inheritdoc cref="ScoringData.RequestAddress"/>
        public string RequestAddress { get; private set; }

        /// <inheritdoc cref="ScoringData.ResolvedAddress"/>
        public string? ResolvedAddress { get; private set; }

        /// <inheritdoc cref="ScoringData.Blockchain"/>
        public ulong Blockchain { get; private set; }

        /// <inheritdoc cref="ScoringData.Score"/>
        public double Score { get; private set; }

        /// <inheritdoc cref="ScoringData.StatData"/>
        public string StatData { get; private set; }
    }
}