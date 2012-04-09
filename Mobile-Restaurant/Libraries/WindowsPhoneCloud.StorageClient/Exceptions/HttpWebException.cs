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

namespace Microsoft.Samples.WindowsPhoneCloud.StorageClient.Exceptions
{
    using System;
    using System.Net;

    public class HttpWebException : Exception
    {
        public HttpWebException()
        {
        }
        
        public HttpWebException(string message)
            : base(message)
        {
        }

        public HttpWebException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public HttpWebException(string message, HttpStatusCode statusCode)
            : this(message, statusCode, null)
        {            
        }

        public HttpWebException(string message, HttpStatusCode statusCode, Exception innerException)
            : base(message, innerException)
        {
            this.StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}
