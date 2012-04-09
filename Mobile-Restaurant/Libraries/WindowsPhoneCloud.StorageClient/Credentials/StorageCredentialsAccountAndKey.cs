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
    using System.Globalization;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.Samples.Data.Services.Client;
    
    public class StorageCredentialsAccountAndKey : IStorageCredentials
    {
        private string accountName;
        private byte[] accountKey;

        public StorageCredentialsAccountAndKey(string accountName, string accountKey)
        {
            if (string.IsNullOrWhiteSpace(accountName))
            {
                throw new ArgumentException("The account name cannot be empty", "accountName");
            }

            if (string.IsNullOrWhiteSpace(accountKey))
            {
                throw new ArgumentException("The account key cannot be empty", "accountKey");
            }

            this.accountName = accountName;
            this.accountKey = Convert.FromBase64String(accountKey.ToString());
        }

        public void SignRequest(WebRequest webRequest, long contentLength)
        {
            if (webRequest != null)
            {
                webRequest.Headers["x-ms-date"] = this.GetDateHeader();
                webRequest.Headers["Authorization"] = GetAuthenticationHeader(webRequest, contentLength, this.accountName, this.accountKey);
            }
        }

        public void SignRequestLite(WebRequest webRequest)
        {
            if (webRequest != null)
            {
                this.AddAuthenticationHeadersLite(new RequestData { Method = webRequest.Method, RequestUri = webRequest.RequestUri }, webRequest.Headers);
            }
        }

        public void AddAuthenticationHeadersLite(RequestData requestData, WebHeaderCollection requestHeaders)
        {
            if (requestHeaders != null)
            {
                var xmsdate = this.GetDateHeader();
                requestHeaders["x-ms-version"] = "2009-09-19";
                requestHeaders["x-ms-date"] = xmsdate;
                requestHeaders["Authorization"] = GetAuthenticationHeaderLite(requestData, this.accountName, this.accountKey, xmsdate);
            }
        }

        internal virtual string GetDateHeader()
        {
            return DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
        }

        private static string GetCanonicalizedResourceVersion2(Uri address, string accountName)
        {
            var builder = new StringBuilder("/");
            builder.Append(accountName);
            builder.Append(address.AbsolutePath);

            var canonicalizedString = new CanonicalizedString(builder.ToString());
            var values = HttpUtility.ParseQueryString(address.Query);
            var allValues = new WebHeaderCollection();
            foreach (string key in values.AllKeys)
            {
                var list = new List<string>(values.GetValues(key));
                list.Sort();

                var innerBuilder = new StringBuilder();
                foreach (object obj2 in list)
                {
                    if (innerBuilder.Length > 0)
                    {
                        innerBuilder.Append(",");
                    }

                    innerBuilder.Append(obj2.ToString());
                }

                allValues.Add(string.IsNullOrWhiteSpace(key) ? key : key.ToLowerInvariant(), innerBuilder.ToString());
            }

            var keyList = new List<string>(allValues.AllKeys);
            keyList.Sort();

            foreach (string key in keyList)
            {
                var innerBuilder = new StringBuilder(string.Empty);
                innerBuilder.Append(key);
                innerBuilder.Append(":");
                innerBuilder.Append(allValues[key]);
                canonicalizedString.AppendCanonicalizedElement(innerBuilder.ToString());
            }

            return canonicalizedString.Value;
        }

        private static string GetStandardHeaderValue(WebHeaderCollection headers, string headerName)
        {
            var headerValues = GetHeaderValues(headers, headerName);
            if (headerValues.Count != 1)
            {
                return string.Empty;
            }

            return (string)headerValues[0];
        }

        private static List<string> GetHeaderValues(WebHeaderCollection headers, string headerName)
        {
            var list = new List<string>();
            var values = headers.GetValues(headerName);
            if (values != null)
            {
                foreach (string value in values)
                {
                    list.Add(value.TrimStart(new char[0]));
                }
            }

            return list;
        }

        private static string CanonicalizeHttpRequest(string accountName, long contentLength, WebRequest webRequest)
        {
            var canonicalizedString = new CanonicalizedString(webRequest.Method);
            canonicalizedString.AppendCanonicalizedElement(GetStandardHeaderValue(webRequest.Headers, "Content-Encoding"));
            canonicalizedString.AppendCanonicalizedElement(GetStandardHeaderValue(webRequest.Headers, "Content-Language"));
            canonicalizedString.AppendCanonicalizedElement((contentLength == -1L) ? string.Empty : contentLength.ToString(CultureInfo.InvariantCulture));
            canonicalizedString.AppendCanonicalizedElement(GetStandardHeaderValue(webRequest.Headers, "Content-MD5"));
            canonicalizedString.AppendCanonicalizedElement(webRequest.ContentType);
            canonicalizedString.AppendCanonicalizedElement(!string.IsNullOrWhiteSpace(GetStandardHeaderValue(webRequest.Headers, "x-ms-date")) ? string.Empty : GetStandardHeaderValue(webRequest.Headers, "Date"));
            canonicalizedString.AppendCanonicalizedElement(GetStandardHeaderValue(webRequest.Headers, "If-Modified-Since"));
            canonicalizedString.AppendCanonicalizedElement(GetStandardHeaderValue(webRequest.Headers, "If-Match"));
            canonicalizedString.AppendCanonicalizedElement(GetStandardHeaderValue(webRequest.Headers, "If-None-Match"));
            canonicalizedString.AppendCanonicalizedElement(GetStandardHeaderValue(webRequest.Headers, "If-Unmodified-Since"));
            canonicalizedString.AppendCanonicalizedElement(GetStandardHeaderValue(webRequest.Headers, "Range"));
            AddCanonicalizedHeaders(webRequest.Headers, canonicalizedString);
            AddCanonicalizedResourceVersion2(webRequest.RequestUri, accountName, canonicalizedString);
            return canonicalizedString.Value;
        }

        private static void AddCanonicalizedResourceVersion2(Uri address, string accountName, CanonicalizedString canonicalizedString)
        {
            var element = GetCanonicalizedResourceVersion2(address, accountName);

            canonicalizedString.AppendCanonicalizedElement(element);
        }

        private static void AddCanonicalizedHeaders(WebHeaderCollection headers, CanonicalizedString canonicalizedString)
        {
            var keyList = new List<string>();
            foreach (string key in headers.AllKeys)
            {
                if (key.ToLowerInvariant().StartsWith("x-ms-", StringComparison.Ordinal))
                {
                    keyList.Add(key.ToLowerInvariant());
                }
            }

            keyList.Sort();
            foreach (string key in keyList)
            {
                var builder = new StringBuilder(key);
                var colonAndComma = ":";
                foreach (string values in GetHeaderValues(headers, key))
                {
                    string value = values.Replace("\r\n", string.Empty);
                    builder.Append(colonAndComma);
                    builder.Append(value);
                    colonAndComma = ",";
                }

                canonicalizedString.AppendCanonicalizedElement(builder.ToString());
            }
        }

        private static string ComputeMacSha256(byte[] accountKey, string canonicalizedString)
        {
            var bytes = Encoding.UTF8.GetBytes(canonicalizedString);
            using (var hmacsha = new HMACSHA256(accountKey))
            {
                return Convert.ToBase64String(hmacsha.ComputeHash(bytes));
            }
        }

        private static string CanonicalizeHttpRequestLite(string accountName, string xmsdate, Uri requestUri)
        {
            if (string.IsNullOrWhiteSpace(xmsdate))
            {
                throw new ArgumentException(
                        string.Format(
                        CultureInfo.CurrentCulture,
                        "Canonicalization did not find a non empty x-ms-date header in the WebRequest. Please use a WebRequest with a valid x-ms-date header in RFC 123 format (example request.Headers[\"x-ms-date\"] = DateTime.UtcNow.ToString(\"R\", CultureInfo.InvariantCulture))",
                        new object[0]),
                    "xmsdate");
            }

            var canonicalizedString = new CanonicalizedString(xmsdate);
            canonicalizedString.AppendCanonicalizedElement(GetCanonicalizedResourceLite(requestUri, accountName));

            return canonicalizedString.Value;
        }

        private static string GetCanonicalizedResourceLite(Uri address, string accountName)
        {
            var builder = new StringBuilder("/");
            builder.Append(accountName);
            builder.Append(address.AbsolutePath);

            var parsedQuery = HttpUtility.ParseQueryString(address.Query);
            var compQuery = parsedQuery["comp"] != null ? parsedQuery["comp"] : string.Empty;
            if (!string.IsNullOrWhiteSpace(compQuery))
            {
                builder.Append("?comp=");
                builder.Append(compQuery);
            }

            return builder.ToString();
        }

        private static string GetAuthenticationHeader(WebRequest webRequest, long contentLength, string accountName, byte[] accountKey)
        {
            var canonicalizedString = CanonicalizeHttpRequest(accountName, contentLength, webRequest);
            var hmacsha256 = ComputeMacSha256(accountKey, canonicalizedString);

            return string.Format(CultureInfo.InvariantCulture, "SharedKey {0}:{1}", accountName, hmacsha256);
        }

        private static string GetAuthenticationHeaderLite(RequestData requestData, string accountName, byte[] accountKey, string xmsdate)
        {
            var canonicalizedString = CanonicalizeHttpRequestLite(accountName, xmsdate, requestData.RequestUri);
            var hmacsha256 = ComputeMacSha256(accountKey, canonicalizedString);

            return string.Format(CultureInfo.InvariantCulture, "SharedKeyLite {0}:{1}", accountName, hmacsha256);
        }
    }
}
