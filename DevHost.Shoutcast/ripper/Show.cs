using System;
using Caliburn.Micro;

namespace ripper
{
    public class Show : PropertyChangedBase 
    {
        private readonly string mId = Guid.NewGuid().ToString();
        private DayOfWeek mDayOfWeek;
        private string mShowName;
        private string mUrl;
        private int mStartTime;
        private int mDuration;

        public static string Job_Data_Key = "show";

        public string Id
        {
            get
            {
                return mId;
            }
        }

        public string JobId
        {
            get
            {
                return string.Format("job-{0}", Id);
            }
        }

        public string TriggerId
        {
            get
            {
                return string.Format("trigger-{0}", Id);
            }
        }

        public string ShowName
        {
            get
            {
                return mShowName;
            }
            set
            {
                mShowName = value;
                NotifyOfPropertyChange(() => ShowName);
            }
        }

        public string Url
        {
            get
            {
                return mUrl;
            }
            set
            {
                mUrl = value;
                NotifyOfPropertyChange(() => Url);
            }
        }

        public string DayOfWeek
        {
            get
            {
                return mDayOfWeek.ToString();
            }
            set
            {
                Enum.TryParse(value, true, out mDayOfWeek);
                NotifyOfPropertyChange(() => DayOfWeek);
                NotifyOfPropertyChange(() => DayOfWeekCron);
            }
        }

        public DayOfWeek DayOfWeekCron
        {
            get
            {
                return mDayOfWeek;
            }
        }

        public int StartTime
        {
            get
            {
                return mStartTime;
            }
            set
            {
                mStartTime = value;
                NotifyOfPropertyChange(() => StartTime);
            }
        }

        public int Duration
        {
            get
            {
                return mDuration;
            }
            set
            {
                mDuration = value;
                NotifyOfPropertyChange(() => Duration);
            }
        }

        public Show ShallowCopy()
        {
            return (Show)MemberwiseClone();
        }
    }
}
