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
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Samples.Data.Services.Client;

    internal sealed class ClientWebHeaderCollection : WebHeaderCollection
    {
        private System.Net.WebHeaderCollection innerCollection;

        private System.Net.HttpWebRequest request;

        internal ClientWebHeaderCollection(System.Net.WebHeaderCollection collection)
        {
            Debug.Assert(collection != null, "collection can't be null.");
            this.innerCollection = collection;
        }

        internal ClientWebHeaderCollection(System.Net.WebHeaderCollection collection, System.Net.HttpWebRequest request)
        {
            Debug.Assert(collection != null, "collection can't be null.");
            this.innerCollection = collection;
            this.request = request;
        }

        #region Properties.

        public override int Count
        {
            get
            {
                return this.innerCollection.Count;
            }
        }

        public override ICollection<string> AllKeys
        {
            get
            {
                return this.innerCollection.AllKeys;
            }
        }

        public override string this[string name]
        {
            get
            {
                return this.innerCollection[name];
            }

            set
            {
                if (name == XmlConstants.HttpContentLength)
                {
                    return;
                }
                else if (name == XmlConstants.HttpAcceptCharset)
                {
                    Debug.Assert(value == XmlConstants.Utf8Encoding, "Asking for AcceptCharset different thatn UTF-8.");
                    return;
                }
                else if (name == XmlConstants.HttpCookie)
                {
                    if (this.request != null)
                    {
                        System.Net.CookieContainer cookieContainer = new System.Net.CookieContainer();
                        cookieContainer.SetCookies(this.request.RequestUri, value);
                        this.request.CookieContainer = cookieContainer;
                    }
                    else
                    {
                        this.innerCollection[name] = value;
                    }
                }
                else
                {
                    this.innerCollection[name] = value;
                }
            }
        }

        public override string this[Microsoft.Samples.Data.Services.Http.HttpRequestHeader header]
        {
            get
            {
                return this[HttpHeaderToName.RequestHeaderNames[header]];
            }

            set
            {
                this[HttpHeaderToName.RequestHeaderNames[header]] = value;
            }
        }

        #endregion Properties.
    }
}
