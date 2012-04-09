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
    using System.IO;
    using System.Text;
    using System.Xml;
    using Microsoft.Samples.Data.Services.Http;

    public abstract class DataServiceRequest
    {
        internal DataServiceRequest()
        {
        }

        public abstract Type ElementType
        {
            get;
        }

        public abstract Uri RequestUri
        {
            get;
        }

        internal abstract ProjectionPlan Plan
        {
            get;
        }

        internal abstract QueryComponents QueryComponents
        {
            get;
        }

        public override string ToString()
        {
            return this.QueryComponents.Uri.ToString();
        }   

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Returning MaterializeAtom, caller will dispose")]
        internal static MaterializeAtom Materialize(DataServiceContext context, QueryComponents queryComponents, ProjectionPlan plan, string contentType, Stream response)
        {
            Debug.Assert(null != queryComponents, "querycomponents");

            string mime = null;
            Encoding encoding = null;
            if (!string.IsNullOrEmpty(contentType))
            {
                HttpProcessUtility.ReadContentType(contentType, out mime, out encoding);
            }

            if (string.Equals(mime, XmlConstants.MimeApplicationAtom, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(mime, XmlConstants.MimeApplicationXml, StringComparison.OrdinalIgnoreCase))
            {
                if (null != response)
                {
                    XmlReader reader = XmlUtil.CreateXmlReader(response, encoding);
                    return new MaterializeAtom(context, reader, queryComponents, plan, context.MergeOption);
                }
            }

            return MaterializeAtom.EmptyResults;
        }

        internal static DataServiceRequest GetInstance(Type elementType, Uri requestUri)
        {
            Type genericType = typeof(DataServiceRequest<>).MakeGenericType(elementType);
            return (DataServiceRequest)Activator.CreateInstance(genericType, new object[] { requestUri });
        }

        internal static IEnumerable<TElement> EndExecute<TElement>(object source, DataServiceContext context, IAsyncResult asyncResult)
        {
            QueryResult result = null;
            try
            {
                result = QueryResult.EndExecute<TElement>(source, asyncResult);
                return result.ProcessResult<TElement>(context, result.ServiceRequest.Plan);
            }
            catch (DataServiceQueryException ex)
            {
                Exception inEx = ex;
                while (inEx.InnerException != null)
                {
                    inEx = inEx.InnerException;
                }

                DataServiceClientException serviceEx = inEx as DataServiceClientException;
                if (context.IgnoreResourceNotFoundException && serviceEx != null && serviceEx.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    QueryOperationResponse qor = new QueryOperationResponse<TElement>(new Dictionary<string, string>(ex.Response.Headers), ex.Response.Query, MaterializeAtom.EmptyResults);
                    qor.StatusCode = (int)HttpStatusCode.NotFound;
                    return (IEnumerable<TElement>)qor;
                }

                throw;
            }
        }

        internal IAsyncResult BeginExecute(object source, DataServiceContext context, AsyncCallback callback, object state)
        {
            QueryResult result = this.CreateResult(source, context, callback, state);
            result.BeginExecute();
            return result;
        }

        private QueryResult CreateResult(object source, DataServiceContext context, AsyncCallback callback, object state)
        {
            Debug.Assert(null != context, "context is null");
            HttpWebRequest request = context.CreateRequest(this.QueryComponents.Uri, XmlConstants.HttpMethodGet, false, null, this.QueryComponents.Version, false);
            return new QueryResult(source, "Execute", this, request, callback, state);
        }
    }
}
