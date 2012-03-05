namespace Gour.Phone.ViewModel
{
    using System.ComponentModel;

    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDesignTime
        {
            get { return DesignerProperties.IsInDesignTool; }
        }

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            var propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
