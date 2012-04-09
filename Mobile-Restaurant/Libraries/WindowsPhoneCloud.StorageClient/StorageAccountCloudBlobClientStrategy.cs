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
    using System.Net;
    using System.Net.Browser;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Serializers;

    internal class StorageAccountCloudBlobClientStrategy : ICloudBlobClient
    {
        private Uri storageAccountEndpoint;

        public StorageAccountCloudBlobClientStrategy(string storageAccountEndpoint, IStorageCredentials credentials = null)
        {
            this.storageAccountEndpoint = new Uri(storageAccountEndpoint, UriKind.Absolute);
            this.Credentials = credentials ?? new StorageCredentialsAnonymous();
        }

        public IStorageCredentials Credentials { get; private set; }

        public ICloudBlobContainer GetContainerReference(string containerName)
        {
            var resultContainerName = string.Empty;
            var tmpUri = default(Uri);
            if (Uri.TryCreate(containerName, UriKind.RelativeOrAbsolute, out tmpUri))
            {
                // If it is invoked with a URI, we get the Path from the Uri and attach that to the container.
                if (tmpUri.IsAbsoluteUri)
                {
                    resultContainerName = tmpUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                }
                else
                {
                    resultContainerName = containerName;
                }
            }
            else
            {
                throw new ArgumentException("The name is not valid", "containerName");
            }

            var resultContainerUri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.storageAccountEndpoint.ToString().TrimEnd('/'), resultContainerName));
            return new StorageAccountCloudBlobContainer(this.Credentials)
            {
                Name = resultContainerName,
                Uri = resultContainerUri
            };
        }

        public void ListContainers(Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            this.InnerListContainers(null, callback);
        }

        public void ListContainers(string prefix, Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            this.InnerListContainers(prefix, callback);
        }

        public void InnerListContainers(string prefix, Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            var serviceOperation = string.Concat(this.storageAccountEndpoint, "?comp=list&timeout=90");

            if (!string.IsNullOrWhiteSpace(prefix))
            {
                serviceOperation = string.Concat(serviceOperation, "&prefix=", prefix.Replace(@"\", "/"));
            }

            // Adding an incremental seed to avoid a cached response.
            serviceOperation = string.Concat(serviceOperation, "&incrementalSeed=", DateTime.UtcNow.Ticks);

            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(new Uri(serviceOperation));
            request.Headers["x-ms-version"] = "2009-09-19";
            request.Method = "GET";

            this.Credentials.SignRequest(request, -1);
            request.BeginGetResponse(
                ar =>
                {
                    try
                    {
                            var response = (HttpWebResponse)request.EndGetResponse(ar);
                            var serializer = new StorageAccountBlobDataContractSerializer();
                            var results = serializer.ReadContainerObjects(response.GetResponseStream());
                            callback(new CloudOperationResponse<IEnumerable<ICloudBlobContainer>>((results != null) ? results : null, null));
                    }
                    catch (Exception exception)
                    {
                        callback(new CloudOperationResponse<IEnumerable<ICloudBlobContainer>>(null, StorageClientExceptionParser.ParseXmlWebException(exception as WebException) ?? exception));
                    }
                },
                null);
        }
    }
}
