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
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Browser;
    using System.Runtime.Serialization;
    using System.Text;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Exceptions;

    public class RegistrationClient : IRegistrationClient
    {
        private const string RegisterSignatureOperation = "/register";
        private const string ValidateSignatureOperation = "/validate";

        private readonly IStorageCredentials storageCredentials;

        private readonly string serviceUri;
        private IWebRequestCreate clientHttp;

        public RegistrationClient(string serviceUri, IStorageCredentials storageCredentials)
        {
            this.serviceUri = serviceUri;
            this.storageCredentials = storageCredentials;
        }

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

        public void CheckUserRegistration(Action<RegistrationSuccessEventArgs> registeredCallback, Action<RegistrationSuccessEventArgs> notRegisteredCallback, Action<RegistrationExceptionEventArgs> exceptionCallback)
        {
            var uriBuilder = new UriBuilder(this.serviceUri);
            uriBuilder.Path += ValidateSignatureOperation;

            // Adding an incremental seed to avoid a cached response.
            uriBuilder.Query += string.Format(CultureInfo.InvariantCulture, "incrementalSeed={0}", DateTime.UtcNow.Ticks);

            var request = this.ClientHttp.Create(uriBuilder.Uri);
            request.Method = "GET";

            this.storageCredentials.SignRequest(request, -1);

            request.BeginGetResponse(
                asyncResult =>
                {
                    try
                    {
                        var response = request.EndGetResponse(asyncResult);
                        var reader = new StreamReader(response.GetResponseStream());
                        var serializer = new DataContractSerializer(typeof(string));
                        var responseBody = serializer.ReadObject(response.GetResponseStream()) as string;
                        reader.Close();
                        var registered = default(bool);
                        if (bool.TryParse(responseBody, out registered) && registered)
                        {
                            registeredCallback(new RegistrationSuccessEventArgs());
                        }
                        else
                        {
                            notRegisteredCallback(new RegistrationSuccessEventArgs());
                        }
                    }
                    catch (Exception exception)
                    {
                        // Error.
                        var webException = exception as WebException;
                        if (webException != null)
                        {
                            exceptionCallback(new RegistrationExceptionEventArgs { Exception = StorageClientExceptionParser.ParseHttpWebException(webException), IsError = true });
                        }
                        else
                        {
                            exceptionCallback(new RegistrationExceptionEventArgs { Exception = new HttpWebException(exception.GetBaseException().Message), IsError = true });
                        }
                    }
                },
            null);
        }
        
        public void Register(string userName, string email, Action<RegistrationSuccessEventArgs> successCallback, Action<RegistrationExceptionEventArgs> exceptionCallback)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email))
            {
                return;
            }

            var uriBuilder = new UriBuilder(this.serviceUri);
            uriBuilder.Path += RegisterSignatureOperation;

            var request = this.ClientHttp.Create(uriBuilder.Uri);
            request.Method = "POST";
            request.ContentType = "text/xml";

            var data = new RegistrationUser { Name = userName, EMail = email };
            var postData = string.Empty;

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(RegistrationUser));
                serializer.WriteObject(stream, data);
                byte[] contextAsByteArray = stream.ToArray();
                postData = Encoding.UTF8.GetString(contextAsByteArray, 0, contextAsByteArray.Length);
            }
            
            int encodedDataLength = Encoding.UTF8.GetByteCount(postData);

            this.storageCredentials.SignRequest(request, encodedDataLength);

            request.BeginGetRequestStream(
                ar =>
                {
                    var postStream = request.EndGetRequestStream(ar);
                    var byteArray = Encoding.UTF8.GetBytes(postData);

                    postStream.Write(byteArray, 0, encodedDataLength);
                    postStream.Close();

                    request.BeginGetResponse(
                        asyncResult =>
                        {
                            var status = string.Empty;
                            try
                            {
                                var response = request.EndGetResponse(asyncResult);
                                var serializer = new DataContractSerializer(typeof(string));
                                status = serializer.ReadObject(response.GetResponseStream()) as string;
                                if (string.IsNullOrWhiteSpace(status) || !status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                                {
                                    exceptionCallback(new RegistrationExceptionEventArgs() { IsError = false, Exception = new HttpWebException(status) });
                                }
                                else
                                {
                                    successCallback(new RegistrationSuccessEventArgs() { UserName = userName });
                                }
                            }
                            catch (Exception exception)
                            {
                                // Error.
                                var webException = exception as WebException;
                                if (webException != null)
                                {
                                    exceptionCallback(new RegistrationExceptionEventArgs() { IsError = true, Exception = StorageClientExceptionParser.ParseHttpWebException(webException) });
                                }
                                else
                                {
                                    exceptionCallback(new RegistrationExceptionEventArgs() { IsError = true, Exception = new HttpWebException(exception.GetBaseException().Message) });
                                }
                            }
                        },
                    null);
                },
            request);
        }
    }
}
