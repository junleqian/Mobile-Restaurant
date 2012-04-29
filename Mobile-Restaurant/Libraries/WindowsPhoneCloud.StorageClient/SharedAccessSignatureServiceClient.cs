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
    using System.Runtime.Serialization;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Serializers;

    public class SharedAccessSignatureServiceClient : ISharedAccessSignatureServiceClient
    {
        private const string GetContainerSharedAccessSignatureOperation = "container";
        private const string GetBlobSharedAccessSignatureOperation = "blob";
        private const string ListContainersSharedAccessSignatureOperation = "containers";
        private const string CreateContainerSharedAccessSignatureOperation = "container";

        private readonly Uri sasServiceAddress;

        private IWebRequestCreate clientHttp;

        public SharedAccessSignatureServiceClient(string sasServiceAddress, IStorageCredentials credentials)
        {
            this.sasServiceAddress = new Uri(sasServiceAddress, UriKind.Absolute);
            this.Credentials = credentials;
        }

        public IStorageCredentials Credentials { get; private set; }

        internal IWebRequestCreate ClientHttp
        {
            get
            {
                return this.clientHttp != null ? this.clientHttp : WebRequestCreator.ClientHttp;
            }

            set
            {
                this.clientHttp = value;
            }
        }

        public void ListContainers(string containerPrefix, Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            // Adding an incremental seed to avoid a cached response.
            var serviceOperation = !string.IsNullOrWhiteSpace(containerPrefix)
                    ? string.Format(CultureInfo.InvariantCulture, "{0}?containerPrefix={1}&incrementalSeed={2}", ListContainersSharedAccessSignatureOperation, containerPrefix, DateTime.UtcNow.Ticks)
                    : string.Format(CultureInfo.InvariantCulture, "{0}?incrementalSeed={1}", ListContainersSharedAccessSignatureOperation, DateTime.UtcNow.Ticks);
            var request = this.CreateOperationRequest("GET", serviceOperation);

            request.BeginGetResponse(
                ar =>
                {
                    try
                    {
                        var response = request.EndGetResponse(ar);
                        var serializer = new SharedAccessSignatureDataContractSerializer();
                        var results = serializer.ReadContainerObjects(response.GetResponseStream());

                        callback(new CloudOperationResponse<IEnumerable<ICloudBlobContainer>>(results, null));
                    }
                    catch (Exception exception)
                    {
                        var parsedException = StorageClientExceptionParser.ParseStringWebException(exception as WebException);

                        if (parsedException != null)
                        {
                            callback(new CloudOperationResponse<IEnumerable<ICloudBlobContainer>>(null, parsedException));
                        }
                        else
                        {
                            callback(new CloudOperationResponse<IEnumerable<ICloudBlobContainer>>(null, exception));
                        }
                    }
                },
                null);
        }

        public void CreateContainer(string containerName, bool createIfNotExists, bool isPublic, Action<CloudOperationResponse<bool>> callback)
        {
            var serviceOperation = string.Format(CultureInfo.InvariantCulture, "{0}/{1}?incrementalSeed={2}", CreateContainerSharedAccessSignatureOperation, containerName, DateTime.UtcNow.Ticks);

            if (createIfNotExists)
            {
                serviceOperation = string.Concat(serviceOperation, "&createIfNotExists=true");
            }

            if (isPublic)
            {
                serviceOperation = string.Concat(serviceOperation, "&isPublic=true");
            }

            // Adding an incremental seed to avoid a cached response.
            var request = this.CreateOperationRequest("PUT", serviceOperation);
            request.BeginGetResponse(
                ar =>
                {
                    try
                    {
                        var response = request.EndGetResponse(ar);
                        var serializer = new DataContractSerializer(typeof(string));
                        var results = serializer.ReadObject(response.GetResponseStream());
                        callback(new CloudOperationResponse<bool>(true, null));
                    }
                    catch (Exception exception)
                    {
                        var parsedException = StorageClientExceptionParser.ParseStringWebException(exception as WebException);

                        if (parsedException != null)
                        {
                            callback(new CloudOperationResponse<bool>(false, parsedException));
                        }
                        else
                        {
                            callback(new CloudOperationResponse<bool>(false, exception));
                        }
                    }
                },
                null);
        }

        public void DeleteContainer(string containerName, Action<CloudOperationResponse<bool>> callback)
        {
            var serviceOperation = string.Format(CultureInfo.InvariantCulture, "{0}/{1}?incrementalSeed={2}", CreateContainerSharedAccessSignatureOperation, containerName, DateTime.UtcNow.Ticks);

            // Adding an incremental seed to avoid a cached response.
            var request = this.CreateOperationRequest("DELETE", serviceOperation);
            request.BeginGetResponse(
                ar =>
                {
                    try
                    {
                        var response = request.EndGetResponse(ar);
                        var serializer = new DataContractSerializer(typeof(string));
                        var results = serializer.ReadObject(response.GetResponseStream());
                        callback(new CloudOperationResponse<bool>(true, null));
                    }
                    catch (Exception exception)
                    {
                        var parsedException = StorageClientExceptionParser.ParseStringWebException(exception as WebException);

                        if (parsedException != null)
                        {
                            callback(new CloudOperationResponse<bool>(false, parsedException));
                        }
                        else
                        {
                            callback(new CloudOperationResponse<bool>(false, exception));
                        }
                    }
                },
                null);
        }
        
        public void GetContainerSharedAccessSignature(string containerName, Action<CloudOperationResponse<Uri>> callback)
        {
            // Adding an incremental seed to avoid a cached response.
            var request = this.CreateOperationRequest("GET", string.Format(CultureInfo.InvariantCulture, "{0}/{1}?&incrementalSeed={2}", GetContainerSharedAccessSignatureOperation, containerName, DateTime.UtcNow.Ticks));

            request.BeginGetResponse(
                ar =>
                {
                    try
                    {
                        var response = request.EndGetResponse(ar);
                        var serializer = new DataContractSerializer(typeof(Uri));
                        var containerUri = serializer.ReadObject(response.GetResponseStream()) as Uri;

                        callback(new CloudOperationResponse<Uri>(containerUri, null));
                    }
                    catch (Exception exception)
                    {
                        var parsedException = StorageClientExceptionParser.ParseStringWebException(exception as WebException);

                        if (parsedException != null)
                        {
                            callback(new CloudOperationResponse<Uri>(null, parsedException));
                        }
                        else
                        {
                            callback(new CloudOperationResponse<Uri>(null, exception));
                        }
                    }
                },
                null);
        }

        public void GetBlobsSharedAccessSignatures(string containerName, string blobPrefix, bool useFlatBlobListing, Action<CloudOperationResponse<IEnumerable<ICloudBlob>>> callback)
        {
            // Adding an incremental seed to avoid a cached response.
            var serviceOperation = !string.IsNullOrWhiteSpace(blobPrefix)
                    ? string.Format(CultureInfo.InvariantCulture, "{0}?containerName={1}&blobPrefix={2}&useFlatBlobListing={3}&incrementalSeed={4}", GetBlobSharedAccessSignatureOperation, containerName, blobPrefix, useFlatBlobListing, DateTime.UtcNow.Ticks)
                    : string.Format(CultureInfo.InvariantCulture, "{0}?containerName={1}&useFlatBlobListing={2}&incrementalSeed={3}", GetBlobSharedAccessSignatureOperation, containerName, useFlatBlobListing, DateTime.UtcNow.Ticks);
            var request = this.CreateOperationRequest("GET", serviceOperation);
            
            request.BeginGetResponse(
                ar =>
                {
                    try
                    {
                        var response = request.EndGetResponse(ar);
                        var serializer = new SharedAccessSignatureDataContractSerializer();
                        var results = serializer.ReadObject(response.GetResponseStream());

                        callback(new CloudOperationResponse<IEnumerable<ICloudBlob>>(results, null));
                    }
                    catch (Exception exception)
                    {
                        var parsedException = StorageClientExceptionParser.ParseStringWebException(exception as WebException);
                        if (parsedException != null)
                        {
                            callback(new CloudOperationResponse<IEnumerable<ICloudBlob>>(null, parsedException));
                        }
                        else
                        {
                            callback(new CloudOperationResponse<IEnumerable<ICloudBlob>>(null, exception));
                        }
                    }
                },
                null);
        }

        private WebRequest CreateOperationRequest(string method, string serviceOperation)
        {
            var operationUri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.sasServiceAddress, serviceOperation));
            var request = this.ClientHttp.Create(operationUri);
            request.Method = method;

            this.Credentials.SignRequestLite(request);

            return request;
        }
    }
}
