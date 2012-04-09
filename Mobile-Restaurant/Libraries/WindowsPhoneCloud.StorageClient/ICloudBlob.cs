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
    using System.IO;
    using System.Net;

    /// <summary>
    /// Represents the a cloud blob.
    /// Modelled after the interface from the Microsoft.WindowsAzure.StorageClient
    /// http://msdn.microsoft.com/en-us/library/microsoft.windowsazure.storageclient.cloudblob_members.aspx
    /// </summary>
    public interface ICloudBlob
    {
        string Name { get; }

        Uri Uri { get; }

        WebHeaderCollection Metadata { get; }

        ICloudBlobContainer Container { get; }

        void Delete(bool includeSnapshots, Action<CloudOperationResponse<bool>> callback);

        void UploadFromStream(Stream source, Action<CloudOperationResponse<bool>> callback);

        void SetMetadata(Action<CloudOperationResponse<bool>> callback);

        void FetchAttributes(Action<CloudOperationResponse<bool>> callback);
    }
}
