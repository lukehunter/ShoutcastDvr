using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using ShoutcastDvr.Controller;
using ShoutcastDvr.Properties;
using Size = System.Windows.Size;

namespace ShoutcastDvr
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Screen, IShell
    {
        private readonly IWindowManager mWindowManager;
        private readonly IShowList mShowList;
        private string mAppName;
        private readonly ShowRecorder mShowRecorder;

        public override string DisplayName
        {
            get
            {
                return mAppName;
            }
            set
            {
                mAppName = value;
            }
        }

        public BindableCollection<Show> Shows
        {
            get
            {
                return mShowList.Shows;
            }
            set
            {
                throw new InvalidOperationException("No setter available through IShowList");
            }
        }

        [ImportingConstructor]
        public ShellViewModel(IWindowManager windowManager)
        {
            mWindowManager = windowManager;
            mAppName = "ShoutcastDvr";
            mShowList =
                new JsonShowList(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                              "scheduled_recordings.json"));
            mShowRecorder = new ShowRecorder(mShowList);
        }

        public ShellViewModel()
        {
            mShowList = new DebugShowList();
        }

        public void Add()
        {
            var vm = new ShowViewModel();

            var result = mWindowManager.ShowDialog(vm);
            bool bresult;

            if (!result.HasValue)
            {
                bresult = false;
            }
            else
            {
                bresult = (bool)result;
            }

            if (!bresult) return;

            Shows.Add(vm.Show);
            mShowList.Save();
        }

        public void Edit(Show show)
        {
            var vm = new ShowViewModel {Show = show.ShallowCopy()};

            var settings = new Dictionary<string, object>()
            {
                {"MinHeight", 350},
                {"MinWidth", 400}
            };

            var result = mWindowManager.ShowDialog(vm, null, settings);
            bool bresult;

            if (!result.HasValue)
            {
                bresult = false;
            }
            else
            {
                bresult = (bool)result;
            }

            if (!bresult)
            {
                return;
            }

            var updated = Shows.First(sr => sr.Id == vm.Show.Id);

            if (updated == null)
            {
                MessageBox.Show("Scheduled recording no longer exists");
                return;
            }

            updated.ShowName = vm.Show.ShowName;
            updated.DayOfWeek = vm.Show.DayOfWeek;
            updated.Duration = vm.Show.Duration;
            updated.StartTime = vm.Show.StartTime;
            updated.Url = vm.Show.Url;

            mShowList.Save();
        }

        public void Remove(Show child)
        {
            Shows.Remove(child);
            mShowList.Save();
        }

        public bool CanStart
        {
            get
            {
                return !mShowRecorder.Running;
            }
        }

        public void Start()
        {
            mShowRecorder.Start();
            NotifyOfPropertyChange(() => CanStart);
        }

        public override void CanClose(Action<bool> callback)
        {
            mShowList.Save();
            mShowRecorder.Shutdown(false);

            var wm = mWindowManager as AppWindowManager;

            if (wm != null)
            {
                if (wm.Window.WindowState == WindowState.Normal)
                {
                    Settings.Default.WindowSize = new Size(wm.Window.Width, wm.Window.Height);
                }
                else
                {
                    Settings.Default.WindowSize = wm.Window.RestoreBounds.Size;
                }
            }

            // Save settings
            Settings.Default.Save();

            callback(true);
        }
    }
}
