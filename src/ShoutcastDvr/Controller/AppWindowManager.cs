using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ShoutcastDvr.Properties;

namespace ShoutcastDvr.Controller
{
    class AppWindowManager : WindowManager, IHandle<ShowRecordingStatusChangedEvent>
    {
        public Window ShellWindow;
        private Dictionary<string, bool> mIsRecording = new Dictionary<string, bool>();

        public AppWindowManager()
        {
            if( EvtAgg.Current == null )
            {
                return;
            }

            EvtAgg.Current.Subscribe(this);
        }

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            var window = base.EnsureWindow(model, view, isDialog);

            if (view is ShellView)
            {
                ShellWindow = window;
                ShellWindow.SizeToContent = SizeToContent.Manual;
                // Set window size
                if (Settings.Default.WindowSize != null && Settings.Default.WindowSize.Height != 0 &&
                    Settings.Default.WindowSize.Width != 0)
                {
                    ShellWindow.Width = Settings.Default.WindowSize.Width;
                    ShellWindow.Height = Settings.Default.WindowSize.Height;
                }
            }

            return window;
        }

        public void Handle(ShowRecordingStatusChangedEvent message)
        {
            if( message.IsRecording )
            {
                mIsRecording[message.ShowId] = message.IsRecording;
            }
            else
            {
                if( mIsRecording.ContainsKey(message.ShowId) )
                {
                    mIsRecording.Remove(message.ShowId);
                }
            }

            ShellWindow.Icon = mIsRecording.Keys.Count > 0 ? 
                new BitmapImage(new Uri("icon-rec.png", UriKind.Relative)) : 
                new BitmapImage(new Uri("icon.png", UriKind.Relative));
        }
    }
}
