// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

namespace Microsoft.Samples.WindowsPhoneCloud.StorageClient
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;

    public class CloudBlobClient : ICloudBlobClient
    {
        private ICloudBlobClient strategy;

        /// <summary>
        /// Initializes a new instance of the CloudBlobClient class to perform operations with Blobs and Containers using Shared Access Signatures.
        /// </summary>
        /// <param name="sasService">A valid Shared Access Signature service client instance.</param>
        /// <param name="credentials"></param>
        public CloudBlobClient(ISharedAccessSignatureServiceClient sasService)
        {
            if (sasService == null)
            {
                throw new ArgumentNullException("sasService", "The Shared Access Signature service client cannot be null.");
            }

            this.strategy = new SharedAccessSignatureServiceCloudBlobClientStrategy(sasService);
        }

        /// <summary>
        /// Initializes a new instance of the CloudBlobClient class to perform operations with Blobs and Containers using the Azure Storage credentials.
        /// </summary>
        /// <param name="containerBaseAddress">The Blob service endpoint to use to create the client.</param>
        /// <param name="credentials">The account credentials.</param>
        public CloudBlobClient(string storageAccountEndpoint, IStorageCredentials credentials = null)
        {
            this.Credentials = credentials ?? new StorageCredentialsAnonymous();
            this.strategy = new StorageAccountCloudBlobClientStrategy(storageAccountEndpoint, this.Credentials);
        }

        public IStorageCredentials Credentials { get; private set; }

        public ICloudBlobContainer GetContainerReference(string containerName)
        {
            return this.strategy.GetContainerReference(containerName);
        }

        public void ListContainers(Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            this.strategy.ListContainers(callback);
        }

        public void ListContainers(string prefix, Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            this.strategy.ListContainers(prefix, callback);
        }
    }
}