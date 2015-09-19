using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.Validation;

namespace ShoutcastDvr
{
    [Export(typeof(ShowViewModel))]
    public class ShowViewModel : ValidatingScreen, IHaveDisplayName
    {
        private string mDisplayName = "Enter Scheduled Recording Info";
        private Show mShow;

        public ShowViewModel()
        {
            AddValidationRule(() => Show.ShowName).Condition(() => string.IsNullOrWhiteSpace(Show.ShowName)).Message("Please enter show name");
            AddValidationRule(() => Show.Duration)
                .Condition(() => Show.Duration <= 0)
                .Message("Please set show duration greater than zero");
            AddValidationRule(() => Show.Url)
                .Condition(() => !Uri.IsWellFormedUriString(Show.Url, UriKind.Absolute))
                .Message("Please enter a valid shoutcast url for the show");
            mShow = new Show();
        }

        public List<string> DayOfWeek
        {
            get
            {
                return new List<string> {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"};
            }
        }

        public string DisplayName
        {
            get
            {
                return mDisplayName;
            }
            set
            {
                mDisplayName = value;
                NotifyOfPropertyChange(() => DisplayName);
            }
        }

        public Show Show
        {
            get
            {
                return mShow;
            }
            set
            {
                mShow = value;
            }
        }

        public string SelectedDayOfWeek
        {
            get
            {
                return Show.DayOfWeek;
            }
            set
            {
                Show.DayOfWeek = value;
            }
        }

        public void Accept()
        {
            var result = Validate();

            if (result != "")
            {
                MessageBox.Show(result, "Error");
            }
            else
            {
                TryClose(true);   
            }
        }

        public void Cancel()
        {
            TryClose(false);
        }
    }
}
