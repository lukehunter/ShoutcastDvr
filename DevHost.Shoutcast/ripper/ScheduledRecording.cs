using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ripper
{
    public class ScheduledRecording
    {
        private string mId = Guid.NewGuid().ToString();

        public string Id
        {
            get
            {
                return mId;
            }
            set
            {
                mId = value;
            }
        }

        public string ShowName
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public string DayOfWeek
        {
            get;
            set;
        }

        public int StartTime
        {
            get;
            set;
        }

        public int Duration
        {
            get;
            set;
        }

        public ScheduledRecording ShallowCopy()
        {
            return (ScheduledRecording)MemberwiseClone();
        }
    }
}
