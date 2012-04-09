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

    internal class SharedAccessSignatureServiceCloudBlobClientStrategy : ICloudBlobClient
    {
        private readonly ISharedAccessSignatureServiceClient sasService;
        
        public SharedAccessSignatureServiceCloudBlobClientStrategy(ISharedAccessSignatureServiceClient sasService)
        {
            if (sasService == null)
            {
                throw new ArgumentNullException("sasService", "The Shared Access Signature service client cannot be null.");
            }

            this.sasService = sasService;
        }

        public IStorageCredentials Credentials { get; private set; }

        public ICloudBlobContainer GetContainerReference(string containerName)
        {
            return new SharedAccessSignatureServiceCloudBlobContainer(this.sasService)
            {
                Name = containerName,
            };
        }

        public void ListContainers(Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            this.sasService.ListContainers(null, callback);
        }

        public void ListContainers(string prefix, Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            this.sasService.ListContainers(prefix, callback);
        }
    }
}
