// ------------------------------------------------------------------------------------------------------
// <copyright file="EthereumFunctionRequest.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

namespace Nomis.Etherscan.Interfaces.Requests
{
    /// <summary>
    /// Smart-contract base method request.
    /// </summary>
    public abstract class EthereumFunctionRequest
    {
        /// <summary>
        /// Initialize <see cref="EthereumFunctionRequest"/>.
        /// </summary>
        /// <param name="abi">The smart-contract ABI.</param>
        /// <param name="contractAddress">The smart-contract address.</param>
        /// <param name="functionName">The smart-contract called method name.</param>
        /// <param name="parameters">Array of the smart-contract method's parameters.</param>
        protected EthereumFunctionRequest(
            string abi,
            string contractAddress,
            string functionName,
            params object[] parameters)
        {
            Abi = abi;
            ContractAddress = contractAddress;
            FunctionName = functionName;
            Parameters = parameters;
        }

        /// <summary>
        /// The smart-contract ABI.
        /// </summary>
        /// <example>
        /// [{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"address_\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"enum HapiProxy.Category\",\"name\":\"category_\",\"type\":\"uint8\"},{\"indexed\":false,\"internalType\":\"uint8\",\"name\":\"risk_\",\"type\":\"uint8\"}],\"name\":\"CreateAddress\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"reporterAddress_\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint8\",\"name\":\"permissionLevel_\",\"type\":\"uint8\"}],\"name\":\"CreateReporter\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint8\",\"name\":\"version\",\"type\":\"uint8\"}],\"name\":\"Initialized\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"address_\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"enum HapiProxy.Category\",\"name\":\"category_\",\"type\":\"uint8\"},{\"indexed\":false,\"internalType\":\"uint8\",\"name\":\"risk_\",\"type\":\"uint8\"}],\"name\":\"UpdateAddress\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"reporterAddress_\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint8\",\"name\":\"permissionLevel_\",\"type\":\"uint8\"}],\"name\":\"UpdateReporter\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"address_\",\"type\":\"address\"},{\"internalType\":\"enum HapiProxy.Category\",\"name\":\"category_\",\"type\":\"uint8\"},{\"internalType\":\"uint8\",\"name\":\"risk_\",\"type\":\"uint8\"}],\"name\":\"createAddress\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"success\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address[]\",\"name\":\"addresses_\",\"type\":\"address[]\"},{\"internalType\":\"enum HapiProxy.Category\",\"name\":\"category_\",\"type\":\"uint8\"},{\"internalType\":\"uint8\",\"name\":\"risk_\",\"type\":\"uint8\"}],\"name\":\"createAddresses\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"success\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"reporterAddress_\",\"type\":\"address\"},{\"internalType\":\"uint8\",\"name\":\"permissionLevel_\",\"type\":\"uint8\"}],\"name\":\"createReporter\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"success\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"address_\",\"type\":\"address\"}],\"name\":\"getAddress\",\"outputs\":[{\"internalType\":\"enum HapiProxy.Category\",\"name\":\"category\",\"type\":\"uint8\"},{\"internalType\":\"uint8\",\"name\":\"risk\",\"type\":\"uint8\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"reporterAddress_\",\"type\":\"address\"}],\"name\":\"getReporter\",\"outputs\":[{\"internalType\":\"uint8\",\"name\":\"permissionLevel\",\"type\":\"uint8\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"initialize\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"address_\",\"type\":\"address\"},{\"internalType\":\"enum HapiProxy.Category\",\"name\":\"category_\",\"type\":\"uint8\"},{\"internalType\":\"uint8\",\"name\":\"risk_\",\"type\":\"uint8\"}],\"name\":\"updateAddress\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"success\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"reporterAddress_\",\"type\":\"address\"},{\"internalType\":\"uint8\",\"name\":\"permissionLevel_\",\"type\":\"uint8\"}],\"name\":\"updateReporter\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"success\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]
        /// </example>
        public string Abi { get; set; }

        /// <summary>
        /// The smart-contract address.
        /// </summary>
        /// <example>
        /// 0x730c549587b3d6068F78DcED2C0d18Ae6c731B02
        /// </example>
        public string ContractAddress { get; set; }

        /// <summary>
        /// The smart-contract called method name.
        /// </summary>
        /// <example>
        /// getAddress
        /// </example>
        public string FunctionName { get; set; }

        /// <summary>
        /// Array of the smart-contract method's parameters.
        /// </summary>
        /// <example>
        /// null
        /// </example>
        public object[] Parameters { get; set; }
    }
}