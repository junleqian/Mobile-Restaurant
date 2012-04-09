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
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Contains one text item that is obtained after a Hawaii OCR call.
    /// </summary>
    [DataContract]
    public partial class OcrText
    {
        /// <summary>
        /// Initializes a new instance of the OcrText class.
        /// </summary>
        public OcrText()
        {
            this.Words = new List<OcrWord>();
        }
        
        /// <summary>
        /// Gets or sets the orientation of the text.
        /// </summary>
        [DataMember]
        public string Orientation { get; set; }

        /// <summary>
        /// Gets or sets the skewness of the text.
        /// </summary>
        [DataMember]
        public string Skew { get; set; }

        /// <summary>
        /// Gets or sets the list of words that are contained in the text.
        /// </summary>
        [DataMember]
        public List<OcrWord> Words { get; set; }

        /// <summary>
        /// Gets the text of all the words (this.Words) separated by 
        /// space and combined in a single string.
        /// </summary>
        public string Text
        {
            get
            {
                if (this.Words.Count == 0)
                {
                    return string.Empty;
                }

                StringBuilder sb = new StringBuilder();
                this.Words.ForEach((item) =>
                {
                    sb.Append(item.Text);
                    sb.Append(" ");
                });

                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
        }
    }
}
