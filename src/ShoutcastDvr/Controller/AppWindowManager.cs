using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using ShoutcastDvr.Properties;

namespace ShoutcastDvr.Controller
{
    class AppWindowManager : WindowManager
    {
        public Window Window { get; set; }

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            Window = base.EnsureWindow(model, view, isDialog);

            if (view is ShellView)
            {
                Window.SizeToContent = SizeToContent.Manual;
                // Set window size
                if (Settings.Default.WindowSize != null && Settings.Default.WindowSize.Height != 0 &&
                    Settings.Default.WindowSize.Width != 0)
                {
                    Window.Width = Settings.Default.WindowSize.Width;
                    Window.Height = Settings.Default.WindowSize.Height;
                }
            }

            return Window;
        }
    }
}
