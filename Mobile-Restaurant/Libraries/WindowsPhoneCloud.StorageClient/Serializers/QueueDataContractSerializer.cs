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
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;

    internal class QueueDataContractSerializer : IDataContractSerializer<ICloudQueue>
    {
        private readonly DataContractSerializer serializer;
        private readonly IStorageCredentials storageCredentials;

        public QueueDataContractSerializer(IStorageCredentials credentials)
        {
            this.serializer = new DataContractSerializer(typeof(StorageAccountQueueListResponse));
            this.storageCredentials = credentials;
        }

        public IEnumerable<ICloudQueue> ReadObject(Stream stream)
        {
            var results = this.serializer.ReadObject(stream) as StorageAccountQueueListResponse;
            foreach (var queue in results.Queues)
            {
                queue.StorageCredentials = this.storageCredentials;
            }

            return results.Queues.Cast<ICloudQueue>();
        }
    }
}
