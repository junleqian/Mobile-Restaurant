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
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Security;
    using Microsoft.Samples.Data.Services.Client;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;

    [SecuritySafeCritical]
    [CLSCompliant(false)]
    public interface ITableServiceContext
    {
        event EventHandler<ReadingWritingEntityEventArgs> ReadingEntity;

        event EventHandler<ReadingWritingEntityEventArgs> WritingEntity;

        event EventHandler<SendingRequestEventArgs> SendingRequest;

        IStorageCredentials StorageCredentials { get; set; }

        Uri BaseUri { get; }

        bool ApplyingChanges { get; }

        string DataNamespace { get; set; }

        ReadOnlyCollection<EntityDescriptor> Entities { get; }

        bool IgnoreMissingProperties { get; set; }

        bool IgnoreResourceNotFoundException { get; set; }

        ReadOnlyCollection<LinkDescriptor> Links { get; }

        MergeOption MergeOption { get; set; }

        Func<Type, string> ResolveName { get; set; }

        Func<string, Type> ResolveType { get; set; }

        SaveChangesOptions SaveChangesDefaultOptions { get; set; }

        Uri TypeScheme { get; set; }

        bool UsePostTunneling { get; set; }

        void AddTable(TableServiceSchema table);

        IAsyncResult BeginSaveChanges(AsyncCallback callback, object state);

        IAsyncResult BeginSaveChanges(SaveChangesOptions options, AsyncCallback callback, object state);

        DataServiceResponse EndSaveChanges(IAsyncResult asyncResult);

        IAsyncResult BeginExecute<TElement>(Uri requestUri, AsyncCallback callback, object state);

        IAsyncResult BeginExecute<T>(DataServiceQueryContinuation<T> continuation, AsyncCallback callback, object state);

        IEnumerable<TElement> EndExecute<TElement>(IAsyncResult asyncResult);

        void AddObject(string entitySetName, object entity);

        void UpdateObject(object entity);

        void DeleteObject(object entity);

        void AddRelatedObject(object source, string sourceProperty, object target);

        void AttachTo(string entitySetName, object entity);

        void AttachTo(string entitySetName, object entity, string etag);
        
        IAsyncResult BeginExecuteBatch(AsyncCallback callback, object state, params DataServiceRequest[] queries);

        IAsyncResult BeginGetReadStream(object entity, DataServiceRequestArgs args, AsyncCallback callback, object state);

        IAsyncResult BeginLoadProperty(object entity, string propertyName, AsyncCallback callback, object state);

        IAsyncResult BeginLoadProperty(object entity, string propertyName, DataServiceQueryContinuation continuation, AsyncCallback callback, object state);

        IAsyncResult BeginLoadProperty(object entity, string propertyName, Uri nextLinkUri, AsyncCallback callback, object state);

        void CancelRequest(IAsyncResult asyncResult);

        bool Detach(object entity);

        DataServiceResponse EndExecuteBatch(IAsyncResult asyncResult);

        DataServiceStreamResponse EndGetReadStream(IAsyncResult asyncResult);

        QueryOperationResponse EndLoadProperty(IAsyncResult asyncResult);

        EntityDescriptor GetEntityDescriptor(object entity);

        Uri GetMetadataUri();

        Uri GetReadStreamUri(object entity);

        void SetSaveStream(object entity, Stream stream, bool closeStream, DataServiceRequestArgs args);

        void SetSaveStream(object entity, Stream stream, bool closeStream, string contentType, string slug);

        bool TryGetEntity<TEntity>(Uri identity, out TEntity entity) where TEntity : class;

        bool TryGetUri(object entity, out Uri identity);
    }
}
