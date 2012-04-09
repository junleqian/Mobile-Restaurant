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

// Copyright 2010 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.

namespace Microsoft.Samples.Data.Services.Common
{
    using System;
    using System.IO;
    using System.Xml;
    using Microsoft.Samples.Data.Services.Client;

    internal sealed class EpmCustomContentWriterNodeData : IDisposable
    {
        private bool disposed;

        internal EpmCustomContentWriterNodeData(EpmTargetPathSegment segment, object element)
        {
            this.XmlContentStream = new MemoryStream();
            XmlWriterSettings customContentWriterSettings = new XmlWriterSettings();
            customContentWriterSettings.OmitXmlDeclaration = true;
            customContentWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
            this.XmlContentWriter = XmlWriter.Create(this.XmlContentStream, customContentWriterSettings);
            this.PopulateData(segment, element);
        }

        internal EpmCustomContentWriterNodeData(EpmCustomContentWriterNodeData parentData, EpmTargetPathSegment segment, object element)
        {
            this.XmlContentStream = parentData.XmlContentStream;
            this.XmlContentWriter = parentData.XmlContentWriter;
            this.PopulateData(segment, element);
        }

        internal MemoryStream XmlContentStream
        {
            get;
            private set;
        }

        internal XmlWriter XmlContentWriter
        {
            get;
            private set;
        }

        internal String Data
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                if (this.XmlContentWriter != null)
                {
                    this.XmlContentWriter.Close();
                    this.XmlContentWriter = null;
                }

                if (this.XmlContentStream != null)
                {
                    this.XmlContentStream.Dispose();
                    this.XmlContentStream = null;
                }

                this.disposed = true;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "XmlReader on MemoryStream does not require disposal")]
        internal void AddContentToTarget(XmlWriter target)
        {
            Util.CheckArgumentNull(target, "target");
            this.XmlContentWriter.Close();
            this.XmlContentWriter = null;
            this.XmlContentStream.Seek(0, SeekOrigin.Begin);
            XmlReaderSettings customContentReaderSettings = new XmlReaderSettings();
            customContentReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlReader reader = XmlReader.Create(this.XmlContentStream, customContentReaderSettings);
            this.XmlContentStream = null;
            target.WriteNode(reader, false);
        }

        private void PopulateData(EpmTargetPathSegment segment, object element)
        {
            if (segment.EpmInfo != null)
            {
                Object propertyValue;

                try
                {
                    propertyValue = segment.EpmInfo.ReadPropertyValue(element);
                }
                catch (System.Reflection.TargetInvocationException)
                {
                    throw;
                }

                this.Data = propertyValue == null ? String.Empty : ClientConvert.ToString(propertyValue, false);
            }
        }
    }
}