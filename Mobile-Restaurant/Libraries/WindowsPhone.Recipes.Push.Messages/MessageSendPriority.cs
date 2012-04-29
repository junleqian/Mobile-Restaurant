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

namespace WindowsPhone.Recipes.Push.Messages
{
    /// <summary>
    /// Represents the priorities of which the Push Notification Service sends the message.
    /// </summary>
    public enum MessageSendPriority
    {
        /// <value>The message should be delivered by the Push Notification Service immediately.</value>
        High = 0,

        /// <value>The message should be delivered by the Push Notification Service within 450 seconds.</value>
        Normal = 1,

        /// <value>The message should be delivered by the Push Notification Service within 900 seconds.</value>
        Low = 2
    }
}
