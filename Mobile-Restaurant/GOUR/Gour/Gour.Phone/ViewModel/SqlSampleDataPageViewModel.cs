namespace Gour.Phone.ViewModel
{
    using System;
    using System.Data.Services.Client;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Threading;
    using Microsoft.Phone.Shell;
    using Gour.Phone.Models;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient;

    public class SqlSampleDataPageViewModel : PivotItemViewModel
    {
        private const string IconsRootUri = "/Toolkit.Content/";
        private const string SqlSampleDataEntityName = "SqlSampleData";

        private readonly Dispatcher dispatcher;

        private bool isListing = false;
        private bool hasResults = true;
        private string message = string.Empty;

        public SqlSampleDataPageViewModel()
            : this(Deployment.Current.Dispatcher, App.CloudClientFactory.ResolveOdataServiceContext())
        {
        }

        public SqlSampleDataPageViewModel(Dispatcher dispatcher, DataServiceContext odataServiceContext)
        {
            this.dispatcher = dispatcher;
            this.Context = odataServiceContext;
            this.Items = new DataServiceCollection<SqlSampleData>(odataServiceContext);
            this.Items.LoadCompleted += this.OnLoadCompleted;
        }

        public DataServiceCollection<SqlSampleData> Items { get; set; }

        public DataServiceContext Context { get; private set; }

        public bool IsListing
        {
            get
            {
                return this.isListing;
            }

            set
            {
                if (this.isListing != value)
                {
                    this.isListing = value;
                    this.NotifyPropertyChanged("IsListing");
                }
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.NotifyPropertyChanged("Message");
                }
            }
        }

        public bool HasResults
        {
            get
            {
                return this.hasResults;
            }

            set
            {
                if (this.hasResults != value)
                {
                    this.hasResults = value;
                    this.NotifyPropertyChanged("HasResults");
                }
            }
        }

        public void LoadSqlSampleData()
        {
            if (!this.IsListing)
            {
                // Adding an incremental seed to avoid a cached response.
                var tableUri = new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}/{1}?incrementalSeed={2}",
                        this.Context.BaseUri,
                        SqlSampleDataEntityName,
                        DateTime.UtcNow.Ticks),
                    UriKind.Absolute);

                this.IsListing = true;
                this.Message = "Loading...";

                try
                {
                    this.Items.Clear();
                    this.Items.LoadAsync(tableUri);
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                    this.IsListing = false;
                    this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
        }

        protected override void PopulateApplicationBarButtons(IApplicationBar applicationBar)
        {
            var refreshButton = new ApplicationBarIconButton(new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}", IconsRootUri, "appbar.refresh.rest.png"), UriKind.Relative)) { Text = "refresh" };
            refreshButton.Click += (s, e) => this.LoadSqlSampleData();

            applicationBar.Buttons.Add(refreshButton);
        }

        protected virtual void OnLoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.DispatchResult("The operation has been cancelled.");
            }
            else if (e.Error != null)
            {
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(e.Error).Message;
                this.DispatchResult(string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage));
            }
            else if (this.Items.Continuation != null)
            {
                try
                {
                    this.Items.LoadNextPartialSetAsync();
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                    this.IsListing = false;
                    this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
            else
            {
                this.DispatchResult();
            }
        }

        protected virtual void DispatchResult(string message = "")
        {
            this.dispatcher.BeginInvoke(
                () =>
                {
                    this.Message = message;
                    this.IsListing = false;
                });
        }
    }
}
