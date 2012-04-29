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
    using System.Text;

    /// <summary>
    /// Helper class used to create a Service Uri during a service invocation.
    /// </summary>
    public class ServiceUri
    {
        /// <summary>
        /// Initializes a new instance of the ServiceUri class.
        /// </summary>
        public ServiceUri()
        {
            this.Arguments = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Initializes a new instance of the ServiceUri class.
        /// </summary>
        /// <param name="hostUrl">Specifies the host url. For example: http://ocr2.hawaii-services.net.</param>
        /// <param name="signature">Specifies the service signature.</param>
        public ServiceUri(string hostUrl, string signature)
        {
            this.HostUrl = hostUrl;
            this.Signature = signature;
            this.Arguments = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Gets or sets service host url.
        /// </summary>
        public string HostUrl { get; set; }

        /// <summary>
        /// Gets or sets service's REST signature
        /// For Example :
        /// Service Url : http://hawaiispeech/SpeechRecognition
        /// Signature   : SpeechRecognition
        /// Service Url : http://hawaiispeech/SpeechRecognition/GrammerName/
        /// Signature   : SpeechRecognition/GrammarName.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Gets or sets arguments used for the Query String part of the Service Url.
        /// </summary>
        private List<KeyValuePair<string, string>> Arguments { get; set; }

        /// <summary>
        /// A Helper method to add a query string key-value pair.
        /// </summary>
        /// <param name="key">
        /// The key in the query string.
        /// </param>
        /// <param name="value">
        /// The value corresponding to the key.
        /// </param>
        public void AddQueryString(string key, string value)
        {
            this.Arguments.Add(new KeyValuePair<string, string>(key, value));
        }

        /// <summary>
        /// Returns the service uri string.
        /// </summary>
        /// <returns>A valid Service Uri string.</returns>
        public override string ToString()
        {
            StringBuilder address = new StringBuilder();

            // Create the service uri
            if (string.IsNullOrEmpty(this.Signature))
            {
                address.AppendFormat("{0}", this.HostUrl);
            }
            else
            {
                address.AppendFormat("{0}/{1}", this.HostUrl, this.Signature);
            }

            int count = this.Arguments.Count;

            // Add ? to start the query strings
            if (count != 0)
            {
                address.Append("?");
            }

            // Form the query string from the arguments collection.
            foreach (KeyValuePair<string, string> item in this.Arguments)
            {
                address.AppendFormat("{0}={1}", item.Key, item.Value);
                address.Append("&");
            }

            if (count != 0)
            {
                // Remove the last ambersand.
                address.Remove(address.Length - 1, 1);
            }

            // Return the service uri string.
            return address.ToString();
        }
    }
}
