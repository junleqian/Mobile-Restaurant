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
    using System.Net;
    using System.Net.Browser;
    using System.Runtime.Serialization;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;

    [KnownType(typeof(StorageAccountCloudBlob))]
    [KnownType(typeof(SharedAccessSignatureServiceCloudBlob))]
    [DataContract]
    public abstract class CloudBlobBase : ICloudBlob
    {
        // Used only for deserialization purposes.
        public CloudBlobBase()
        {
        }

        public CloudBlobBase(ICloudBlobContainer container)
        {
            this.Container = container;
            this.Metadata = new WebHeaderCollection();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember(Name = "Url")]
        public virtual Uri Uri { get; set; }

        public ICloudBlobContainer Container { get; private set; }

        public WebHeaderCollection Metadata { get; private set; }

        protected IStorageCredentials Credentials { get; set; }

        public virtual void Delete(bool includeSnapshots, Action<CloudOperationResponse<bool>> callback)
        {
            var uriBuilder = this.CreateUriBuilder();

            // Set a timeout query string parameter.
            uriBuilder.Query = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}",
                uriBuilder.Query.TrimStart('?'),
                string.IsNullOrWhiteSpace(uriBuilder.Query) ? "timeout=90" : "&timeout=90");

            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uriBuilder.Uri);
            request.Headers["x-ms-version"] = "2009-09-19";
            request.Headers["x-ms-delete-snapshots"] = includeSnapshots ? "include" : "only";
            request.Method = "DELETE";

            this.SignRequest(request, 0);
            request.BeginGetResponse(
                ar =>
                {
                    try
                    {
                            var response = (HttpWebResponse)request.EndGetResponse(ar);

                            if (response.StatusCode == HttpStatusCode.Accepted)
                            {
                                callback(new CloudOperationResponse<bool>(true, null));
                            }
                            else
                            {
                                var exceptionMessage = string.Format(CultureInfo.InvariantCulture, "An error occurred while deleting the blob named '{0}'", this.Name);

                                callback(new CloudOperationResponse<bool>(false, new Exception(exceptionMessage)));
                            }
                    }
                    catch (Exception exception)
                    {
                        callback(new CloudOperationResponse<bool>(false, StorageClientExceptionParser.ParseXmlWebException(exception as WebException) ?? exception));
                    }
                },
                null);
        }

        public virtual void UploadFromStream(System.IO.Stream source, Action<CloudOperationResponse<bool>> callback)
        {
            var uri = this.CreateUriBuilder().Uri;

            var uploader = new CloudBlobUploader(source, uri, this.Metadata, this.Credentials);

            uploader.UploadFinished += (s, r) => callback(r);
            uploader.StartUpload();
        }

        public virtual void SetMetadata(Action<CloudOperationResponse<bool>> callback)
        {
            if (this.Metadata.Count == 0)
            {
                callback(new CloudOperationResponse<bool>(true, null));
                return;
            }

            var uriBuilder = this.CreateUriBuilder();

            // Set a timeout query string parameter.
            uriBuilder.Query = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}",
                uriBuilder.Query.TrimStart('?'),
                string.IsNullOrWhiteSpace(uriBuilder.Query) ? "comp=metadata&timeout=90" : "&comp=metadata&timeout=90");

            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uriBuilder.Uri);
            request.Headers["x-ms-version"] = "2009-09-19";
            request.Method = "PUT";

            var metadataHeader = "x-ms-meta-";

            foreach (string key in this.Metadata.AllKeys)
            {
                request.Headers[string.Concat(metadataHeader, key)] = this.Metadata[key];
            }

            this.SignRequest(request, 0);

            request.BeginGetResponse(
                asyncResult =>
                {
                    try
                    {
                        var response = (HttpWebResponse)request.EndGetResponse(asyncResult);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            callback(new CloudOperationResponse<bool>(true, null));
                        }
                        else
                        {
                            var exceptionMessage = string.Format(CultureInfo.InvariantCulture, "An error occurred while adding a metadata to the blob '{0}'", this.Name);

                            callback(new CloudOperationResponse<bool>(false, new Exception(exceptionMessage)));
                        }
                    }
                    catch (Exception exception)
                    {
                        callback(new CloudOperationResponse<bool>(false, StorageClientExceptionParser.ParseXmlWebException(exception as WebException) ?? exception));
                    }
                },
                null);
        }

        public virtual void FetchAttributes(Action<CloudOperationResponse<bool>> callback)
        {
            var uriBuilder = this.CreateUriBuilder();

            // Set a timeout query string parameter.
            uriBuilder.Query = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}",
                uriBuilder.Query.TrimStart('?'),
                string.IsNullOrWhiteSpace(uriBuilder.Query) ? "comp=metadata&timeout=90" : "&comp=metadata&timeout=90");

            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uriBuilder.Uri);
            request.Headers["x-ms-version"] = "2009-09-19";
            request.Method = "HEAD";

            this.SignRequest(request, 0);
            request.BeginGetResponse(
                ar =>
                {
                    try
                    {
                            var response = (HttpWebResponse)request.EndGetResponse(ar);

                            this.LoadMetadata(response.Headers);

                            callback(new CloudOperationResponse<bool>(true, null));
                    }
                    catch (Exception exception)
                    {
                        callback(new CloudOperationResponse<bool>(false, StorageClientExceptionParser.ParseXmlWebException(exception as WebException) ?? exception));
                    }
                },
                null);
        }

        protected abstract UriBuilder CreateUriBuilder();

        protected abstract void SignRequest(HttpWebRequest request, long contentLength);

        private void LoadMetadata(WebHeaderCollection headers)
        {
            var metadataHeader = "x-ms-meta-";

            foreach (string key in headers.AllKeys)
            {
                if (key.StartsWith(metadataHeader))
                {
                    this.Metadata[key.Substring(metadataHeader.Length)] = headers[key];
                }
            }
        }
    }
}
