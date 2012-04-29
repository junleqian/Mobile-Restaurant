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
    /// Microsoft Push Notification Service notification request status.
    /// </summary>
    public enum NotificationStatus
    {
        /// <value>The request is not applicable.</value>
        NotApplicable,

        /// <value>The notification request was accepted.</value>
        Received,

        /// <value>Queue overflow. The Push Notification Service should re-send the notification later.</value>
        QueueFull,

        /// <value>The push notification was suppressed by the Push Notification Service.</value>
        Suppressed,

        /// <value>The push notification was dropped by the Push Notification Service.</value>
        Dropped,
    }
}
