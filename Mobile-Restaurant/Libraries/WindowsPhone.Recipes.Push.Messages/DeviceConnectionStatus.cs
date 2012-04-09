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
    /// Windows Phone Device connection status.
    /// </summary>
    public enum DeviceConnectionStatus
    {
        /// <value>The request is not applicable.</value>
        NotApplicable,

        /// <value>The device is connected.</value>
        Connected,

        /// <value>The device is temporarily disconnected.</value>
        TempDisconnected,

        /// <value>The device is in an inactive state.</value>
        Inactive
    }
}
