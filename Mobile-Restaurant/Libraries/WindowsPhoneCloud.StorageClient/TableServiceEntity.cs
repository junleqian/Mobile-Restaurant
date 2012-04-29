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
    using System.ComponentModel;
    using Microsoft.Samples.Data.Services.Common;

    [DataServiceKey("PartitionKey", "RowKey")]
    public abstract class TableServiceEntity : INotifyPropertyChanged
    {
        private string partitionKey;
        private string rowKey;
        private DateTime timestamp;

        public TableServiceEntity()
        {
        }

        public TableServiceEntity(string partitionKey, string rowKey)
        {
            this.partitionKey = partitionKey;
            this.rowKey = rowKey;
            this.timestamp = DateTime.UtcNow;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual string PartitionKey
        {
            get
            {
                return this.partitionKey;
            }

            set
            {
                this.partitionKey = value;
                this.OnPropertyChanged("PartitionKey");
            }
        }

        public virtual string RowKey
        {
            get
            {
                return this.rowKey;
            }

            set
            {
                this.rowKey = value;
                this.OnPropertyChanged("RowKey");
            }
        }

        public DateTime Timestamp
        {
            get
            {
                return this.timestamp;
            }

            set
            {
                this.timestamp = value;
                this.OnPropertyChanged("Timestamp");
            }
        }

        protected virtual void OnPropertyChanged(string changedProperty)
        {
            var propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(changedProperty));
            }
        }
    }
}
