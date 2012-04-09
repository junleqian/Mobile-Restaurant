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

namespace Microsoft.Samples.Hawaii.Services.Client
{
    using System;

    /// <summary>
    /// Specifies the HTTP method that is used when communicating with the server.
    /// </summary>
    public enum HttpMethod
    {
        /// <summary>
        /// Represents the Http Get method.
        /// </summary>
        Get,
 
        /// <summary>
        /// Represents the Http Post method.
        /// </summary>
        Post,

        /// <summary>
        /// Represents the Http Put method.
        /// </summary>
        Put, 

        /// <summary>
        /// Represents the Http Delete method.
        /// </summary>
        Delete 
    }
}
