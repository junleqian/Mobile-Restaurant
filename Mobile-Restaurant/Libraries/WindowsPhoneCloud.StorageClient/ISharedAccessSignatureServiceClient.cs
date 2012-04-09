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

    public interface ISharedAccessSignatureServiceClient
    {
        IStorageCredentials Credentials { get; }

        void CreateContainer(string containerName, bool createIfNotExist, bool isPublic, Action<CloudOperationResponse<bool>> callback);

        void DeleteContainer(string containerName, Action<CloudOperationResponse<bool>> callback);

        void GetContainerSharedAccessSignature(string containerName, Action<CloudOperationResponse<Uri>> callback);

        void GetBlobsSharedAccessSignatures(string containerName, string blobPrefix, bool useFlatBlobListing, Action<CloudOperationResponse<IEnumerable<ICloudBlob>>> callback);

        void ListContainers(string containerPrefix, Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback);
    }
}
