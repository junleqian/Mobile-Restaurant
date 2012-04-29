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
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Browser;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;

    internal class CloudBlobUploader : ICloudBlobUploader
    {
        private const long ChunkSize = 4194304;

        private Stream fileStream;
        private long dataLength;
        private long dataSent;

        private bool useBlocks;
        private string currentBlockId;
        private IList<string> blockIds = new List<string>();

        private Uri uploadUri;

        private WebHeaderCollection metadata;

        public CloudBlobUploader(Stream fileStream, Uri uploadUri, WebHeaderCollection metadata, IStorageCredentials credentials)
        {
            this.Credentials = credentials;

            this.fileStream = fileStream;
            this.uploadUri = uploadUri;
            this.dataLength = this.fileStream.Length;
            this.dataSent = 0;

            this.metadata = metadata != null ? metadata : new WebHeaderCollection();

            // Upload the blob in smaller blocks if it's a "big" file.
            this.useBlocks = (this.dataLength - this.dataSent) > ChunkSize;
        }

        public event EventHandler<CloudOperationResponse<bool>> UploadFinished;

        protected IStorageCredentials Credentials { get; private set; }

        public void StartUpload()
        {
            var uriBuilder = new UriBuilder(this.uploadUri);

            // Set a timeout query string parameter.
            uriBuilder.Query = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}",
                uriBuilder.Query.TrimStart('?'),
                string.IsNullOrWhiteSpace(uriBuilder.Query) ? "timeout=10000" : "&timeout=10000");

            if (this.useBlocks)
            {
                // Encode the block name and add it to the query string.
                this.currentBlockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                uriBuilder.Query = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}&comp=block&blockid={1}",
                    uriBuilder.Query.TrimStart('?'),
                    this.currentBlockId);
            }

            // With or without using blocks, we'll make a PUT request with the data.
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uriBuilder.Uri);

            if (this.metadata.Count > 0)
            {
                var metadataHeader = "x-ms-meta-";

                foreach (string key in this.metadata.AllKeys)
                {
                    request.Headers.Add(string.Concat(metadataHeader, key), this.metadata[key]);
                }
            }

            request.Method = "PUT";
            request.BeginGetRequestStream(this.WriteToStreamCallback, request);
        }

        private void WriteToStreamCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;
            var requestStream = request.EndGetRequestStream(asynchronousResult);
            var buffer = new byte[4096];
            var bytesRead = 0;
            var tempTotal = 0;
            this.fileStream.Position = this.dataSent;

            while (((bytesRead = this.fileStream.Read(buffer, 0, buffer.Length)) != 0) && (tempTotal + bytesRead < ChunkSize))
            {
                requestStream.Write(buffer, 0, bytesRead);
                requestStream.Flush();

                this.dataSent += bytesRead;
                tempTotal += bytesRead;
            }

            requestStream.Close();

            if (this.Credentials != null)
            {
                request.Headers["x-ms-version"] = "2009-09-19";
                request.Headers["x-ms-blob-type"] = "BlockBlob";

                this.Credentials.SignRequest(request, tempTotal);
            }

            request.BeginGetResponse(this.ReadHttpResponseCallback, request);
        }

        private void ReadHttpResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                var request = (HttpWebRequest)asynchronousResult.AsyncState;
                var response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                
                if (this.useBlocks)
                {
                    this.blockIds.Add(this.currentBlockId);
                }

                // If there is more data, send another request.
                if (this.dataSent < this.dataLength)
                {
                    this.StartUpload();
                }
                else
                {
                    this.fileStream.Close();
                    this.fileStream.Dispose();

                    if (this.useBlocks)
                    {
                        // Commit the blocks into the blob.
                        this.PutBlockList();
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            this.RaiseUploadFinished(true, null);
                        }
                        else
                        {
                            this.RaiseUploadFinished(false, new Exception(string.Format(CultureInfo.InvariantCulture, "Error uploading blob: {0}", response.StatusDescription)));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.RaiseUploadFinished(false, exception);
            }
        }

        private void PutBlockList()
        {
            var uriBuilder = new UriBuilder(this.uploadUri);
            uriBuilder.Query = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}",
                uriBuilder.Query.TrimStart('?'),
                string.IsNullOrWhiteSpace(uriBuilder.Query) ? "comp=blocklist" : "&comp=blocklist");

            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uriBuilder.Uri);
            request.Method = "PUT";

            // x-ms-version is required for put block list
            request.Headers["x-ms-version"] = "2009-09-19";

            request.BeginGetRequestStream(this.BlockListWriteToStreamCallback, request);
        }

        private void BlockListWriteToStreamCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;
            var requestStream = request.EndGetRequestStream(asynchronousResult);
            var document = new XDocument(new XElement("BlockList", from blockId in this.blockIds select new XElement("Uncommitted", blockId)));
            var writer = XmlWriter.Create(requestStream, new XmlWriterSettings { Encoding = Encoding.UTF8 });
            var length = 0L;

            document.Save(writer);
            writer.Flush();

            length = requestStream.Length;
            requestStream.Close();

            if (this.Credentials != null)
            {
                request.Headers["x-ms-version"] = "2009-09-19";

                this.Credentials.SignRequest(request, length);
            }

            request.BeginGetResponse(this.BlockListReadHttpResponseCallback, request);
        }

        private void BlockListReadHttpResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                var request = (HttpWebRequest)asynchronousResult.AsyncState;
                var response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    this.RaiseUploadFinished(true, null);
                }
                else
                {
                    this.RaiseUploadFinished(false, new Exception(string.Format(CultureInfo.InvariantCulture, "Error uploading blob: {0}", response.StatusDescription)));
                }
            }
            catch (Exception exception)
            {
                this.RaiseUploadFinished(false, exception);
            }
        }

        private void RaiseUploadFinished(bool response, Exception exception)
        {
            var uploadfinished = this.UploadFinished;
            if (uploadfinished != null)
            {
                uploadfinished(this, new CloudOperationResponse<bool>(response, StorageClientExceptionParser.ParseXmlWebException(exception as WebException) ?? exception));
            }
        }
    }
}
