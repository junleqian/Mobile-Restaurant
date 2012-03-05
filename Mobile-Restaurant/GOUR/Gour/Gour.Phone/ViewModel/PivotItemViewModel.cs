namespace Gour.Phone.ViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Phone.Shell;

    public abstract class PivotItemViewModel : BaseViewModel
    {
        public void UpdateApplicationBarButtons(IApplicationBar applicationBar, string[] preserveButtons)
        {
            // Create a list of the buttons to preserve in the application bar.
            var preserveButtonsList = new List<IApplicationBarIconButton>();
            foreach (IApplicationBarIconButton button in applicationBar.Buttons)
            {
                if (preserveButtons.Contains(button.Text))
                {
                    preserveButtonsList.Add(button);
                }
            }

            // Clear the current buttons in the application bar.
            applicationBar.Buttons.Clear();

            // Add application bar buttons specific of the pivot item.
            this.PopulateApplicationBarButtons(applicationBar);

            // Add the preserved application bar buttons.
            foreach (var button in preserveButtonsList)
            {
                applicationBar.Buttons.Add(button);
            }
        }

        protected abstract void PopulateApplicationBarButtons(IApplicationBar applicationBar);
    }
}
