using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Quartz;
using Quartz.Impl;
using log4net;

namespace ripper
{
    /// <summary>
    /// Schedules Quartz.Net jobs to record shows. Keeps Quartz schedule up to date with IShowList.
    /// </summary>
    class ShowRecorder
    {
        private readonly IShowList mShowList;
        private IScheduler mScheduler;
        private bool mRunning;
        private static ILog mLogger = LogManager.GetLogger(typeof(ShowRecorder));

        public bool Running
        {
            get
            {
                return mRunning;
            }
            set
            {
                mRunning = value;
            }
        }

        public ShowRecorder(IShowList showList)
        {
            mShowList = showList;
            mScheduler = StdSchedulerFactory.GetDefaultScheduler();

            AddAllShows();
            SetupHandlers();
        }

        public void Start()
        {
            mLogger.Info("Starting Quartz scheduler..");
            mScheduler.Start();

            mRunning = true;
        }

        public void Shutdown(bool waitForJobsToComplete)
        {
            mLogger.Info("Shutting down Quartz scheduler..");
            mScheduler.Shutdown(waitForJobsToComplete);
        }

        private void SetupHandlers()
        {
            mShowList.Shows.CollectionChanged += OnShowsChanged;
        }

        private void OnShowsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var old = args.OldItems != null ? args.OldItems.Cast<Show>().ToList() : new List<Show>();
            var nnew = args.NewItems != null ? args.NewItems.Cast<Show>().ToList() : new List<Show>();
            var added = nnew.Except(old).ToList();
            var removed = old.Except(nnew).ToList();

            mLogger.InfoFormat("{0} show(s) added, {1} show(s) removed", added.Count, removed.Count);
            added.ForEach(AddJob);
            removed.ForEach(RemoveJob);
        }

        private void AddAllShows()
        {
            mShowList.Shows.ToList().ForEach(AddJob);
        }

        private void AddJob(Show show)
        {
            mLogger.InfoFormat("ShowRecorder.AddJob {0} {1}", show.Id, show.ShowName);
            var job = JobBuilder.Create<RecordShowJob>()
                                       .WithIdentity(show.JobId)
                                       .Build();

            job.JobDataMap.Put(Show.Job_Data_Key, show);

            var minutes = show.StartTime % 100;
            var hours = show.StartTime / 100;

            var trigger = TriggerBuilder.Create()
                                        .WithIdentity(show.TriggerId)
                                        .WithSchedule(
                                            CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(show.DayOfWeekCron,
                                                                                            hours, minutes))
                                        .ForJob(show.JobId)
                                        .Build();

            mScheduler.ScheduleJob(job, trigger);

            show.PropertyChanged += ShowChanged;
        }

        private void ShowChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var show = sender as Show;

            if (show == null)
            {
                return;
            }

            UpdateJob(show);
        }

        private void RemoveJob(Show show)
        {
            mLogger.InfoFormat("ShowRecorder.RemoveJob {0} {1}", show.Id, show.ShowName);
            mScheduler.DeleteJob(new JobKey(show.JobId));
            show.PropertyChanged -= ShowChanged;
        }

        private void UpdateJob(Show show)
        {
            RemoveJob(show);
            AddJob(show);
        }
    }
}
