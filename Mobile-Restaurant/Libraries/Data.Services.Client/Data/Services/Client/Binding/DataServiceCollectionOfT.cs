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

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;

    #endregion Namespaces.

    public enum TrackingMode
    {
        None,
        AutoChangeTracking
    }

    public class DataServiceCollection<T> : ObservableCollection<T>, IDataServiceCollection
    {
        #region Private fields.

        private BindingObserver observer;

        private bool rootCollection;

        private DataServiceQueryContinuation<T> continuation;

        private bool trackingOnLoad;

        private Func<EntityChangedParams, bool> entityChangedCallback;

        private Func<EntityCollectionChangedParams, bool> collectionChangedCallback;

        private string entitySetName;

        private bool asyncOperationInProgress;

        #endregion Private fields.

        public DataServiceCollection()
            : this(null, null, TrackingMode.AutoChangeTracking, null, null, null)
        {
        }

        public DataServiceCollection(IEnumerable<T> items)
            : this(null, items, TrackingMode.AutoChangeTracking, null, null, null)
        {
        }

        public DataServiceCollection(IEnumerable<T> items, TrackingMode trackingMode)
            : this(null, items, trackingMode, null, null, null)
        {
        }

        public DataServiceCollection(DataServiceContext context)
            : this(context, null, TrackingMode.AutoChangeTracking, null, null, null)
        {
        }

        public DataServiceCollection(
            DataServiceContext context,
            string entitySetName,
            Func<EntityChangedParams, bool> entityChangedCallback,
            Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
            : this(context, null, TrackingMode.AutoChangeTracking, entitySetName, entityChangedCallback, collectionChangedCallback)
        {
        }

        public DataServiceCollection(
            IEnumerable<T> items,
            TrackingMode trackingMode,
            string entitySetName,
            Func<EntityChangedParams, bool> entityChangedCallback,
            Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
            : this(null, items, trackingMode, entitySetName, entityChangedCallback, collectionChangedCallback)
        {
        }

        public DataServiceCollection(
            DataServiceContext context,
            IEnumerable<T> items,
            TrackingMode trackingMode,
            string entitySetName,
            Func<EntityChangedParams, bool> entityChangedCallback,
            Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
        {
            if (trackingMode == TrackingMode.AutoChangeTracking)
            {
                if (context == null)
                {
                    if (items == null)
                    {
                        this.trackingOnLoad = true;

                        this.entitySetName = entitySetName;
                        this.entityChangedCallback = entityChangedCallback;
                        this.collectionChangedCallback = collectionChangedCallback;
                    }
                    else
                    {
                        context = DataServiceCollection<T>.GetContextFromItems(items);
                    }
                }

                if (!this.trackingOnLoad)
                {
                    if (items != null)
                    {
                        DataServiceCollection<T>.ValidateIteratorParameter(items);
                    }

                    this.StartTracking(context, items, entitySetName, entityChangedCallback, collectionChangedCallback);
                }
            }
            else if (items != null)
            {
                this.Load(items);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800", Justification = "Constructor and debug-only code can't reuse cast.")]
        internal DataServiceCollection(
            object atomMaterializer,
            DataServiceContext context,
            IEnumerable<T> items,
            TrackingMode trackingMode,
            string entitySetName,
            Func<EntityChangedParams, bool> entityChangedCallback,
            Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
            : this(
                context != null ? context : ((AtomMaterializer)atomMaterializer).Context,
                items,
                trackingMode,
                entitySetName,
                entityChangedCallback,
                collectionChangedCallback)
        {
            Debug.Assert(atomMaterializer != null, "atomMaterializer != null");
            Debug.Assert(((AtomMaterializer)atomMaterializer).Context != null, "Context != null");

            if (items != null)
            {
                ((AtomMaterializer)atomMaterializer).PropagateContinuation(items, this);
            }
        }

        #region Properties

        public DataServiceQueryContinuation<T> Continuation
        {
            get { return this.continuation; }
            set { this.continuation = value; }
        }

        internal BindingObserver Observer
        {
            get
            {
                return this.observer;
            }

            set
            {
                Debug.Assert(!this.rootCollection, "Must be a child collection to have the Observer setter called.");
                Debug.Assert(typeof(System.ComponentModel.INotifyPropertyChanged).IsAssignableFrom(typeof(T)), "The entity type must be trackable (by implementing INotifyPropertyChanged interface)");
                this.observer = value;
            }
        }

        internal bool IsTracking
        {
            get { return this.observer != null; }
        }

        #endregion

        public void Load(IEnumerable<T> items)
        {
            DataServiceCollection<T>.ValidateIteratorParameter(items);

            if (this.trackingOnLoad)
            {
                DataServiceContext context = DataServiceCollection<T>.GetContextFromItems(items);

                this.trackingOnLoad = false;

                this.StartTracking(context, items, this.entitySetName, this.entityChangedCallback, this.collectionChangedCallback);
            }
            else
            {
                this.StartLoading();
                try
                {
                    this.InternalLoadCollection(items);
                }
                finally
                {
                    this.FinishLoading();
                }
            }
        }

        public event EventHandler<LoadCompletedEventArgs> LoadCompleted;

        public void LoadAsync(Uri requestUri)
        {
            Util.CheckArgumentNull(requestUri, "requestUri");

            if (!this.IsTracking)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_OperationForTrackedOnly);
            }

            if (this.asyncOperationInProgress)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_MultipleLoadAsyncOperationsAtTheSameTime);
            }

            DataServiceContext context = this.observer.Context;
            requestUri = Util.CreateUri(context.BaseUri, requestUri);

            this.BeginLoadAsyncOperation(
                asyncCallback => context.BeginExecute<T>(requestUri, asyncCallback, null),
                asyncResult =>
                {
                    QueryOperationResponse<T> response = (QueryOperationResponse<T>)context.EndExecute<T>(asyncResult);
                    this.Load(response);
                    return response;
                });
        }

        public void LoadAsync()
        {
            if (!this.IsTracking)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_OperationForTrackedOnly);
            }

            object parent;
            string property;
            if (!this.observer.LookupParent(this, out parent, out property))
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_LoadAsyncNoParamsWithoutParentEntity);
            }

            if (this.asyncOperationInProgress)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_MultipleLoadAsyncOperationsAtTheSameTime);
            }

            this.BeginLoadAsyncOperation(
                asyncCallback => this.observer.Context.BeginLoadProperty(parent, property, asyncCallback, null),
                asyncResult => (QueryOperationResponse)this.observer.Context.EndLoadProperty(asyncResult));
        }

        public bool LoadNextPartialSetAsync()
        {
            if (!this.IsTracking)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_OperationForTrackedOnly);
            }

            if (this.asyncOperationInProgress)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_MultipleLoadAsyncOperationsAtTheSameTime);
            }

            if (this.Continuation == null)
            {
                if (this.LoadCompleted != null)
                {
                    this.LoadCompleted(this, new LoadCompletedEventArgs(null, null));
                }

                return false;
            }

            this.BeginLoadAsyncOperation(
                asyncCallback => this.observer.Context.BeginExecute(this.Continuation, asyncCallback, null),
                asyncResult =>
                {
                    QueryOperationResponse<T> response = (QueryOperationResponse<T>)this.observer.Context.EndExecute<T>(asyncResult);
                    this.Load(response);
                    return response;
                });

            return true;
        }

        public void Load(T item)
        {
            if (item == null)
            {
                throw Error.ArgumentNull("item");
            }

            this.StartLoading();
            try
            {
                if (!this.Contains(item))
                {
                    this.Add(item);
                }
            }
            finally
            {
                this.FinishLoading();
            }
        }

        public void Clear(bool stopTracking)
        {
            if (!this.IsTracking)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_OperationForTrackedOnly);
            }

            if (!stopTracking)
            {
                this.Clear();
            }
            else
            {
                Debug.Assert(this.observer.Context != null, "Must have valid context when the collection is being observed.");
                try
                {
                    this.observer.DetachBehavior = true;
                    this.Clear();
                }
                finally
                {
                    this.observer.DetachBehavior = false;
                }
            }
        }

        public void Detach()
        {
            if (!this.IsTracking)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_OperationForTrackedOnly);
            }

            if (!this.rootCollection)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_CannotStopTrackingChildCollection);
            }

            this.observer.StopTracking();
            this.observer = null;

            this.rootCollection = false;
        }

        public new void Add(T item)
        {
            if (this.IsTracking)
            {
                INotifyPropertyChanged notify = item as INotifyPropertyChanged;
                if (notify == null)
                {
                    throw new InvalidOperationException(Strings.DataBinding_NotifyPropertyChangedNotImpl(item.GetType()));
                }
            }

            base.Add(item);
        }

        protected override void InsertItem(int index, T item)
        {
            if (this.trackingOnLoad)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_InsertIntoTrackedButNotLoadedCollection);
            }

            base.InsertItem(index, item);
        }

        CollectionState IDataServiceCollection.SaveState(
            DataServiceContext context, Dictionary<EntityDescriptor, Guid> entityDescriptorToId)
        {
            if (this.observer == null)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_OperationForTrackedOnly);
            }

            if (this.observer.Context != context)
            {
                throw new InvalidOperationException(Strings.DataServiceState_CollectionNotInContext);
            }

            var state = new CollectionState();

            state.EntitySetName = this.entitySetName;
            state.EntityTypeName = typeof(T).AssemblyQualifiedName;
            state.RootCollection = this.rootCollection;

            state.DescriptorIds = new List<Guid>(this.Count);
            foreach (object entity in this)
            {
                EntityDescriptor entityDescriptor = context.GetEntityDescriptor(entity);
                Guid id = entityDescriptorToId[entityDescriptor];
                state.DescriptorIds.Add(id);
            }

            return state;
        }

        internal static DataServiceCollection<T> RestoreState(
            CollectionState state, DataServiceContext context, Dictionary<Guid, EntityDescriptor> idToEntityDescriptor)
        {
            var items = new List<T>(state.DescriptorIds.Count);
            foreach (Guid id in state.DescriptorIds)
            {
                EntityDescriptor entityDescriptor = idToEntityDescriptor[id];
                items.Add((T)entityDescriptor.Entity);
            }

            DataServiceCollection<T> collection = new DataServiceCollection<T>(
                context, items, TrackingMode.AutoChangeTracking, state.EntitySetName, null, null);

            collection.rootCollection = state.RootCollection;

            return collection;
        }

        private static void ValidateIteratorParameter(IEnumerable<T> items)
        {
            Util.CheckArgumentNull(items, "items");
        }

        private static DataServiceContext GetContextFromItems(IEnumerable<T> items)
        {
            Debug.Assert(items != null, "items != null");

            QueryOperationResponse queryOperationResponse = items as QueryOperationResponse;
            if (queryOperationResponse != null)
            {
                Debug.Assert(queryOperationResponse.Results != null, "Got QueryOperationResponse without valid results.");
                DataServiceContext context = queryOperationResponse.Results.Context;
                Debug.Assert(context != null, "Materializer must always have valid context.");
                return context;
            }

            throw new ArgumentException(Strings.DataServiceCollection_CannotDetermineContextFromItems);
        }

        private void InternalLoadCollection(IEnumerable<T> items)
        {
            Debug.Assert(items != null, "items != null");

            foreach (T item in items)
            {
                if (!this.Contains(item))
                {
                    this.Add(item);
                }
            }

            QueryOperationResponse<T> response = items as QueryOperationResponse<T>;
            if (response != null)
            {
                this.continuation = response.GetContinuation();
            }
            else
            {
                this.continuation = null;
            }
        }

        private void StartLoading()
        {
            if (this.IsTracking)
            {
                if (this.observer.Context == null)
                {
                    throw new InvalidOperationException(Strings.DataServiceCollection_LoadRequiresTargetCollectionObserved);
                }

                this.observer.AttachBehavior = true;
            }
        }

        private void FinishLoading()
        {
            if (this.IsTracking)
            {
                this.observer.AttachBehavior = false;
            }
        }

        private void StartTracking(
            DataServiceContext context,
            IEnumerable<T> items,
            string entitySet,
            Func<EntityChangedParams, bool> entityChanged,
            Func<EntityCollectionChangedParams, bool> collectionChanged)
        {
            Debug.Assert(context != null, "Must have a valid context to initialize.");
            Debug.Assert(this.observer == null, "Must have no observer which implies Initialize should only be called once.");

            if (items != null)
            {
                this.InternalLoadCollection(items);
            }

            this.observer = new BindingObserver(context, entityChanged, collectionChanged);

            this.observer.StartTracking(this, entitySet);

            this.rootCollection = true;
        }

        private void BeginLoadAsyncOperation(
            Func<AsyncCallback, IAsyncResult> beginCall,
            Func<IAsyncResult, QueryOperationResponse> endCall)
        {
            Debug.Assert(!this.asyncOperationInProgress, "Trying to start a new LoadAsync while another is still in progress. We should have thrown.");

            this.asyncOperationInProgress = true;
            try
            {
                IAsyncResult asyncResult = beginCall(
                    ar => System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            QueryOperationResponse result = endCall(ar);
                            this.asyncOperationInProgress = false;
                            if (this.LoadCompleted != null)
                            {
                                this.LoadCompleted(this, new LoadCompletedEventArgs(result, null));
                            }
                        }
                        catch (Exception ex)
                        {
                            this.asyncOperationInProgress = false;
                            if (this.LoadCompleted != null)
                            {
                                this.LoadCompleted(this, new LoadCompletedEventArgs(null, ex));
                            }
                        }
                    }));
            }
            catch (Exception)
            {
                this.asyncOperationInProgress = false;
                throw;
            }
        }
    }

    internal interface IDataServiceCollection
    {
        CollectionState SaveState(DataServiceContext context, Dictionary<EntityDescriptor, Guid> entityDescriptorToId);
    }
}
