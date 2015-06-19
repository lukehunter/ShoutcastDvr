using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ripper
{
    [Export(typeof(ScheduledRecordingViewModel))]
    public class ScheduledRecordingViewModel : Screen, IHaveDisplayName
    {
        private string mDisplayName = "Enter Scheduled Recording Info";
        private ScheduledRecording mScheduledRecording = new ScheduledRecording();

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

        public ScheduledRecording ScheduledRecording
        {
            get
            {
                return mScheduledRecording;
            }
            set
            {
                mScheduledRecording = value;
            }
        }

        public string SelectedDayOfWeek
        {
            get
            {
                return ScheduledRecording.DayOfWeek;
            }
            set
            {
                ScheduledRecording.DayOfWeek = value;
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
