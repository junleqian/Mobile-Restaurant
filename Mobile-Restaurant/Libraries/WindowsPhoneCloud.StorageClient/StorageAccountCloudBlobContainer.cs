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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Browser;
    using System.Runtime.Serialization;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Serializers;

    [DataContract(Name = "Container", Namespace = "")]
    public class StorageAccountCloudBlobContainer : ICloudBlobContainer
    {
        // Used only for deserialization purposes.
        public StorageAccountCloudBlobContainer()
        {
        }

        public StorageAccountCloudBlobContainer(IStorageCredentials credentials)
        {
            this.Credentials = credentials;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember(Name = "Url")]
        public Uri Uri { get; set; }

        protected IStorageCredentials Credentials { get; private set; }

        public void Create(bool createPublicContainer, Action<CloudOperationResponse<bool>> callback)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(new Uri(string.Format(CultureInfo.InvariantCulture, "{0}?restype=container&timeout=10000", this.Uri)));

            request.Method = "PUT";
            request.Headers["x-ms-version"] = "2009-09-19";

            if (createPublicContainer)
            {
                request.Headers["x-ms-blob-public-access"] = "container";
            }

            request.BeginGetRequestStream(
                ar =>
                {
                    this.Credentials.SignRequest(request, 0);
                    request.BeginGetResponse(
                        asyncResult =>
                        {
                            var req = (HttpWebRequest)asyncResult.AsyncState;
                            try
                            {
                                var res = (HttpWebResponse)req.EndGetResponse(asyncResult);
                                if (res.StatusCode == HttpStatusCode.Created)
                                {
                                    callback(new CloudOperationResponse<bool>(true, null));
                                }
                                else
                                {
                                    callback(new CloudOperationResponse<bool>(false, new Exception(string.Format(CultureInfo.InvariantCulture, "Error creating container: {0}", res.StatusDescription))));
                                }
                            }
                            catch (Exception exception)
                            {
                                var webException = exception as WebException;
                                if (webException != null)
                                {
                                    var resStream = webException.Response.GetResponseStream();
                                    var sr = new StreamReader(resStream);
                                    if (sr.ReadToEnd().Contains("ContainerAlreadyExists"))
                                    {
                                        callback(new CloudOperationResponse<bool>(false, null));
                                    }
                                    else
                                    {
                                        resStream.Position = 0;
                                        callback(new CloudOperationResponse<bool>(false, webException));
                                    }
                                }
                                else
                                {
                                    callback(new CloudOperationResponse<bool>(false, exception));
                                }
                            }
                        },
                        request);
                },
                request);
        }

        public void CreateIfNotExist(bool createPublicContainer, Action<CloudOperationResponse<bool>> callback)
        {
            this.DoesContainerExist(
                cloudOperationResponse =>
                {
                    if (cloudOperationResponse.Exception != null)
                    {
                        callback(new CloudOperationResponse<bool>(false, cloudOperationResponse.Exception));
                    }
                    else
                    {
                        if (!cloudOperationResponse.Response.Exists)
                        {
                            this.Create(createPublicContainer, callback);
                        }
                        else if (createPublicContainer && !cloudOperationResponse.Response.Public)
                        {
                            this.MakePublic(callback);
                        }
                        else
                        {
                            callback(new CloudOperationResponse<bool>(true, null));
                        }
                    }
                });
        }

        public void Delete(Action<CloudOperationResponse<bool>> callback)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(new Uri(string.Format(CultureInfo.InvariantCulture, "{0}?restype=container&timeout=10000", this.Uri)));

            request.Method = "DELETE";
            request.Headers["x-ms-version"] = "2009-09-19";

            request.BeginGetRequestStream(
                ar =>
                {
                    this.Credentials.SignRequest(request, 0);
                    request.BeginGetResponse(
                        asyncResult =>
                        {
                            var req = (HttpWebRequest)asyncResult.AsyncState;
                            try
                            {
                                var res = (HttpWebResponse)req.EndGetResponse(asyncResult);
                                if (res.StatusCode == HttpStatusCode.Accepted)
                                {
                                    callback(new CloudOperationResponse<bool>(true, null));
                                }
                                else
                                {
                                    callback(new CloudOperationResponse<bool>(false, new Exception(string.Format(CultureInfo.InvariantCulture, "Error creating container: {0}", res.StatusDescription))));
                                }
                            }
                            catch (Exception exception)
                            {
                                var webException = exception as WebException;
                                if (webException != null)
                                {
                                    var resStream = webException.Response.GetResponseStream();
                                    var sr = new StreamReader(resStream);
                                    if (sr.ReadToEnd().Contains("ContainerBeingDeleted"))
                                    {
                                        callback(new CloudOperationResponse<bool>(false, null));
                                    }
                                    else
                                    {
                                        resStream.Position = 0;
                                        callback(new CloudOperationResponse<bool>(false, webException));
                                    }
                                }
                                else
                                {
                                    callback(new CloudOperationResponse<bool>(false, exception));
                                }
                            }
                        },
                        request);
                },
                request);
        }

        public ICloudBlob GetBlobReference(string blobAddressUri)
        {
            return new StorageAccountCloudBlob(this, this.Credentials)
            {
                Name = blobAddressUri,
                Uri = new Uri(string.Concat(this.Uri, "/", blobAddressUri))
            };
        }

        public void ListBlobs(Action<CloudOperationResponse<System.Collections.Generic.IEnumerable<ICloudBlob>>> callback)
        {
            this.InnerListBlobsWithPrefixUsingStorageKey(null, false, callback);
        }

        public void ListBlobs(string blobPrefix, bool useFlatBlobListing, Action<CloudOperationResponse<System.Collections.Generic.IEnumerable<ICloudBlob>>> callback)
        {
            this.InnerListBlobsWithPrefixUsingStorageKey(blobPrefix, useFlatBlobListing, callback);
        }

        private void DoesContainerExist(Action<CloudOperationResponse<ContainerExistResponse>> callback)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(new Uri(string.Format(CultureInfo.InvariantCulture, "{0}?restype=container&comp=acl&timeout=10000", this.Uri)));

            request.Method = "HEAD";
            request.Headers["x-ms-version"] = "2009-09-19";
            this.Credentials.SignRequest(request, 0);
            request.BeginGetResponse(
                asyncResult =>
                {
                    var req = (HttpWebRequest)asyncResult.AsyncState;
                    try
                    {
                        var res = (HttpWebResponse)req.EndGetResponse(asyncResult);
                        var result = new ContainerExistResponse
                        {
                            Exists = true,
                            Public = res.Headers.AllKeys.Contains("x-ms-blob-public-access")
                        };

                        callback(new CloudOperationResponse<ContainerExistResponse>(result, null));
                    }
                    catch (Exception exception)
                    {
                        var webException = exception as WebException;
                        var result = new ContainerExistResponse { Exists = false };
                        if (webException != null)
                        {
                            var resStream = webException.Response.GetResponseStream();
                            var sr = new StreamReader(resStream);
                            var responseContent = sr.ReadToEnd();
                            if (webException.Message.Contains("NotFound"))
                            {
                                callback(new CloudOperationResponse<ContainerExistResponse>(result, null));
                            }
                            else
                            {
                                callback(new CloudOperationResponse<ContainerExistResponse>(result, exception));
                            }
                        }
                        else
                        {
                            callback(new CloudOperationResponse<ContainerExistResponse>(result, exception));
                        }
                    }
                },
                request);
        }

        private void InnerListBlobsWithPrefixUsingStorageKey(string blobPrefix, bool useFlatBlobListing, Action<CloudOperationResponse<System.Collections.Generic.IEnumerable<ICloudBlob>>> callback)
        {
            var serviceOperation = string.Concat(this.Uri, "?restype=container&comp=list&timeout=90");

            if (!useFlatBlobListing)
            {
                serviceOperation = string.Concat(serviceOperation, "&delimiter=/");
            }

            if (!string.IsNullOrWhiteSpace(blobPrefix))
            {
                serviceOperation = string.Concat(serviceOperation, "&prefix=", blobPrefix.Replace(@"\", "/"));
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
                        var results = serializer.ReadObject(response.GetResponseStream());

                        callback(new CloudOperationResponse<System.Collections.Generic.IEnumerable<ICloudBlob>>((results != null) ? results : new ICloudBlob[0], null));
                    }
                    catch (Exception exception)
                    {
                        callback(new CloudOperationResponse<System.Collections.Generic.IEnumerable<ICloudBlob>>(null, StorageClientExceptionParser.ParseXmlWebException(exception as WebException) ?? exception));
                    }
                },
                null);
        }

        private void MakePublic(Action<CloudOperationResponse<bool>> callback)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(new Uri(string.Format(CultureInfo.InvariantCulture, "{0}?restype=container&comp=acl&timeout=10000", this.Uri)));

            request.Method = "PUT";
            request.Headers["x-ms-version"] = "2009-09-19";
            request.Headers["x-ms-blob-public-access"] = "container";

            request.BeginGetRequestStream(
                ar =>
                {
                    this.Credentials.SignRequest(request, 0);
                    request.BeginGetResponse(
                        asyncResult =>
                        {
                            var req = (HttpWebRequest)asyncResult.AsyncState;
                            try
                            {
                                var res = (HttpWebResponse)req.EndGetResponse(asyncResult);
                                callback(new CloudOperationResponse<bool>(true, null));
                            }
                            catch (Exception exception)
                            {
                                callback(new CloudOperationResponse<bool>(false, StorageClientExceptionParser.ParseXmlWebException(exception as WebException) ?? exception));
                            }
                        },
                        request);
                },
                request);
        }

        internal struct ContainerExistResponse
        {
            public bool Exists { get; set; }

            public bool Public { get; set; }
        }
    }
}
