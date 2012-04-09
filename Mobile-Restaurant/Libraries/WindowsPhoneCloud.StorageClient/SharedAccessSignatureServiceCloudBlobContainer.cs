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
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Browser;
    using System.Runtime.Serialization;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;

    [DataContract(Name = "Container", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Samples.WindowsPhoneCloud.StorageClient")]
    public class SharedAccessSignatureServiceCloudBlobContainer : ICloudBlobContainer
    {
        private ISharedAccessSignatureServiceClient sasService;

        // Used only for deserialization purposes.
        public SharedAccessSignatureServiceCloudBlobContainer()
        {
        }

        public SharedAccessSignatureServiceCloudBlobContainer(ISharedAccessSignatureServiceClient sasService)
        {
            if (sasService == null)
            {
                throw new ArgumentNullException("sasService", "The Shared Access Signature service client cannot be null.");
            }

            this.sasService = sasService;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember(Name = "Url")]
        public Uri Uri { get; set; }

        public void Create(bool createPublicContainer, Action<CloudOperationResponse<bool>> callback)
        {
            this.sasService.CreateContainer(this.Name, false, createPublicContainer, callback);
        }

        public void CreateIfNotExist(bool createPublicContainer, Action<CloudOperationResponse<bool>> callback)
        {
            this.sasService.CreateContainer(this.Name, true, createPublicContainer, callback);
        }

        public void Delete(Action<CloudOperationResponse<bool>> callback)
        {
            this.sasService.DeleteContainer(this.Name, callback);
        }

        public ICloudBlob GetBlobReference(string blobAddressUri)
        {
            return new SharedAccessSignatureServiceCloudBlob(this, this.sasService)
            {
                Name = blobAddressUri,
                Uri = new Uri(string.Concat(this.Uri, "/", blobAddressUri)),
            };
        }

        public void ListBlobs(Action<CloudOperationResponse<System.Collections.Generic.IEnumerable<ICloudBlob>>> callback)
        {
            this.sasService.GetBlobsSharedAccessSignatures(this.Name, null, false, callback);
        }

        public void ListBlobs(string blobPrefix, bool useFlatBlobListing, Action<CloudOperationResponse<System.Collections.Generic.IEnumerable<ICloudBlob>>> callback)
        {
            this.sasService.GetBlobsSharedAccessSignatures(this.Name, blobPrefix, useFlatBlobListing, callback);
        }
    }
}
