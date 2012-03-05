namespace Gour.Phone.ViewModel
{
    public class MainPageViewModel : BaseViewModel
    {
        private NotificationsPageViewModel notificationsPageViewModel;
        private SqlSampleDataPageViewModel sqlSampleDataPageViewModel;

        public NotificationsPageViewModel NotificationsViewModel
        {
            get
            {
                if (this.notificationsPageViewModel == null)
                {
                    this.notificationsPageViewModel = new NotificationsPageViewModel();
                }

                return this.notificationsPageViewModel;
            }

            set
            {
                this.notificationsPageViewModel = value;
            }
        }

        public SqlSampleDataPageViewModel SqlSampleDataPageViewModel
        {
            get
            {
                if (this.sqlSampleDataPageViewModel == null)
                {
                    this.sqlSampleDataPageViewModel = new SqlSampleDataPageViewModel();
                }

                return this.sqlSampleDataPageViewModel;
            }

            set
            {
                this.sqlSampleDataPageViewModel = value;
            }
        }
    }
}
