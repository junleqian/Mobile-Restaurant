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

namespace Microsoft.Samples.WindowsPhoneCloud.StorageClient
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Samples.Data.Services.Client;

    public class CloudTableClient : ICloudTableClient
    {
        private ITableServiceContext context;

        [CLSCompliant(false)]
        public CloudTableClient(ITableServiceContext context)
        {
            this.context = context;
        }

        public void CreateTable(string tableName, Action<CloudOperationResponse<string>> callback)
        {
            this.context.AddTable(new TableServiceSchema(tableName));
            this.context.BeginSaveChanges(
                asyncResult =>
                {
                    try
                    {
                        this.context.EndSaveChanges(asyncResult);
                        callback.Invoke(new CloudOperationResponse<string>(null, null));
                    }
                    catch (Exception exception)
                    {
                        callback.Invoke(new CloudOperationResponse<string>(null, StorageClientExceptionParser.ParseDataServiceException(exception)));
                    }
                },
                null);
        }

        public void CreateTableIfNotExist(string tableName, Action<CloudOperationResponse<bool>> callback)
        {
            this.DoesTableExist(
                tableName,
                tableExistsResult =>
                {
                    if (tableExistsResult.Exception != null || tableExistsResult.Response)
                    {
                        callback.Invoke(new CloudOperationResponse<bool>(false, tableExistsResult.Exception));
                    }
                    else
                    {
                        this.CreateTable(
                            tableName,
                            tableCreateResult =>
                            {
                                callback.Invoke(new CloudOperationResponse<bool>(tableCreateResult.Exception == null, tableCreateResult.Exception));
                            });
                    }
                });
        }

        public void DoesTableExist(string tableName, Action<CloudOperationResponse<bool>> callback)
        {
            var uri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/Tables?$filter=TableName eq '{1}'", this.context.BaseUri.ToString().TrimEnd('/'), tableName));

            this.context.BeginExecute<TableServiceSchema>(
                uri, 
                asyncResult =>
                {
                    try
                    {
                        var response = this.context.EndExecute<TableServiceSchema>(asyncResult);
                        var responseList = response as IEnumerable<TableServiceSchema>;
                        var responseStatus = response as OperationResponse;
                        callback.Invoke(new CloudOperationResponse<bool>((responseList != null ? responseList.FirstOrDefault() != null : false), (responseStatus != null ? StorageClientExceptionParser.ParseDataServiceException(responseStatus.Error) : null)));
                    }
                    catch (Exception originalException)
                    {
                        var dataServiceException = originalException.GetBaseException() as DataServiceClientException;
                        if ((dataServiceException != null) && (dataServiceException.StatusCode == 404))
                        {
                            callback.Invoke(new CloudOperationResponse<bool>(false, null));
                        }
                        else
                        {
                            callback.Invoke(new CloudOperationResponse<bool>(false, StorageClientExceptionParser.ParseDataServiceException(originalException)));
                        }
                    }
                },
                null);
        }
    }
}
