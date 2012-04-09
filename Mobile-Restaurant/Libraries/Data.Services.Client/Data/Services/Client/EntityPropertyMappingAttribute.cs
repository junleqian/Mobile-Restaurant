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
    using Microsoft.Samples.Data.Services.Client;

    public enum SyndicationItemProperty
    {
        CustomProperty,

        AuthorEmail,

        AuthorName,

        AuthorUri,

        ContributorEmail,

        ContributorName,

        ContributorUri,

        Updated,

        Published,

        Rights,

        Summary,

        Title
    }

    public enum SyndicationTextContentKind
    {
        Plaintext,

        Html,

        Xhtml
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class EntityPropertyMappingAttribute : Attribute
    {
#region Private Members

        private const string AtomNamespacePrefix = "atom";

        private readonly string sourcePath;

        private readonly string targetPath;

        private readonly SyndicationItemProperty targetSyndicationItem;

        private readonly SyndicationTextContentKind targetTextContentKind;

        private readonly string targetNamespacePrefix;

        private readonly string targetNamespaceUri;

        private readonly bool keepInContent;
#endregion

#region Constructors

        public EntityPropertyMappingAttribute(string sourcePath, SyndicationItemProperty targetSyndicationItem, SyndicationTextContentKind targetTextContentKind, bool keepInContent)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("sourcePath"));
            }

            this.sourcePath            = sourcePath;
            this.targetPath            = SyndicationItemPropertyToPath(targetSyndicationItem);
            this.targetSyndicationItem = targetSyndicationItem;
            this.targetTextContentKind = targetTextContentKind;
            this.targetNamespacePrefix = EntityPropertyMappingAttribute.AtomNamespacePrefix;
            this.targetNamespaceUri    = XmlConstants.AtomNamespace;
            this.keepInContent         = keepInContent;
        }

        public EntityPropertyMappingAttribute(string sourcePath, string targetPath, string targetNamespacePrefix, string targetNamespaceUri, bool keepInContent)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("sourcePath"));
            }

            this.sourcePath = sourcePath;

            if (string.IsNullOrEmpty(targetPath))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("targetPath"));
            }

            if (targetPath[0] == '@')
            {
                throw new ArgumentException(Strings.EpmTargetTree_InvalidTargetPath(targetPath));
            }

            this.targetPath = targetPath;

            this.targetSyndicationItem = SyndicationItemProperty.CustomProperty;
            this.targetTextContentKind = SyndicationTextContentKind.Plaintext;
            this.targetNamespacePrefix = targetNamespacePrefix;

            if (string.IsNullOrEmpty(targetNamespaceUri))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("targetNamespaceUri"));
            }

            this.targetNamespaceUri = targetNamespaceUri;

            Uri uri;
            if (!Uri.TryCreate(targetNamespaceUri, UriKind.Absolute, out uri))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_TargetNamespaceUriNotValid(targetNamespaceUri));
            }

            this.keepInContent = keepInContent;
        }

        #region Properties

        public string SourcePath
        {
            get { return this.sourcePath; }
        }

        public string TargetPath
        {
            get { return this.targetPath; }
        }

        public SyndicationItemProperty TargetSyndicationItem
        {
            get { return this.targetSyndicationItem; }
        }

        public string TargetNamespacePrefix
        {
            get { return this.targetNamespacePrefix; }
        }

        public String TargetNamespaceUri
        {
            get { return this.targetNamespaceUri; }
        }

        public SyndicationTextContentKind TargetTextContentKind
        {
            get { return this.targetTextContentKind; }
        }

        public bool KeepInContent
        {
            get { return this.keepInContent; }
        }

        #endregion

        internal static String SyndicationItemPropertyToPath(SyndicationItemProperty targetSyndicationItem)
        {
            switch (targetSyndicationItem)
            {
                case SyndicationItemProperty.AuthorEmail:
                    return "author/email";
                case SyndicationItemProperty.AuthorName:
                    return "author/name";
                case SyndicationItemProperty.AuthorUri:
                    return "author/uri";
                case SyndicationItemProperty.ContributorEmail:
                    return "contributor/email";
                case SyndicationItemProperty.ContributorName:
                    return "contributor/name";
                case SyndicationItemProperty.ContributorUri:
                    return "contributor/uri";
                case SyndicationItemProperty.Updated:
                    return "updated";
                case SyndicationItemProperty.Published:
                    return "published";
                case SyndicationItemProperty.Rights:
                    return "rights";
                case SyndicationItemProperty.Summary:
                    return "summary";
                case SyndicationItemProperty.Title:
                    return "title";
                default:
                    throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("targetSyndicationItem"));
            }
        }

#endregion 
    }
}
