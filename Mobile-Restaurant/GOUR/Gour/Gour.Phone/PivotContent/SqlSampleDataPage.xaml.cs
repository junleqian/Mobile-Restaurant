namespace Gour.Phone.PivotContent
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Gour.Phone.ViewModel;

    public partial class SqlSampleDataPage : UserControl
    {
        public SqlSampleDataPage()
        {
            this.InitializeComponent();
            this.Loaded += this.OnSqlSampleDataPageLoaded;
        }

        public SqlSampleDataPageViewModel ViewModel
        {
            get { return this.DataContext as SqlSampleDataPageViewModel; }
            set { this.DataContext = value; }
        }

        private void OnSqlSampleDataPageLoaded(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.LoadSqlSampleData();
            }
        }
    }
}