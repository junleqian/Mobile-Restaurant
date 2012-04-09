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
    using System.Runtime.Serialization;

    internal class QueueMessageDataContractSerializer : IDataContractSerializer<CloudQueueMessage>
    {
        private readonly DataContractSerializer serializer;

        public QueueMessageDataContractSerializer()
        {
            this.serializer = new DataContractSerializer(typeof(IEnumerable<CloudQueueMessage>), "QueueMessagesList", string.Empty);
        }

        public IEnumerable<CloudQueueMessage> ReadObject(Stream stream)
        {
            return this.serializer.ReadObject(stream) as IEnumerable<CloudQueueMessage>;
        }
    }
}
