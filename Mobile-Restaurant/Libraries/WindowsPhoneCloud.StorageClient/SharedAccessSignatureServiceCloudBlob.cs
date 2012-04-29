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
    using System.Runtime.Serialization;

    [DataContract(Name = "Blob")]
    public class SharedAccessSignatureServiceCloudBlob : CloudBlobBase
    {
        private readonly ISharedAccessSignatureServiceClient sasService;
        
        private Uri sharedAccessSignatureUri;

        // Used only for deserialization purposes.
        public SharedAccessSignatureServiceCloudBlob()
        {
        }

        public SharedAccessSignatureServiceCloudBlob(ICloudBlobContainer container, ISharedAccessSignatureServiceClient sasService)
            : base(container)
        {
            if (sasService == null)
            {
                throw new ArgumentNullException("sasService", "The Shared Access Signature service client cannot be null.");
            }

            this.sasService = sasService;
        }

        public override Uri Uri
        {
            get
            {
                if (this.sharedAccessSignatureUri == null)
                {
                    return base.Uri;
                }

                var builder = this.CreateUriBuilder();
                builder.Query = string.Empty;

                return builder.Uri;
            }

            set
            {
                base.Uri = value;
            }
        }

        public override void Delete(bool includeSnapshots, Action<CloudOperationResponse<bool>> callback)
        {
            this.ExecuteWithSAS(() => base.Delete(includeSnapshots, callback), callback);
        }

        public override void UploadFromStream(System.IO.Stream source, Action<CloudOperationResponse<bool>> callback)
        {
            this.ExecuteWithSAS(() => base.UploadFromStream(source, callback), callback);
        }

        public override void SetMetadata(Action<CloudOperationResponse<bool>> callback)
        {
            this.ExecuteWithSAS(() => base.SetMetadata(callback), callback);
        }

        public override void FetchAttributes(Action<CloudOperationResponse<bool>> callback)
        {
            this.ExecuteWithSAS(() => base.FetchAttributes(callback), callback);
        }

        protected override void SignRequest(HttpWebRequest request, long contentLength)
        {
        }

        protected override UriBuilder CreateUriBuilder()
        {
            var uriBuilder = new UriBuilder(this.sharedAccessSignatureUri);
            uriBuilder.Path += string.Format(CultureInfo.InvariantCulture, "/{0}", this.Name.TrimStart('/', '\\'));

            return uriBuilder;
        }

        private void ExecuteWithSAS(Action successCallback, Action<CloudOperationResponse<bool>> failureCallback)
        {
            if (!this.IsContainerSASExpired())
            {
                successCallback();
            }
            else
            {
                this.sasService.GetContainerSharedAccessSignature(
                    this.Container.Name,
                    r =>
                    {
                        if (r.Exception == null)
                        {
                            this.sharedAccessSignatureUri = r.Response;
                            successCallback();
                        }
                        else
                        {
                            failureCallback(new CloudOperationResponse<bool>(false, r.Exception));
                        }
                    });
            }
        }

        private bool IsContainerSASExpired()
        {
            if (this.sharedAccessSignatureUri == null)
            {
                return true;
            }

            var se = Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials.HttpUtility.ParseQueryString(this.sharedAccessSignatureUri.Query)["se"];
            if (string.IsNullOrWhiteSpace(se))
            {
                return true;
            }

            DateTime expirationDate;
            if (!DateTime.TryParseExact(se, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out expirationDate))
            {
                return true;
            }

            return expirationDate <= DateTime.UtcNow;
        }
    }
}
