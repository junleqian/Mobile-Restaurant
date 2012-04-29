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

namespace Microsoft.Samples.Hawaii.Services.Client.Ocr
{
    /// <summary>
    /// Helper class that provides access to the OCR service.
    /// </summary>
    public class OcrService : IOcrService
    {
        /// <summary>
        /// Specifies the service host name. This will be used to create the service url.
        /// </summary>
        public string HostName;

        /// <summary>
        /// Specifies a signature for the REST method that executes the OCR processing.
        /// </summary>
        public string ServiceSignature;

        /// <summary>
        /// Creates a new OcrServices object
        /// </summary>
        /// <param name="hostName">Specifies the service host name. This will be used to create the service url.</param>
        /// <param name="serviceSignature">Specifies a signature for the REST method that executes the OCR processing.</param>
        public OcrService(string hostName, string serviceSignature)
        {
            this.HostName = hostName;
            this.ServiceSignature = serviceSignature;
        }

        public ServiceResult RecognizeImage(string hawaiiAppId, byte[] imageBuffer)
        {
            OcrAgent agent = new OcrAgent(
                this.HostName,
                this.ServiceSignature,
                hawaiiAppId,
                imageBuffer);

            return agent.ProcessRequest();
        }
    }
}