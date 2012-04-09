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

    /// <summary>
    /// Represents the a client for accessing cloud blobs.
    /// Modelled after the interface from the Microsoft.WindowsAzure.StorageClient
    /// http://msdn.microsoft.com/en-us/library/microsoft.windowsazure.storageclient.cloudblobclient_members.aspx
    /// </summary>
    public interface ICloudBlobClient
    {
        IStorageCredentials Credentials { get; }

        ICloudBlobContainer GetContainerReference(string containerAddress);

        void ListContainers(Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback);

        void ListContainers(string prefix, Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback);
    }
}
