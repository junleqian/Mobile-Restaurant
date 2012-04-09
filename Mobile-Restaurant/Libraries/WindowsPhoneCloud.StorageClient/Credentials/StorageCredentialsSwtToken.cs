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
    using System.Globalization;
    using System.Net;
    using Microsoft.Samples.Data.Services.Client;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Exceptions;

    public class StorageCredentialsSwtToken : IStorageCredentials
    {
        private const long ExpirationBuffer = 5;

        private readonly string swtToken;
        private readonly long expiration;

        public StorageCredentialsSwtToken(string authenticationToken, long expiration)
        {
            this.swtToken = authenticationToken;
            this.expiration = expiration;
        }

        public void SignRequestLite(WebRequest webRequest)
        {
            this.ValidateToken();

            if (webRequest == null)
            {
                throw new ArgumentNullException("webRequest");
            }

            webRequest.Headers["Authorization"] = string.Format(CultureInfo.InvariantCulture, "OAuth {0}", this.swtToken);
        }

        public void AddAuthenticationHeadersLite(RequestData requestData, WebHeaderCollection requestHeaders)
        {
            this.ValidateToken();

            if (requestHeaders == null)
            {
                throw new ArgumentNullException("requestHeaders");
            }

            requestHeaders["Authorization"] = string.Format(CultureInfo.InvariantCulture, "OAuth {0}", this.swtToken);
        }

        public void SignRequest(WebRequest webRequest, long contentLength)
        {
            this.ValidateToken();

            if (webRequest == null)
            {
                throw new ArgumentNullException("webRequest");
            }

            webRequest.Headers["Authorization"] = string.Format(CultureInfo.InvariantCulture, "OAuth {0}", this.swtToken);
        }

        private void ValidateToken()
        {
            var epoch = (DateTime.UtcNow.Ticks - 621355968000000000) / 10000000;
            if (this.expiration <= epoch + ExpirationBuffer)
            {
                throw new SecurityTokenExpirationException("Your security token has expired. Please, log out the application and log in again.");
            }
        }
    }
}
