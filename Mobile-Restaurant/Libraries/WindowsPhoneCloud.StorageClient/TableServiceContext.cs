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
    using System.Security;
    using Microsoft.Samples.Data.Services.Client;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;

    [SecuritySafeCritical]
    [CLSCompliant(false)]
    public class TableServiceContext : DataServiceContext, ITableServiceContext
    {
        public TableServiceContext(string baseAddress, IStorageCredentials credentials)
            : base(new Uri(baseAddress))
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException("baseAddress");
            }

            this.SendingRequest += this.DataContextSendingRequest;
            this.IgnoreMissingProperties = true;
            this.MergeOption = MergeOption.PreserveChanges;

            this.StorageCredentials = credentials;
        }

        public IStorageCredentials StorageCredentials { get; set; }

        public void AddTable(TableServiceSchema table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            if (string.IsNullOrWhiteSpace(table.TableName))
            {
                throw new ArgumentException("You need to provide a valid name for the new table", "table");
            }

            this.AddObject("Tables", table);
        }

        private void DataContextSendingRequest(object sender, SendingRequestEventArgs e)
        {
            this.StorageCredentials.AddAuthenticationHeadersLite(e.RequestData, e.RequestHeaders);
        }
    }
}
