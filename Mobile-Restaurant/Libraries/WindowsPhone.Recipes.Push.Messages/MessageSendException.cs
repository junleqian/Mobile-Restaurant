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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using WindowsPhone.Recipes.Push.Messages.Properties;

namespace WindowsPhone.Recipes.Push.Messages
{
    /// <summary>
    /// Represents errors that occur during push notification message send operation.
    /// </summary>
    public class MessageSendException : Exception
    {
        /// <summary>
        /// Gets the message send result.
        /// </summary>
        public MessageSendResult Result { get; private set; }

        /// <summary>
        /// Initializes a new instance of this type.
        /// </summary>
        /// <param name="result">The send operation result.</param>
        /// <param name="innerException">An inner exception causes this error.</param>
        internal MessageSendException(MessageSendResult result, Exception innerException)
            : base(Resources.FailedToSendMessage, innerException)
        {
            Result = result;
        }
    }
}
