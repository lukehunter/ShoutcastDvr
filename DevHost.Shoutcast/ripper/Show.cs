using System;
using System.Diagnostics;
using System.Timers;
using Caliburn.Micro;

namespace ripper
{
    public class Show : PropertyChangedBase, IDisposable, IHandle<ShowRecordingStatusChangedEvent>
    {
        private readonly string mId = Guid.NewGuid().ToString();
        private DayOfWeek mDayOfWeek;
        private string mShowName;
        private string mUrl;
        private int mStartTime;
        private int mDuration;
        private Timer mTimer;

        public static string Job_Data_Key = "show";
        private bool mIsRecording;

        public Show()
        {
            mTimer = new Timer(45000);
            mTimer.Elapsed += OnTimerTick;
            mTimer.Enabled = true;

            if (EvtAgg.Current == null)
            {
                return;
            }

            EvtAgg.Current.Subscribe(this);
        }

        private void OnTimerTick(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            NotifyOfPropertyChange(() => NextRun);
        }

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

        public bool IsRecording
        {
            get
            {
                return mIsRecording;
            }
            set
            {
                mIsRecording = value;
                NotifyOfPropertyChange(() => IsRecording);
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
                NotifyOfPropertyChange(() => NextRun);
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
                NotifyOfPropertyChange(() => NextRun);
                NotifyOfPropertyChange(() => StartText);
            }
        }

        public string StartText
        {
            get
            {
                var ampm = StartTime >= 1200 ? "pm" : "am";
                var hour = (StartTime / 100) % 12;

                if (hour == 0)
                {
                    hour = 12;
                }

                return string.Format("{0}:{1:D2} {2}", hour, StartTime % 100, ampm);
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
                NotifyOfPropertyChange(() => NextRun);
            }
        }

        public string NextRun
        {
            get
            {
                var dateTime = DateTime.Now;
                var timeSpan = TimeSpan.FromMinutes(1);
                var projTime = dateTime + timeSpan;
                TimeSpan untilNext;

                if (dateTime.DayOfWeek == mDayOfWeek)
                {
                    if (MilFromDt(dateTime) > StartTime)
                    {
                        untilNext = TimeSpan.FromDays(6) +
                                    TimeSpan.FromMinutes(24 * 60 - MilDiff(StartTime, MilFromDt(dateTime)));
                    }
                    else
                    {
                        untilNext = TimeSpan.FromMinutes(MilDiff(MilFromDt(dateTime), StartTime));
                    }
                }
                else
                {
                    // TODO LAH 2015-06-21: ugly poor performing hack
                    while (MilFromDt(projTime) < StartTime || projTime.DayOfWeek != mDayOfWeek)
                    {
                        timeSpan = timeSpan.Add(TimeSpan.FromMinutes(1));
                        projTime = dateTime + timeSpan;
                    }

                    untilNext = timeSpan;
                }

                string untilNextStr = untilNext.TotalMinutes <= 0
                                          ? string.Format("Recording: {0}m remaining", untilNext.TotalMinutes + Duration)
                                          : string.Format("{0}d {1}h {2}m", untilNext.Days, untilNext.Hours, untilNext.Minutes);

                Debug.WriteLine("self check {0} {1} {2}", DayOfWeek, StartTime, DateTime.Now + untilNext );


                return untilNextStr;
            }
        }

        private static int MilDiff(int first, int second)
        {
            int minFirst = first / 100 * 60 + first % 100;
            int minSecond = second / 100 * 60 + second % 100;

            return minSecond - minFirst;
        }

        private static int MilFromDt(DateTime ts)
        {
            return ts.Hour * 100 + ts.Minute;
        }

        public Show ShallowCopy()
        {
            return (Show)MemberwiseClone();
        }

        public void Dispose()
        {
            mTimer.Enabled = false;
            mTimer.Dispose();
        }

        public void Handle(ShowRecordingStatusChangedEvent message)
        {
            if (message.ShowId == Id)
            {
                IsRecording = message.IsRecording;
            }
        }
    }
}
