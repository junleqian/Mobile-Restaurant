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

// Copyright 2010 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.

namespace Microsoft.Samples.Data.Services.Http
{
    #region Namespaces.

    using System;
    using Microsoft.Samples.Data.Services.Client;
    using System.Diagnostics;
    using System.IO;

    #endregion Namespaces.

    internal abstract class WebRequest
    {
        public abstract string ContentType 
        { 
            get; 
            set; 
        }

        public abstract Microsoft.Samples.Data.Services.Http.WebHeaderCollection Headers
        {
            get;
        }

        public abstract string Method 
        { 
            get; 
            set; 
        }

        public abstract Uri RequestUri 
        { 
            get; 
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose the returned value")]
        public static Microsoft.Samples.Data.Services.Http.WebRequest Create(Uri requestUri, HttpStack httpStack)
        {
            Debug.Assert(requestUri != null, "requestUri != null");
            if ((requestUri.Scheme != Uri.UriSchemeHttp) && (requestUri.Scheme != Uri.UriSchemeHttps))
            {
                throw new NotSupportedException();
            }

            return new ClientHttpWebRequest(requestUri);
        }

        public abstract void Abort();

        public abstract IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state);

        public abstract IAsyncResult BeginGetResponse(AsyncCallback callback, object state);

        public abstract Stream EndGetRequestStream(IAsyncResult asyncResult);

        public abstract Microsoft.Samples.Data.Services.Http.WebResponse EndGetResponse(IAsyncResult asyncResult);

        private static bool UriRequiresClientHttpWebRequest(Uri uri)
        {
            return true;
        }
    }
}
