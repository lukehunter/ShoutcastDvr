using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using System.Web.Script.Serialization;

namespace ripper
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Screen, IShell
    {
        private readonly IWindowManager mWindowManager;
        private BindableCollection<ScheduledRecording> mScheduledRecordings = new BindableCollection<ScheduledRecording>();
        private readonly string mSavedDataFile =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "scheduled_recordings.json");

        public BindableCollection<ScheduledRecording> ScheduledRecordings
        {
            get
            {
                return mScheduledRecordings;
            }
            private set
            {
                mScheduledRecordings = value;
            }
        }

        [ImportingConstructor]
        public ShellViewModel(IWindowManager windowManager)
        {
            mWindowManager = windowManager;
            Load();
        }

        public ShellViewModel()
        {
            ScheduledRecordings = new BindableCollection<ScheduledRecording>
            {
                new ScheduledRecording() { ShowName = "palette swapped" },
                new ScheduledRecording() { ShowName = "othermusic" },
                new ScheduledRecording() { ShowName = "blah"}
            };
        }

        public void Add()
        {
            //ScheduledRecordings.Add(new ScheduledRecording() {ShowName = Guid.NewGuid().ToString()});
            var vm = new ScheduledRecordingViewModel();

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
            
            if (bresult)
            {
                ScheduledRecordings.Add(vm.ScheduledRecording);
            }

        }

        public void Edit(ScheduledRecording child)
        {
            var vm = new ScheduledRecordingViewModel();
            vm.ScheduledRecording = child.ShallowCopy();

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

            if (!bresult)
            {
                return;
            }

            var updated = ScheduledRecordings.First(sr => sr.Id == vm.ScheduledRecording.Id);

            if (updated == null)
            {
                MessageBox.Show("Scheduled recording no longer exists");
                return;
            }

            updated.ShowName = vm.ScheduledRecording.ShowName;
            updated.DayOfWeek = vm.ScheduledRecording.DayOfWeek;
            updated.Duration = vm.ScheduledRecording.Duration;
            updated.StartTime = vm.ScheduledRecording.StartTime;
            updated.Url = vm.ScheduledRecording.Url;
        }

        public void Remove(ScheduledRecording child)
        {
            ScheduledRecordings.Remove(child);
        }

        public bool CanStream(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }

        public void Stream(string url)
        {
            MessageBox.Show(string.Format("Recording {0}", url));
        }

        public override void CanClose(Action<bool> callback)
        {
            Save(mSavedDataFile, ScheduledRecordings);
            callback(true);
        }

        private static void Save(string filename, object obj)
        {
            var json = new JavaScriptSerializer().Serialize(obj);
            File.WriteAllText(filename, json);
        }

        private void Load()
        {
            if (!File.Exists(mSavedDataFile))
            {
                return;
            }

            var json = File.ReadAllText(mSavedDataFile);
            ScheduledRecordings = new JavaScriptSerializer().Deserialize<BindableCollection<ScheduledRecording>>(json);
        }
    }
}
