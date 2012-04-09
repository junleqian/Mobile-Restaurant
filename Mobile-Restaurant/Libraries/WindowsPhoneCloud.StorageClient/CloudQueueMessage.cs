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
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract(Name = "QueueMessage", Namespace = "")]
    public class CloudQueueMessage
    {
        [DataMember(Name = "MessageText")]
        public byte[] AsBytes
        {
            get; set;
        }

        public string AsString
        {
            get
            {
                return Encoding.UTF8.GetString(this.AsBytes, 0, this.AsBytes.Length);
            }
        }

        [DataMember(IsRequired = false)]
        public int DequeueCount { get; set; }

        [DataMember(Name = "MessageId")]
        public string Id { get; set; }

        [DataMember(IsRequired = false)]
        public string PopReceipt { get; set; }
    }
}
