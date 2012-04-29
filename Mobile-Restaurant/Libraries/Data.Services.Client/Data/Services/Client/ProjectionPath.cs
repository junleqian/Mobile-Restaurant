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
    #region Namespaces.

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Text;

    #endregion Namespaces.

    [DebuggerDisplay("{ToString()}")]
    internal class ProjectionPath : List<ProjectionPathSegment>
    {
        #region Constructors.

        internal ProjectionPath() : base()
        {
        }

        internal ProjectionPath(ParameterExpression root, Expression expectedRootType, Expression rootEntry)
            : base()
        {
            this.Root = root;
            this.RootEntry = rootEntry;
            this.ExpectedRootType = expectedRootType;
        }

        internal ProjectionPath(ParameterExpression root, Expression expectedRootType, Expression rootEntry, IEnumerable<Expression> members)
            : this(root, expectedRootType, rootEntry)
        {
            Debug.Assert(members != null, "members != null");

            foreach (Expression member in members)
            {
                this.Add(new ProjectionPathSegment(this, ((MemberExpression)member).Member.Name, member.Type));
            }
        }

        #endregion Constructors.

        #region Internal properties.

        internal ParameterExpression Root 
        { 
            get; 
            private set; 
        }

        internal Expression RootEntry 
        { 
            get; 
            private set; 
        }

        internal Expression ExpectedRootType 
        { 
            get; 
            private set; 
        }

        #endregion Internal properties.

        #region Methods.

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.Root.ToString());
            builder.Append("->");
            for (int i = 0; i < this.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append('.');
                }

                builder.Append(this[i].Member == null ? "*" : this[i].Member);
            }

            return builder.ToString();
        }

        #endregion Methods.
    }
}
