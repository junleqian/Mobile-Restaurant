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
    using System.Net;
    using System.Runtime.Serialization;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;

    public class StorageAccountCloudBlob : CloudBlobBase
    {
        // Used only for deserialization purposes.
        public StorageAccountCloudBlob()
        {
        }

        public StorageAccountCloudBlob(ICloudBlobContainer container, IStorageCredentials credentials)
            : base(container)
        {
            this.Credentials = credentials;
        }

        protected override UriBuilder CreateUriBuilder()
        {
            if (this.Uri != null)
            {
                return new UriBuilder(this.Uri);
            }
            else
            {
                throw new ArgumentNullException("Uri");
            }
        }

        protected override void SignRequest(HttpWebRequest request, long contentLength)
        {
            this.Credentials.SignRequest(request, 0);
        }
    }
}