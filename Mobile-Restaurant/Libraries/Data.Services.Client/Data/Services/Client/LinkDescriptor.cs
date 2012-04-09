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

namespace Microsoft.Samples.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("State = {state}")]
    public sealed class LinkDescriptor : Descriptor
    {
        #region Fields

        internal static readonly System.Collections.Generic.IEqualityComparer<LinkDescriptor> EquivalenceComparer = new Equivalent();

        private object source;

        private string sourceProperty;

        private object target;

        #endregion

        internal LinkDescriptor(object source, string sourceProperty, object target)
            : this(source, sourceProperty, target, EntityStates.Unchanged)
        {
        }

        internal LinkDescriptor(object source, string sourceProperty, object target, EntityStates state)
            : base(state)
        {
            this.source = source;
            this.sourceProperty = sourceProperty;
            this.target = target;
        }

        #region Public Properties

        public object Target
        {
            get { return this.target; }
        }

        public object Source
        {
            get { return this.source; }
        }

        public string SourceProperty
        {
            get { return this.sourceProperty; }
        }

        #endregion

        internal LinkDescriptorState SaveState(
            Dictionary<object, EntityDescriptor> entityToDescriptor, Dictionary<EntityDescriptor, Guid> descriptorToId)
        {
            var state = new LinkDescriptorState();

            base.SaveState(state);

            state.SourceDescriptorId = descriptorToId[entityToDescriptor[this.source]];
            state.SourceProperty = this.sourceProperty;
            state.TargetDescriptorId = descriptorToId[entityToDescriptor[this.target]];

            return state;
        }

        internal static LinkDescriptor RestoreState(
            LinkDescriptorState state, Dictionary<Guid, EntityDescriptor> idToEntityDescriptor)
        {
            var linkDescriptor = new LinkDescriptor(
                idToEntityDescriptor[state.SourceDescriptorId].Entity,
                state.SourceProperty,
                idToEntityDescriptor[state.TargetDescriptorId].Entity);

            linkDescriptor.RestoreState((DescriptorState)state);

            return linkDescriptor;
        }

        internal override bool IsResource
        {
            get { return false; }
        }

        internal bool IsEquivalent(object src, string srcPropName, object targ)
        {
            return (this.source == src &&
                this.target == targ &&
                this.sourceProperty == srcPropName);
        }

        private sealed class Equivalent : System.Collections.Generic.IEqualityComparer<LinkDescriptor>
        {
            public bool Equals(LinkDescriptor x, LinkDescriptor y)
            {
                return (null != x) && (null != y) && x.IsEquivalent(y.source, y.sourceProperty, y.target);
            }

            public int GetHashCode(LinkDescriptor obj)
            {
                return (null != obj) ? (obj.Source.GetHashCode() ^ ((null != obj.Target) ? obj.Target.GetHashCode() : 0) ^ obj.SourceProperty.GetHashCode()) : 0;
            }
        }
    }
}
