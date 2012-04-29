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
    using System.Net;

    internal static class HttpUtility
    {
        public static WebHeaderCollection ParseQueryString(string queryString)
        {
            var res = new WebHeaderCollection();
            int num = (queryString != null) ? queryString.Length : 0;
            for (int i = 0; i < num; i++)
            {
                int startIndex = i;
                int num4 = -1;
                while (i < num)
                {
                    char ch = queryString[i];
                    if (ch == '=')
                    {
                        if (num4 < 0)
                        {
                            num4 = i;
                        }
                    }
                    else if (ch == '&')
                    {
                        break;
                    }

                    i++;
                }

                var str = string.Empty;
                var str2 = string.Empty;
                if (num4 >= 0)
                {
                    str = queryString.Substring(startIndex, num4 - startIndex);
                    str2 = queryString.Substring(num4 + 1, (i - num4) - 1);
                }
                else
                {
                    str2 = queryString.Substring(startIndex, i - startIndex);
                }

                res[str.Replace("?", string.Empty)] = System.Net.HttpUtility.UrlDecode(str2);
            }

            return res;
        }
    }
}
