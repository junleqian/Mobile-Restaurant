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
    using System.IO;
    using System.Net;
    using System.Net.Browser;
    using System.Runtime.Serialization;
    using System.Text;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Exceptions;

    public class AuthenticationClient : IAuthenticationClient
    {
        private const string LoginSignatureOperation = "/login";
        private const string ValidateSignatureOperation = "/validate";
        private const string RegisterSignatureOperation = "/register";

        private string serviceUri;
        private IWebRequestCreate clientHttp;

        public AuthenticationClient(string serviceUri)
        {
            this.serviceUri = serviceUri;
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

        public void Login(string userName, string password, Action<AuthenticationSuccessEventArgs> successCallback, Action<AuthenticationExceptionEventArgs> exceptionCallback)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            var uriBuilder = new UriBuilder(this.serviceUri);
            uriBuilder.Path += LoginSignatureOperation;

            var request = this.ClientHttp.Create(uriBuilder.Uri);
            request.Method = "POST";
            request.ContentType = "text/xml";

            var data = new Login { UserName = userName, Password = password };
            var postData = string.Empty;

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(Login));
                serializer.WriteObject(stream, data);
                byte[] contextAsByteArray = stream.ToArray();
                postData = Encoding.UTF8.GetString(contextAsByteArray, 0, contextAsByteArray.Length);
            }

            int encodedDataLength = Encoding.UTF8.GetByteCount(postData);

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
                            var token = string.Empty;
                            try
                            {
                                var response = request.EndGetResponse(asyncResult);
                                var serializer = new DataContractSerializer(typeof(string));
                                token = serializer.ReadObject(response.GetResponseStream()) as string;
                                if (string.IsNullOrWhiteSpace(token))
                                {
                                    // Wrong credentials.
                                    exceptionCallback(new AuthenticationExceptionEventArgs { WrongCredentials = true, IsError = false });
                                }
                                else
                                {
                                    // Success.
                                    successCallback(new AuthenticationSuccessEventArgs { UserName = userName, AuthenticationToken = token });
                                }
                            }
                            catch (Exception exception)
                            {
                                // Error.
                                var webException = exception as WebException;
                                if (webException != null)
                                {
                                    exceptionCallback(new AuthenticationExceptionEventArgs { Exception = StorageClientExceptionParser.ParseHttpWebException(webException), WrongCredentials = false, IsError = true });
                                }
                                else
                                {
                                    exceptionCallback(new AuthenticationExceptionEventArgs { Exception = new HttpWebException(exception.GetBaseException().Message), WrongCredentials = false, IsError = true });
                                }
                            }
                        },
                    null);
                },
            request);
        }

        public void Validate(string token, Action<AuthenticationSuccessEventArgs> successCallback, Action<AuthenticationExceptionEventArgs> exceptionCallback)
        {
            var uriBuilder = new UriBuilder(this.serviceUri);
            uriBuilder.Path += ValidateSignatureOperation;

            var request = this.ClientHttp.Create(uriBuilder.Uri);
            request.Method = "POST";
            request.ContentType = "text/xml";

            var postData = string.Empty;

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(string));
                serializer.WriteObject(stream, token);

                var contextAsByteArray = stream.ToArray();
                postData = Encoding.UTF8.GetString(contextAsByteArray, 0, contextAsByteArray.Length);
            }

            int encodedDataLength = Encoding.UTF8.GetByteCount(postData);

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
                            try
                            {
                                var response = request.EndGetResponse(asyncResult);
                                var reader = new StreamReader(response.GetResponseStream());
                                var serializer = new DataContractSerializer(typeof(string));
                                var userName = serializer.ReadObject(response.GetResponseStream()) as string;
                                reader.Close();

                                if (string.IsNullOrWhiteSpace(userName))
                                {
                                    // Wrong credentials.
                                    exceptionCallback(new AuthenticationExceptionEventArgs { IsError = false, WrongCredentials = true });
                                }
                                else
                                {
                                    // Success.
                                    successCallback(new AuthenticationSuccessEventArgs { UserName = userName, AuthenticationToken = token });
                                }
                            }
                            catch (Exception exception)
                            {
                                // Error.
                                var webException = exception as WebException;
                                if (webException != null)
                                {
                                    exceptionCallback(new AuthenticationExceptionEventArgs { Exception = StorageClientExceptionParser.ParseHttpWebException(webException), IsError = true, WrongCredentials = false });
                                }
                                else
                                {
                                    exceptionCallback(new AuthenticationExceptionEventArgs { Exception = new HttpWebException(exception.GetBaseException().Message), IsError = true, WrongCredentials = false });
                                }
                            }
                        },
                    null);
                },
                request);
        }

        public void Register(string userName, string email, string password, string confirmPassword, Action<RegistrationSuccessEventArgs> successCallback, Action<RegistrationExceptionEventArgs> exceptionCallback)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                return;
            }

            if (!password.Equals(confirmPassword, StringComparison.InvariantCulture))
            {
                exceptionCallback(new RegistrationExceptionEventArgs { IsError = false, Exception = new HttpWebException("The password does not match with its confirmation.") });
                return;
            }

            var uriBuilder = new UriBuilder(this.serviceUri);
            uriBuilder.Path += RegisterSignatureOperation;

            var request = this.ClientHttp.Create(uriBuilder.Uri);
            request.Method = "POST";
            request.ContentType = "text/xml";

            var data = new RegistrationUser { Name = userName, EMail = email, Password = password };
            var postData = string.Empty;

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(RegistrationUser));
                serializer.WriteObject(stream, data);
                byte[] contextAsByteArray = stream.ToArray();
                postData = Encoding.UTF8.GetString(contextAsByteArray, 0, contextAsByteArray.Length);
            }

            int encodedDataLength = Encoding.UTF8.GetByteCount(postData);

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
                                    // Registration Failed.
                                    exceptionCallback(new RegistrationExceptionEventArgs { Exception = new HttpWebException(status), IsError = false });
                                }
                                else
                                {
                                    // Success.
                                    successCallback(new RegistrationSuccessEventArgs { UserName = userName });
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
                },
            request);
        }
    }
}