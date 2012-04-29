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

namespace Microsoft.Samples.Hawaii.Services.Client
{
    using System;
    using System.Text;

    /// <summary>
    /// ClientIdentity represents a client identity for the purposes of communicating with the server.
    /// </summary>
    public class ClientIdentity
    {
        /// <summary>
        /// Initializes a new instance of the ClientIdentity class.
        /// </summary>
        /// <param name="appId">Specifies an application id.</param>
        public ClientIdentity(string appId) :
            this(appId, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ClientIdentity class.
        /// </summary>
        /// <param name="appId">Specifies the Hawaii Application Id.</param>
        /// <param name="registrationId">Specifies a registration id.</param>
        /// <param name="secretKey">Specifies a secret key.</param>
        public ClientIdentity(string appId, string registrationId, string secretKey)
        {
            this.ApplicationId = appId;
            this.RegistrationId = registrationId;
            this.SecretKey = secretKey;
        }

        /// <summary>
        /// Gets or sets the Hawaii Application Id.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the registration id.
        /// </summary>
        public string RegistrationId { get; set; }

        /// <summary>
        /// Gets or sets the secret key.
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Gets the identity token that is used when communicating with the server.
        /// </summary>
        /// <returns>Endcode token.</returns>
        public string GetEncodedToken()
        {
            string token = string.Empty;

            if (!string.IsNullOrEmpty(this.RegistrationId) &&
                !string.IsNullOrEmpty(this.SecretKey))
            {
                if (!string.IsNullOrEmpty(this.ApplicationId))
                {
                    // If this app token is not empty, take only id and secret key
                    token = string.Format("{0}:{1}:{2}", this.ApplicationId, this.RegistrationId, this.SecretKey);
                }
                else
                {
                    token = string.Format("{0}:{1}", this.RegistrationId, this.SecretKey);
                }
            }
            else if (!string.IsNullOrEmpty(this.ApplicationId))
            {
                token = this.ApplicationId;
            }

            if (!string.IsNullOrEmpty(token))
            {
                return string.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(token)));
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
