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

namespace Microsoft.Samples.WindowsPhoneCloud.StorageClient.Serializers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;

    internal class StorageAccountBlobDataContractSerializer : IDataContractSerializer<ICloudBlob>
    {
        private DataContractSerializer cloudBlobListResponseSerializer;
        private DataContractSerializer cloudBlobContainerListResponseSerializer;

        public IEnumerable<ICloudBlob> ReadObject(Stream stream)
        {
            if (this.cloudBlobListResponseSerializer == null)
            {
                this.cloudBlobListResponseSerializer = new DataContractSerializer(typeof(StorageAccountCloudBlobListResponse));
            }

            var response = this.cloudBlobListResponseSerializer.ReadObject(stream) as StorageAccountCloudBlobListResponse;
            return response.Blobs
                .Select(b => new StorageAccountCloudBlob { Name = b.Name, Uri = b.Url })
                .ToArray();
        }

        public IEnumerable<ICloudBlobContainer> ReadContainerObjects(Stream stream)
        {
            if (this.cloudBlobContainerListResponseSerializer == null)
            {
                this.cloudBlobContainerListResponseSerializer = new DataContractSerializer(typeof(StorageAccountCloudBlobContainerListResponse));
            }

            var response = this.cloudBlobContainerListResponseSerializer.ReadObject(stream) as StorageAccountCloudBlobContainerListResponse;
            return response.Containers.ToArray();
        }
    }
}
