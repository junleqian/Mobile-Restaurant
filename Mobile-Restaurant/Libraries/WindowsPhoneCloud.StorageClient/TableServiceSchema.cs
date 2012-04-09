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
    using System.ComponentModel;
    using Microsoft.Samples.Data.Services.Common;

    [DataServiceEntity]
    [EntitySet("Tables")]
    [DataServiceKey("TableName")]
    public class TableServiceSchema : INotifyPropertyChanged
    {
        private string tableName;

        public TableServiceSchema()
        {
        }

        public TableServiceSchema(string tableName)
        {
            this.tableName = tableName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string TableName
        {
            get
            {
                return this.tableName;
            }

            set
            {
                this.tableName = value;
                this.OnPropertyChanged("TableName");
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
