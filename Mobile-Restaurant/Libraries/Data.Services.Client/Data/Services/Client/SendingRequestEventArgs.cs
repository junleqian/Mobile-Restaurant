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

namespace Microsoft.Samples.Data.Services.Client
{
    using System;
    using System.Diagnostics;

    public class SendingRequestEventArgs : EventArgs
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Not used in Silverlight")]
        private RequestData requestData;
        private System.Net.WebHeaderCollection requestHeaders;

        internal SendingRequestEventArgs(RequestData requestData, System.Net.WebHeaderCollection requestHeaders)
        {
            Debug.Assert(null != requestData, "null requestData");
            Debug.Assert(null != requestHeaders, "null requestHeaders");
            this.requestData = requestData;
            this.requestHeaders = requestHeaders;
        }

        public RequestData RequestData
        {
            get { return this.requestData; }
        }

        public System.Net.WebHeaderCollection RequestHeaders
        {
            get { return this.requestHeaders; }
        }
    }

    [CLSCompliant(true)]
    public class RequestData
    {
        public string Method { get; set; }

        public Uri RequestUri { get; set; }
    }
}
