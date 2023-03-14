// ------------------------------------------------------------------------------------------------------
// <copyright file="IEthereumScoringService.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.Etherscan.Interfaces.Requests;
using Nomis.Utils.Contracts.Services;

namespace Nomis.Etherscan.Interfaces
{
    /// <summary>
    /// Ethereum scoring service.
    /// </summary>
    public interface IEthereumScoringService :
        IBlockchainScoringService,
        IBlockchainDescriptor,
        IInfrastructureService
    {
        /// <summary>
        /// Call read-method from smart-contract.
        /// </summary>
        /// <typeparam name="T">The type of returned value.</typeparam>
        /// <param name="request">Smart-contract read-method request.</param>
        /// <returns>Return the result of the called smart-contract read-method.</returns>
        public Task<T?> CallReadFunctionAsync<T>(
            EthereumCallReadFunctionRequest request);
    }
}