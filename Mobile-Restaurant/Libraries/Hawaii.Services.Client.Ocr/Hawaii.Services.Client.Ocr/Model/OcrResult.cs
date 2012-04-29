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
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class describes the result obtained after a Hawaii OCR call.
    /// </summary>
    [DataContract]
    public partial class OcrResult
    {
        /// <summary>
        /// Initializes a new instance of the OcrResult class.
        /// </summary>
        public OcrResult()
        {
            this.InternalErrorMessage = null;
            this.OcrTexts = new List<OcrText>();
        }

        /// <summary>
        /// Gets or sets the error message if an error occures during the OCR process.
        /// </summary>
        [DataMember]
        public string InternalErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets OcrTexts items.
        /// </summary>
        [DataMember]
        public List<OcrText> OcrTexts { get; set; }
    }
}
