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
    using System.Diagnostics;
    using System.Reflection;
    using Microsoft.Samples.Data.Services.Client;
    using ClientTypeOrResourceType_Alias = Microsoft.Samples.Data.Services.Client.ClientType;
    using TypeOrResourceType_Alias = System.Type;

    [DebuggerDisplay("EntityPropertyMappingInfo {DefiningType}")]
    internal sealed class EntityPropertyMappingInfo
    {
        private readonly EntityPropertyMappingAttribute attribute;

        private readonly TypeOrResourceType_Alias definingType;

        private readonly string[] segmentedSourcePath;

        private readonly ClientTypeOrResourceType_Alias actualPropertyType;

        public EntityPropertyMappingInfo(EntityPropertyMappingAttribute attribute, Type definingType, ClientType actualPropertyType)
        {
            Debug.Assert(attribute != null, "attribute != null");
            Debug.Assert(definingType != null, "definingType != null");
            Debug.Assert(actualPropertyType != null, "actualPropertyType != null");

            this.attribute = attribute;
            this.definingType = definingType;
            this.actualPropertyType = actualPropertyType;

            Debug.Assert(!string.IsNullOrEmpty(attribute.SourcePath), "Invalid source path");
            this.segmentedSourcePath = attribute.SourcePath.Split('/');
        }

        public EntityPropertyMappingAttribute Attribute
        {
            get { return this.attribute; }
        }

        public TypeOrResourceType_Alias DefiningType
        {
            get { return this.definingType; }
        }

        internal object ReadPropertyValue(object element)
        {
            return ReadPropertyValue(element, this.actualPropertyType, this.segmentedSourcePath, 0);
        }

        private static object ReadPropertyValue(object element, ClientType resourceType, string[] srcPathSegments, int currentSegment)
        {
            if (element == null || currentSegment == srcPathSegments.Length)
            {
                return element;
            }
            else
            {
                String srcPathPart = srcPathSegments[currentSegment];

                ClientType.ClientProperty resourceProperty = resourceType.GetProperty(srcPathPart, true);
                if (resourceProperty == null)
                {
                    throw Error.InvalidOperation(Strings.EpmSourceTree_InaccessiblePropertyOnType(srcPathPart, resourceType.ElementTypeName));
                }

                if (resourceProperty.IsKnownType ^ (currentSegment == srcPathSegments.Length - 1))
                {
                    throw Error.InvalidOperation(!resourceProperty.IsKnownType ? Strings.EpmClientType_PropertyIsComplex(resourceProperty.PropertyName) :
                                                                                 Strings.EpmClientType_PropertyIsPrimitive(resourceProperty.PropertyName));
                }

                PropertyInfo pi = element.GetType().GetProperty(srcPathPart, BindingFlags.Instance | BindingFlags.Public);
                Debug.Assert(pi != null, "Cannot find property " + srcPathPart + "on type " + element.GetType().Name);

                return ReadPropertyValue(
                            pi.GetValue(element, null),
                            resourceProperty.IsKnownType ? null : ClientType.Create(resourceProperty.PropertyType),
                            srcPathSegments,
                            ++currentSegment);
            }
        }
    }
}
