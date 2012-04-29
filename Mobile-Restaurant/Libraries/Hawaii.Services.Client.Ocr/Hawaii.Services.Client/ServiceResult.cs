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
    using System.Collections.Generic;
    using System.Net;

    /// <summary>
    /// A base class for all Hawaii service result classes. 
    /// Various Hawaii service result classes will represent the result corresponding to different type of Hawaii service calls. 
    /// This class contains functionality common to all Hawaii service result classes.
    /// </summary>
    public abstract class ServiceResult 
    {
        /// <summary>
        /// Initializes a new instance of the ServiceResult class.
        /// </summary>
        public ServiceResult() :
            this(Status.Unspecified, null)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the ServiceResult class.
        /// </summary>
        /// <param name="stateObject">Specifies a user-defined object.</param>
        /// <param name="status">Specifies the status of the service call.</param>
        public ServiceResult(Status status) :
            this(status, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceResult class.
        /// </summary>
        /// <param name="status">Specifies the status of the service call.</param>
        /// <param name="exception">An exception instance used if an error occured during the service call.</param>
        public ServiceResult(Status status, Exception exception)
        {
            this.Status = status;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets or sets the error exception.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the status of the service call.
        /// </summary>
        public Status Status { get; set; }
    }
}
