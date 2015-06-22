using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ripper
{
    [Export(typeof(ShowViewModel))]
    public class ShowViewModel : Screen, IHaveDisplayName
    {
        private string mDisplayName = "Enter Scheduled Recording Info";
        private Show mShow;

        public ShowViewModel()
        {
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
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }
    }
}
