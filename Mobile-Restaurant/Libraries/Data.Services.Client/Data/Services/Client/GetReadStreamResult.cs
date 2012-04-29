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
    using System.Diagnostics;
    using Microsoft.Samples.Data.Services.Http;

    internal class GetReadStreamResult : BaseAsyncResult
    {
        private readonly HttpWebRequest request;

        private HttpWebResponse response;

        internal GetReadStreamResult(
            object source,
            string method,
            HttpWebRequest request,
            AsyncCallback callback,
            object state)
            : base(source, method, callback, state)
        {
            Debug.Assert(request != null, "Null request can't be wrapped to a result.");
            this.request = request;
            this.Abortable = request;
        }

        internal void Begin()
        {
            try
            {
                IAsyncResult asyncResult;
                asyncResult = BaseAsyncResult.InvokeAsync(this.request.BeginGetResponse, GetReadStreamResult.AsyncEndGetResponse, this);

                this.CompletedSynchronously &= asyncResult.CompletedSynchronously;
            }
            catch (Exception e)
            {
                this.HandleFailure(e);
                throw;
            }
            finally
            {
                this.HandleCompleted();
            }

            Debug.Assert(!this.CompletedSynchronously || this.IsCompleted, "if CompletedSynchronously then MUST IsCompleted");
        }

        internal DataServiceStreamResponse End()
        {
            if (this.response != null)
            {
                DataServiceStreamResponse streamResponse = new DataServiceStreamResponse(this.response);
                return streamResponse;
            }
            else
            {
                return null;
            }
        }

        protected override void CompletedRequest()
        {
            Debug.Assert(null != this.response || null != this.Failure, "should have response or exception");
            if (null != this.response)
            {
                InvalidOperationException failure = null;
                if (!WebUtil.SuccessStatusCode(this.response.StatusCode))
                {
                    failure = DataServiceContext.GetResponseText(this.response.GetResponseStream, this.response.StatusCode);
                }

                if (failure != null)
                {
                    this.response.Close();
                    this.HandleFailure(failure);
                }
            }
        }

        private static void AsyncEndGetResponse(IAsyncResult asyncResult)
        {
            GetReadStreamResult state = asyncResult.AsyncState as GetReadStreamResult;
            Debug.Assert(state != null, "Async callback got called for different request.");

            try
            {
                state.CompletedSynchronously &= asyncResult.CompletedSynchronously;
                HttpWebRequest request = Util.NullCheck(state.request, InternalError.InvalidEndGetResponseRequest);

                HttpWebResponse webresponse = null;
                try
                {
                    webresponse = (HttpWebResponse)request.EndGetResponse(asyncResult);
                }
                catch (WebException e)
                {
                    webresponse = (HttpWebResponse)e.Response;
                    if (null == webresponse)
                    {
                        throw;
                    }
                }

                state.SetHttpWebResponse(webresponse);
                state.SetCompleted();
            }
            catch (Exception e)
            {
                if (state.HandleFailure(e))
                {
                    throw;
                }
            }
            finally
            {
                state.HandleCompleted();
            }
        }

        private void SetHttpWebResponse(HttpWebResponse webResponse)
        {
            Debug.Assert(webResponse != null, "Can't set a null response.");
            this.response = webResponse;
        }
    }
}
