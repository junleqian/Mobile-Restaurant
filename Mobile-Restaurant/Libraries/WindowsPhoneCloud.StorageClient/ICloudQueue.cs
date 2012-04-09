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

    public interface ICloudQueue
    {
        string Name { get; set; }

        Uri Uri { get; set; }

        void AddMessage(CloudQueueMessage message, Action<CloudOperationResponse<bool>> callback);

        void Clear(Action<CloudOperationResponse<bool>> callback);

        void Create(Action<CloudOperationResponse<bool>> callback);

        void CreateIfNotExist(Action<CloudOperationResponse<bool>> callback);

        void Delete(Action<CloudOperationResponse<bool>> callback);

        void DeleteMessage(CloudQueueMessage message, Action<CloudOperationResponse<bool>> callback);

        void Exists(Action<CloudOperationResponse<bool>> callback);

        void GetMessage(Action<CloudOperationResponse<CloudQueueMessage>> callback);

        void GetMessage(TimeSpan visibilityTimeout, Action<CloudOperationResponse<CloudQueueMessage>> callback);

        void GetMessages(int messageCount, Action<CloudOperationResponse<IEnumerable<CloudQueueMessage>>> callback);

        void GetMessages(int messageCount, TimeSpan visibilityTimeout, Action<CloudOperationResponse<IEnumerable<CloudQueueMessage>>> callback);

        void PeekMessage(Action<CloudOperationResponse<CloudQueueMessage>> callback);

        void PeekMessages(int messageCount, Action<CloudOperationResponse<IEnumerable<CloudQueueMessage>>> callback);
    }
}
