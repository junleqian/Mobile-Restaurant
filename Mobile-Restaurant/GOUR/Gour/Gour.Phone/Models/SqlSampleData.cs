namespace Gour.Phone.Models
{
    using System;
    using System.ComponentModel;
    using System.Data.Services.Common;

    [EntitySetAttribute("SqlSampleData")]
    [DataServiceKeyAttribute("Id")]
    public class SqlSampleData : INotifyPropertyChanged
    {
        private int id;
        private string userId;
        private string title;
        private DateTime date;
        private bool isPublic;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
                this.OnPropertyChanged("Id");
            }
        }

        public string UserId
        {
            get
            {
                return this.userId;
            }

            set
            {
                this.userId = value;
                this.OnPropertyChanged("UserId");
            }
        }

        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;
                this.OnPropertyChanged("Title");
            }
        }

        public DateTime Date
        {
            get
            {
                return this.date;
            }

            set
            {
                this.date = value;
                this.OnPropertyChanged("Date");
            }
        }

        public bool IsPublic
        {
            get
            {
                return this.isPublic;
            }

            set
            {
                this.isPublic = value;
                this.OnPropertyChanged("IsPublic");
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
