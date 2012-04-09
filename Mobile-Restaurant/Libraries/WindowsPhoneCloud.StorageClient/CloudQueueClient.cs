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

namespace Microsoft.Samples.WindowsPhoneCloud.StorageClient
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Browser;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Serializers;

    public class CloudQueueClient : ICloudQueueClient
    {
        private Uri queueBaseUri;

        /// <summary>
        /// Initializes a new instance of the CloudQueueClient class to perform operations with Queues using the Azure Storage credentials.
        /// </summary>
        /// <param name="queueBaseAddress">The Queue service endpoint to use to create the client.</param>
        /// <param name="credentials">The account credentials.</param>
        public CloudQueueClient(string queueBaseAddress, IStorageCredentials credentials)
        {
            this.queueBaseUri = new Uri(queueBaseAddress, UriKind.Absolute);
            this.StorageCredentials = credentials;
        }

        public IStorageCredentials StorageCredentials { get; set; }

        public ICloudQueue GetQueueReference(string queueName)
        {
            var queueUri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.queueBaseUri.ToString().TrimEnd('/'), queueName));
            return new CloudQueue
                {
                    Name = queueName,
                    Uri = queueUri,
                    StorageCredentials = this.StorageCredentials
                };
        }

        public void ListQueues(Action<CloudOperationResponse<IEnumerable<ICloudQueue>>> callback)
        {
            this.ListQueues(string.Empty, callback);
        }

        public void ListQueues(string queuePrefix, Action<CloudOperationResponse<IEnumerable<ICloudQueue>>> callback)
        {
            var serviceOperation = string.Concat(this.queueBaseUri, "?comp=list");

            if (!string.IsNullOrWhiteSpace(queuePrefix))
            {
                serviceOperation = string.Concat(serviceOperation, "&prefix=", queuePrefix);
            }

            // Adding an incremental seed to avoid a cached response.
            serviceOperation = string.Concat(serviceOperation, "&incrementalSeed=", DateTime.UtcNow.Ticks);

            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(new Uri(serviceOperation));
            request.Headers["x-ms-version"] = "2009-09-19";
            request.Method = "GET";

            this.StorageCredentials.SignRequest(request, -1);
            request.BeginGetResponse(
                ar =>
                {
                    try
                    {
                            var response = (HttpWebResponse)request.EndGetResponse(ar);
                            var serializer = new QueueDataContractSerializer(this.StorageCredentials);
                            var results = serializer.ReadObject(response.GetResponseStream());

                            callback(new CloudOperationResponse<IEnumerable<ICloudQueue>>((results != null) ? results.ToArray() : new CloudQueue[0], null));
                    }
                    catch (Exception exception)
                    {
                        callback(new CloudOperationResponse<IEnumerable<ICloudQueue>>(null, StorageClientExceptionParser.ParseXmlWebException(exception as WebException) ?? exception));
                    }
                },
                null);
        }
    }
}
