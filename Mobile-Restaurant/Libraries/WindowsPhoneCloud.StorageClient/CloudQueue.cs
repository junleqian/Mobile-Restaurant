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
    using System.Runtime.Serialization;
    using System.Text;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Serializers;

    [DataContract(Namespace = "", Name = "Queue")]
    public class CloudQueue : ICloudQueue
    {
        private const int MaximumClearRetries = 10;

        [DataMember]
        public string Name { get; set; }

        [DataMember(Name = "Url")]
        public Uri Uri { get; set; }

        public IStorageCredentials StorageCredentials { get; set; }

        private Uri BaseUri
        {
            get
            {
                var uriString = this.Uri.ToString();
                var baseUri = uriString.Substring(0, uriString.LastIndexOf(this.Name, StringComparison.InvariantCulture));
                return new Uri(baseUri);
            }
        }

        public void AddMessage(CloudQueueMessage message, Action<CloudOperationResponse<bool>> callback)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            var serviceOperation = string.Format(CultureInfo.InvariantCulture, "{0}/messages", this.Uri);
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(new Uri(serviceOperation));
            request.Headers["x-ms-version"] = "2009-09-19";
            request.Method = "POST";

            var xmlData = string.Format(CultureInfo.InvariantCulture, "<QueueMessage><MessageText>{0}</MessageText></QueueMessage>", Convert.ToBase64String(message.AsBytes));

            int encodedDataLength = Encoding.UTF8.GetByteCount(xmlData);

            this.StorageCredentials.SignRequest(request, encodedDataLength);

            request.BeginGetRequestStream(
                ar =>
                {
                    var postStream = request.EndGetRequestStream(ar);
                    var byteArray = Encoding.UTF8.GetBytes(xmlData);
                    postStream.Write(byteArray, 0, encodedDataLength);
                    postStream.Close();
                    request.BeginGetResponse(
                        asyncResult =>
                        {
                            try
                            {
                                var response = (HttpWebResponse)request.EndGetResponse(asyncResult);
                                if (response.StatusCode == HttpStatusCode.Created)
                                {
                                    callback(new CloudOperationResponse<bool>(true, null));
                                }
                                else
                                {
                                    var exceptionMessage = response.StatusCode == HttpStatusCode.BadRequest
                                        ? string.Format(CultureInfo.InvariantCulture, "Trying to add queue message too large to the queue '{0}'", this.Name)
                                        : string.Format(CultureInfo.InvariantCulture, "An error occurred while adding a message to the queue '{0}'", this.Name);

                                    callback(new CloudOperationResponse<bool>(false, new Exception(exceptionMessage)));
                                }
                            }
                            catch (Exception exception)
                            {
                                callback(new CloudOperationResponse<bool>(false, StorageClientExceptionParser.ParseXmlWebException(exception as WebException) ?? exception));
                            }
                        },
                        null);
                },
                request);
        }

        public void Clear(Action<CloudOperationResponse<bool>> callback)
        {
            var serviceOperation = string.Format(CultureInfo.InvariantCulture, "{0}/messages", this.Uri);

            this.InnerClear(serviceOperation, callback, 0);
        }

        public void Create(Action<CloudOperationResponse<bool>> callback)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(this.Uri);
            request.Headers["x-ms-version"] = "2009-09-19";
            request.Method = "PUT";

            this.StorageCredentials.SignRequest(request, 0);
            request.BeginGetResponse(
                ar =>
                {
                    try
                    {
                        var response = (HttpWebResponse)request.EndGetResponse(ar);
                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            callback(new CloudOperationResponse<bool>(true, null));
                        }
                        else
                        {
                            var exceptionMessage = response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.Conflict
                                ? string.Format(CultureInfo.InvariantCulture, "The queue named '{0}' already exists", this.Name)
                                : string.Format(CultureInfo.InvariantCulture, "An error occurred while creating the queue named '{0}'", this.Name);

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

        public void CreateIfNotExist(Action<CloudOperationResponse<bool>> callback)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(this.Uri);
            request.Headers["x-ms-version"] = "2009-09-19";
            request.Method = "PUT";

            this.StorageCredentials.SignRequest(request, 0);
            request.BeginGetResponse(
                ar =>
                {
                    try
                    {
                        var response = (HttpWebResponse)request.EndGetResponse(ar);
                        var success = response.StatusCode == HttpStatusCode.Created;

                        callback(new CloudOperationResponse<bool>(success, null));
                    }
                    catch (Exception exception)
                    {
                        callback(new CloudOperationResponse<bool>(false, StorageClientExceptionParser.ParseXmlWebException(exception as WebException) ?? exception));
                    }
                },
                null);
        }

        public void Delete(Action<CloudOperationResponse<bool>> callback)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(this.Uri);
            request.Headers["x-ms-version"] = "2009-09-19";
            request.Method = "DELETE";

            this.StorageCredentials.SignRequest(request, 0);
            request.BeginGetResponse(
                ar =>
                {
                    try
                    {
                        var response = (HttpWebResponse)request.EndGetResponse(ar);

                        if (response.StatusCode == HttpStatusCode.NoContent)
                        {
                            callback(new CloudOperationResponse<bool>(true, null));
                        }
                        else
                        {
                            var exceptionMessage = string.Format(CultureInfo.InvariantCulture, "An error occurred while deleting the queue named '{0}'", this.Name);

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

        public void GetMessage(Action<CloudOperationResponse<CloudQueueMessage>> callback)
        {
            this.InnerGetMessages(1, false, null, c => callback(new CloudOperationResponse<CloudQueueMessage>(c.Response != null ? c.Response.SingleOrDefault() : null, c.Exception)));
        }

        public void GetMessage(TimeSpan visibilityTimeout, Action<CloudOperationResponse<CloudQueueMessage>> callback)
        {
            this.InnerGetMessages(1, false, visibilityTimeout, c => callback(new CloudOperationResponse<CloudQueueMessage>(c.Response != null ? c.Response.SingleOrDefault() : null, c.Exception)));
        }

        public void GetMessages(int messageCount, Action<CloudOperationResponse<IEnumerable<CloudQueueMessage>>> callback)
        {
            this.InnerGetMessages(messageCount, false, null, callback);
        }

        public void GetMessages(int messageCount, TimeSpan visibilityTimeout, Action<CloudOperationResponse<IEnumerable<CloudQueueMessage>>> callback)
        {
            this.InnerGetMessages(messageCount, false, visibilityTimeout, callback);
        }

        public void PeekMessage(Action<CloudOperationResponse<CloudQueueMessage>> callback)
        {
            this.InnerGetMessages(1, true, null, c => callback(new CloudOperationResponse<CloudQueueMessage>(c.Response != null ? c.Response.SingleOrDefault() : null, c.Exception)));
        }

        public void PeekMessages(int messageCount, Action<CloudOperationResponse<IEnumerable<CloudQueueMessage>>> callback)
        {
            this.InnerGetMessages(messageCount, true, null, callback);
        }

        public void DeleteMessage(CloudQueueMessage message, Action<CloudOperationResponse<bool>> callback)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            var serviceOperation = string.Format(CultureInfo.InvariantCulture, "{0}/messages/{1}?popreceipt={2}", this.Uri, message.Id, System.Net.HttpUtility.UrlEncode(message.PopReceipt));

            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(new Uri(serviceOperation));
            request.Headers["x-ms-version"] = "2009-09-19";
            request.Method = "DELETE";

            this.StorageCredentials.SignRequest(request, 0);
            request.BeginGetResponse(
                ar =>
                {
                    try
                    {
                        var response = (HttpWebResponse)request.EndGetResponse(ar);

                        if (response.StatusCode == HttpStatusCode.NoContent)
                        {
                            callback(new CloudOperationResponse<bool>(true, null));
                        }
                        else
                        {
                            var exceptionMessage = string.Format(CultureInfo.InvariantCulture, "An error occurred while deleting the queue named '{0}'", this.Name);

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

        public void Exists(Action<CloudOperationResponse<bool>> callback)
        {
            var serviceOperation = string.Format(CultureInfo.InvariantCulture, "{0}?comp=list&prefix={1}", this.BaseUri, this.Name);

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
                        var queues = serializer.ReadObject(response.GetResponseStream());
                        var result = queues.SingleOrDefault(q => q.Name.Equals(this.Name));
                        callback(new CloudOperationResponse<bool>(result != null, null));
                    }
                    catch (Exception exception)
                    {
                        callback(new CloudOperationResponse<bool>(false, StorageClientExceptionParser.ParseXmlWebException(exception as WebException) ?? exception));
                    }
                },
                null);
        }

        private void InnerClear(string serviceOperation, Action<CloudOperationResponse<bool>> callback, int retryCounter)
        {
            if (retryCounter > MaximumClearRetries)
            {
                var exceptionMessage = string.Format(CultureInfo.InvariantCulture, "Clear operation was not completed due to an internal time out. Please retry to finish the operation");
                callback(new CloudOperationResponse<bool>(false, new Exception(exceptionMessage)));
            }
            else
            {
                var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(new Uri(serviceOperation));
                request.Headers["x-ms-version"] = "2009-09-19";
                request.Method = "DELETE";

                this.StorageCredentials.SignRequest(request, 0);
                request.BeginGetResponse(
                    ar =>
                    {
                        try
                        {
                            var response = (HttpWebResponse)request.EndGetResponse(ar);

                            if (response.StatusCode == HttpStatusCode.NoContent)
                            {
                                callback(new CloudOperationResponse<bool>(true, null));
                            }
                            else
                            {
                                if (response.StatusCode == HttpStatusCode.InternalServerError &&
                                    response.StatusDescription.Equals("OperationTimedOut", StringComparison.Ordinal))
                                {
                                    this.InnerClear(serviceOperation, callback, ++retryCounter);
                                }
                                else
                                {
                                    var exceptionMessage = string.Format(CultureInfo.InvariantCulture, "An error occurred while clearing all the messages from the queue '{0}'", this.Name);
                                    callback(new CloudOperationResponse<bool>(false, new Exception(exceptionMessage)));
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            callback(new CloudOperationResponse<bool>(false, StorageClientExceptionParser.ParseXmlWebException(exception as WebException) ?? exception));
                        }
                    },
                    null);
            }
        }

        private void InnerGetMessages(int messageCount, bool peek, TimeSpan? visibilitytimeout, Action<CloudOperationResponse<IEnumerable<CloudQueueMessage>>> callback)
        {
            if (messageCount < 0 || messageCount > 32)
            {
                callback(new CloudOperationResponse<IEnumerable<CloudQueueMessage>>(null, new Exception("The number of messages to be retrieved from the queue must be greater than zero and less than 32")));
            }
            else if (visibilitytimeout.HasValue && (visibilitytimeout.Value.TotalHours > 2 || visibilitytimeout.Value.TotalSeconds < 0))
            {
                callback(new CloudOperationResponse<IEnumerable<CloudQueueMessage>>(null, new Exception("The visibility timeout must be greater than zero and less than 7200 seconds (2 hours)")));
            }
            else
            {
                var visibilityQueryString = visibilitytimeout.HasValue ? string.Concat("visibilitytimeout=", visibilitytimeout.Value.TotalSeconds) : string.Empty;
                var serviceOperation = string.Format(CultureInfo.InvariantCulture, "{0}/messages?numofmessages={1}{2}{3}", this.Uri, messageCount, peek ? "&peekonly=true" : string.Empty, visibilityQueryString);

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
                            var serializer = new QueueMessageDataContractSerializer();
                            var results = serializer.ReadObject(response.GetResponseStream());

                            callback(new CloudOperationResponse<IEnumerable<CloudQueueMessage>>(results, null));
                        }
                        catch (Exception exception)
                        {
                            callback(new CloudOperationResponse<IEnumerable<CloudQueueMessage>>(null, StorageClientExceptionParser.ParseXmlWebException(exception as WebException) ?? exception));
                        }
                    },
                    null);
            }
        }
    }
}
