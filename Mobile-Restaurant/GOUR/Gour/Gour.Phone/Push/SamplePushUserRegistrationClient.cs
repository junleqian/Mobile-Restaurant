namespace Gour.Phone.Push
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Browser;
    using System.Runtime.Serialization;
    using System.Text;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;
    using Gour.Phone.Models;

    public class SamplePushUserRegistrationClient : ISamplePushUserRegistrationClient
    {
        private const string RegisterNotificationOperation = "/register";
        private const string UnregisterNotificationOperation = "/unregister";
        private const string GetUpdatesNotificationOperation = "/updates";

        private readonly Uri serviceEndpoint;
        private readonly IStorageCredentials storageCredentials;
        private readonly string applicationId;

        public SamplePushUserRegistrationClient(Uri serviceEndpoint, IStorageCredentials storageCredentials, string applicationId)
        {
            this.serviceEndpoint = serviceEndpoint;
            this.storageCredentials = storageCredentials;
            this.applicationId = applicationId;
        }

        public void Connect(Action<SamplePushUserRegistrationResponse<string>> callback)
        {
            var registerOperationUriBuilder = new UriBuilder(this.serviceEndpoint);
            registerOperationUriBuilder.Path += RegisterNotificationOperation;

            PushContext.Current.Connect(c => ExecutePostServiceOperation<string>(registerOperationUriBuilder.Uri, this.storageCredentials, c.ChannelUri, this.applicationId, callback));
        }

        public void Disconnect(Action<SamplePushUserRegistrationResponse<string>> callback)
        {
            if (PushContext.Current.NotificationChannel != null)
            {
                var channelUri = PushContext.Current.NotificationChannel.ChannelUri;
                var unregisterOperationUriBuilder = new UriBuilder(this.serviceEndpoint);

                unregisterOperationUriBuilder.Path += UnregisterNotificationOperation;
                ExecutePostServiceOperation<string>(unregisterOperationUriBuilder.Uri, this.storageCredentials, channelUri, this.applicationId, callback);
            }
            else
            {
                callback(new SamplePushUserRegistrationResponse<string>(string.Empty, null));
            }

            PushContext.Current.Disconnect();
        }

        public void GetUpdates(Action<SamplePushUserRegistrationResponse<string[]>> callback)
        {
            if (PushContext.Current.NotificationChannel != null)
            {
                var channelUri = PushContext.Current.NotificationChannel.ChannelUri;
                var getUpdatesOperationUriBuilder = new UriBuilder(this.serviceEndpoint);
                getUpdatesOperationUriBuilder.Path += GetUpdatesNotificationOperation;

                ExecuteGetServiceOperation<string[]>(getUpdatesOperationUriBuilder.Uri, this.storageCredentials, channelUri, this.applicationId, callback);
            }
        }

        private static void ExecuteGetServiceOperation<T>(Uri serviceOperationUri, IStorageCredentials storageCredentials, Uri channelUri, string applicationId, Action<SamplePushUserRegistrationResponse<T>> callback)
        {
            string deviceId = App.GetDeviceId();
            var getOperationUri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}?applicationId={1}&deviceId={2}", serviceOperationUri, applicationId, deviceId));

            var request = WebRequestCreator.ClientHttp.Create(getOperationUri);
            request.Method = "GET";

            try
            {
                storageCredentials.SignRequest(request, -1);
                request.BeginGetResponse(
                    ar =>
                    {
                        var result = default(T);
                        try
                        {
                            var response = request.EndGetResponse(ar);
                            var serializer = new DataContractSerializer(typeof(T));

                            result = (T)serializer.ReadObject(response.GetResponseStream());
                            callback(new SamplePushUserRegistrationResponse<T>(result, null));
                        }
                        catch (Exception exception)
                        {
                            callback(new SamplePushUserRegistrationResponse<T>(default(T), StorageClientExceptionParser.ParseStringWebException(exception as WebException) ?? exception));
                        }
                    },
                    null);
            }
            catch (Exception exception)
            {
                callback(new SamplePushUserRegistrationResponse<T>(default(T), exception));
            }
        }

        private static void ExecutePostServiceOperation<T>(Uri serviceOperationUri, IStorageCredentials storageCredentials, Uri channelUri, string applicationId, Action<SamplePushUserRegistrationResponse<T>> callback)
        {
            var request = WebRequestCreator.ClientHttp.Create(serviceOperationUri);
            request.Method = "POST";
            request.ContentType = "text/xml";

            var postData = string.Empty;
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(PushUserServiceRequest));
                string deviceId = App.GetDeviceId();
                var newPushUserRegistration = new PushUserServiceRequest { ApplicationId = applicationId, DeviceId = deviceId, ChannelUri = channelUri };
                serializer.WriteObject(stream, newPushUserRegistration);

                byte[] contextAsByteArray = stream.ToArray();
                postData = Encoding.UTF8.GetString(contextAsByteArray, 0, contextAsByteArray.Length);
            }

            try
            {
                storageCredentials.SignRequest(request, postData.Length);
                request.BeginGetRequestStream(
                    ar =>
                    {
                        var postStream = request.EndGetRequestStream(ar);
                        var byteArray = Encoding.UTF8.GetBytes(postData);

                        postStream.Write(byteArray, 0, postData.Length);
                        postStream.Close();

                        request.BeginGetResponse(
                            asyncResult =>
                            {
                                var result = default(T);
                                try
                                {
                                    var response = request.EndGetResponse(asyncResult);
                                    var serializer = new DataContractSerializer(typeof(T));

                                    result = (T)serializer.ReadObject(response.GetResponseStream());
                                    callback(new SamplePushUserRegistrationResponse<T>(result, null));
                                }
                                catch (Exception exception)
                                {
                                    callback(new SamplePushUserRegistrationResponse<T>(default(T), StorageClientExceptionParser.ParseStringWebException(exception as WebException) ?? exception));
                                }
                            },
                        null);
                    },
                request);
            }
            catch (Exception exception)
            {
                callback(new SamplePushUserRegistrationResponse<T>(default(T), exception));
            }
        }
    }
}