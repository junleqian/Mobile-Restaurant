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

namespace Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    internal static class WebHeaderCollectionExtensions
    {
        public static void Add(this WebHeaderCollection col, string key, string value)
        {
            col[key] = value;
        }

        public static void Add(this WebHeaderCollection col, HttpRequestHeader key, string value)
        {
            col[key] = value;
        }

        public static List<string> GetValues(this WebHeaderCollection col, string key)
        {
            var list = new List<string>();
            foreach (var k in col.AllKeys)
            {
                if (k.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(col[k]);
                }
            }

            return list;
        }
    }
}
