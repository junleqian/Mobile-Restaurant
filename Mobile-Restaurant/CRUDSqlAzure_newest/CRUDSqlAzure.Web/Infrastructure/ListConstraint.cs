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

namespace Microsoft.Samples.CRUDSqlAzure.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Routing;

    public enum ListConstraintType
    {
        Exclude,
        Include
    }

    public class ListConstraint : IRouteConstraint
    {
        public ListConstraint()
            : this(ListConstraintType.Include, new string[] { })
        {
        }

        public ListConstraint(params string[] list)
            : this(ListConstraintType.Include, list)
        {
        }

        public ListConstraint(ListConstraintType listType, params string[] list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            this.ListType = listType;
            this.List = new List<string>(list);
        }

        public ListConstraintType ListType { get; private set; }

        public IList<string> List { get; private set; }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException("parameterName");
            }

            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            var value = values[parameterName.ToLowerInvariant()] as string;
            var found = this.List.Any(s => s.Equals(value, StringComparison.OrdinalIgnoreCase));

            return this.ListType == ListConstraintType.Include ? found : !found;
        }
    }
}