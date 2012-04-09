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
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("EpmTargetPathSegment {SegmentName} HasContent={HasContent}")]
    internal class EpmTargetPathSegment
    {
        #region Private fields.

        private string segmentName;

        private string segmentNamespaceUri;

        private string segmentNamespacePrefix;

        private List<EpmTargetPathSegment> subSegments;

        private EpmTargetPathSegment parentSegment;

        #endregion Private fields.

        internal EpmTargetPathSegment()
        {
            this.subSegments = new List<EpmTargetPathSegment>();
        }

        internal EpmTargetPathSegment(string segmentName, string segmentNamespaceUri, string segmentNamespacePrefix, EpmTargetPathSegment parentSegment)
            : this()
        {
            this.segmentName = segmentName;
            this.segmentNamespaceUri = segmentNamespaceUri;
            this.segmentNamespacePrefix = segmentNamespacePrefix;
            this.parentSegment = parentSegment;
        }

        internal string SegmentName
        {
            get
            {
                return this.segmentName;
            }
        }

        internal string SegmentNamespaceUri
        {
            get
            {
                return this.segmentNamespaceUri;
            }
        }

        internal string SegmentNamespacePrefix
        {
            get
            {
                return this.segmentNamespacePrefix;
            }
        }

        internal EntityPropertyMappingInfo EpmInfo
        {
            get;
            set;
        }

        internal bool HasContent
        {
            get
            {
                return this.EpmInfo != null;
            }
        }

        internal bool IsAttribute
        {
            get
            {
                return this.SegmentName[0] == '@';
            }
        }

        internal EpmTargetPathSegment ParentSegment
        {
            get
            {
                return this.parentSegment;
            }
        }

        internal List<EpmTargetPathSegment> SubSegments
        {
            get
            {
                return this.subSegments;
            }
        }
    }
}