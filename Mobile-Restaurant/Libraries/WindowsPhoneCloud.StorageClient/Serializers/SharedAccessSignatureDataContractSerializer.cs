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

    internal class SharedAccessSignatureDataContractSerializer : IDataContractSerializer<ICloudBlob>
    {
        private DataContractSerializer cloudBlobCollectionSerializer;
        private DataContractSerializer cloudBlobContainerListResponseSerializer;

        public IEnumerable<ICloudBlob> ReadObject(Stream stream)
        {
            if (this.cloudBlobCollectionSerializer == null)
            {
                this.cloudBlobCollectionSerializer = new DataContractSerializer(typeof(SharedAccessSignatureServiceCloudBlobListResponse));
            }

            var response = this.cloudBlobCollectionSerializer.ReadObject(stream) as SharedAccessSignatureServiceCloudBlobListResponse;
            return response.Blobs.ToArray();
        }

        public IEnumerable<ICloudBlobContainer> ReadContainerObjects(Stream stream)
        {
            if (this.cloudBlobContainerListResponseSerializer == null)
            {
                this.cloudBlobContainerListResponseSerializer = new DataContractSerializer(typeof(SharedAccessSignatureServiceCloudBlobContainerListResponse));
            }

            var response = this.cloudBlobContainerListResponseSerializer.ReadObject(stream) as SharedAccessSignatureServiceCloudBlobContainerListResponse;
            return response.Containers.ToArray();
        }
    }
}
