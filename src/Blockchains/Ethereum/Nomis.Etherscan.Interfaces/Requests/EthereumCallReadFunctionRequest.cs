// ------------------------------------------------------------------------------------------------------
// <copyright file="EthereumCallReadFunctionRequest.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

namespace Nomis.Etherscan.Interfaces.Requests
{
    /// <summary>
    /// Smart-contract read-method request.
    /// </summary>
    public class EthereumCallReadFunctionRequest :
        EthereumFunctionRequest
    {
        /// <summary>
        /// Initialize <see cref="EthereumCallReadFunctionRequest"/>.
        /// </summary>
        /// <param name="abi">The smart-contract ABI.</param>
        /// <param name="contractAddress">The smart-contract address.</param>
        /// <param name="functionName">The smart-contract called read-method name.</param>
        /// <param name="parameters">Array of the smart-contract read-method's parameters.</param>
        public EthereumCallReadFunctionRequest(
            string abi,
            string contractAddress,
            string functionName,
            params object[] parameters)
            : base(abi, contractAddress, functionName, parameters)
        {
        }
    }
}