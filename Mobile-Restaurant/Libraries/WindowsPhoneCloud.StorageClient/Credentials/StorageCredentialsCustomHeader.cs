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

namespace Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials
{
    using System;
    using System.Net;
    using Microsoft.Samples.Data.Services.Client;

    public class StorageCredentialsCustomHeader : IStorageCredentials
    {
        private readonly string headerName;       
        private readonly string headerValue;

        public StorageCredentialsCustomHeader(string headerName, string headerValue)
        {
            if (string.IsNullOrEmpty(headerName))
            {
                throw new ArgumentNullException("headerName");
            }

            if (string.IsNullOrEmpty(headerValue))
            {
                throw new ArgumentNullException("headerValue");
            }

            this.headerName = headerName;
            this.headerValue = headerValue;
        }

        public void SignRequestLite(WebRequest webRequest)
        {
            if (webRequest == null)
            {
                throw new ArgumentNullException("webRequest");
            }

            webRequest.Headers[this.headerName] = this.headerValue;
        }

        public void AddAuthenticationHeadersLite(RequestData requestData, WebHeaderCollection requestHeaders)
        {
            if (requestHeaders == null)
            {
                throw new ArgumentNullException("requestHeaders");
            }

            requestHeaders[this.headerName] = this.headerValue;
        }

        public void SignRequest(WebRequest webRequest, long contentLength)
        {
            if (webRequest == null)
            {
                throw new ArgumentNullException("webRequest");
            }

            webRequest.Headers[this.headerName] = this.headerValue;
        }
    }
}
