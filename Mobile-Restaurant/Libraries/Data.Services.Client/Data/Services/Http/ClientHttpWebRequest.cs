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
    using System.Diagnostics;
    using System.IO;

    #endregion Namespaces.

    internal sealed class ClientHttpWebRequest : Microsoft.Samples.Data.Services.Http.HttpWebRequest
    {
        private readonly System.Net.HttpWebRequest innerRequest;

        private ClientWebHeaderCollection headerCollection;

        public ClientHttpWebRequest(Uri requestUri)
        {
            Debug.Assert(requestUri != null, "requestUri can't be null.");
            this.innerRequest = (System.Net.HttpWebRequest)System.Net.Browser.WebRequestCreator.ClientHttp.Create(requestUri);
            Debug.Assert(this.innerRequest != null, "ClientHttp.Create failed to create a new request without throwing exception.");
        }

        public override string Accept
        {
            get
            {
                return this.innerRequest.Accept;
            }

            set
            {
                this.innerRequest.Accept = value;
            }
        }

        public override long ContentLength
        {
            set
            {
                return;
            }
        }

        public override bool AllowReadStreamBuffering
        {
            get
            {
                return this.innerRequest.AllowReadStreamBuffering;
            }

            set
            {
                this.innerRequest.AllowReadStreamBuffering = value;
            }
        }

        public override string ContentType
        {
            get
            {
                return this.innerRequest.ContentType;
            }

            set
            {
                this.innerRequest.ContentType = value;
            }
        }

        public override Microsoft.Samples.Data.Services.Http.WebHeaderCollection Headers
        {
            get
            {
                if (this.headerCollection == null)
                {
                    this.headerCollection = new ClientWebHeaderCollection(this.innerRequest.Headers, this.innerRequest);
                }

                return this.headerCollection;
            }
        }

        public override string Method
        {
            get
            {
                return this.innerRequest.Method;
            }

            set
            {
                this.innerRequest.Method = value;
            }
        }

        public override Uri RequestUri
        {
            get
            {
                return this.innerRequest.RequestUri;
            }
        }

        public override void Abort()
        {
            this.innerRequest.Abort();
        }

        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            return this.innerRequest.BeginGetRequestStream(callback, state);
        }

        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            return this.innerRequest.BeginGetResponse(callback, state);
        }

        public override Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            return this.innerRequest.EndGetRequestStream(asyncResult);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose the returned value")]
        public override Microsoft.Samples.Data.Services.Http.WebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            try
            {
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)this.innerRequest.EndGetResponse(asyncResult);
                return new ClientHttpWebResponse(response, this);
            }
            catch (System.Net.WebException exception)
            {
                ClientHttpWebResponse response = new ClientHttpWebResponse((System.Net.HttpWebResponse)exception.Response, this);
                throw new Microsoft.Samples.Data.Services.Http.WebException(exception.Message, exception, response);
            }
        }

        public override System.Net.WebHeaderCollection CreateEmptyWebHeaderCollection()
        {
            return new System.Net.WebHeaderCollection();
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
