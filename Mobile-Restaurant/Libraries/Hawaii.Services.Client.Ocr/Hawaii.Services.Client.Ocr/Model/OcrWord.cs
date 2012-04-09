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
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    /// Contains one word item that is obtained after a Hawaii OCR call.
    /// </summary>
    [DataContract]
    public partial class OcrWord
    {
        /// <summary>
        /// Initializes a new instance of the OcrWord class.
        /// </summary>
        public OcrWord()
        {
        }
        
        /// <summary>
        /// Gets or sets the text of the word.
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the confidence of the word.
        /// </summary>
        [DataMember]
        public string Confidence { get; set; }

        /// <summary>
        /// Gets or sets the bounding box of the word in a string format.
        /// The box is described as X0,Y0,Width,Height. 
        /// X0,Y0 are the coordinates of the top left corner of the word relative to the top left corner of the image.
        /// </summary>
        [DataMember]
        public string Box { get; set; }

        /// <summary>
        /// Returns a System.String that represents this OcrWord instance.
        /// </summary>
        /// <returns>
        /// A System.String that represents this OcrWord instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", this.Text, this.Confidence);
        }
    }
}
