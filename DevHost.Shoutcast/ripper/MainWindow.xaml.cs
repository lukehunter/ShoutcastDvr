using System;
using System.IO;
using System.Threading;
using System.Windows;
using DevHost.Shoutcast;

namespace ripper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread mRecordingThread;
        private AutoResetEvent mIsRecordingCanceled = new AutoResetEvent(false);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            StartRecording();
        }

        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            StopRecording();
        }

        private void StartRecording()
        {
            mIsRecordingCanceled = new AutoResetEvent(false);
            var url = Url.Text;
            mRecordingThread = new Thread(() => DoRecording(url, TimeSpan.FromSeconds(12), Log));
            mRecordingThread.IsBackground = true;
            mRecordingThread.Name = "Ripper recording thread";
            mRecordingThread.Start();
        }

        private void Log(string s)
        {
            var msg = s + "\n";
            if (LogView.Dispatcher.CheckAccess())
            {
                LogView.Text += msg;
            }
            else
            {
                LogView.Dispatcher.Invoke(() => LogView.Text += msg);
            }
        }

        private void DoRecording(string url, TimeSpan duration, Action<string> log)
        {
            var started = false;

            using (var output = File.OpenWrite(string.Format( @"e:\music\incoming-ripper\test-{0}.mp3", DateTime.Now.Ticks)))
            using (var stream = new ShoutcastStream(url))
            {
                var buffer = new byte[8192];
                int bytesread;
                var startTime = new DateTime();
                var lastPrintTime = DateTime.Now;

                while ((bytesread = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (!started)
                    {
                        startTime = DateTime.Now;
                        started = true;
                        log(string.Format("Started at {0}", startTime));
                    }

                    //log(string.Format("Writing {0} bytes...\n", bytesread));
                    output.Write(buffer, 0, bytesread);

                    if (mIsRecordingCanceled.WaitOne(0))
                    {
                        log("Recording cancelled!");
                        break;
                    }

                    if (startTime + duration < DateTime.Now)
                    {
                        log("Recording finished.");
                        break;
                    }

                    if (DateTime.Now - lastPrintTime > TimeSpan.FromSeconds(2))
                    {
                        log(string.Format("Recording for {0} seconds..", DateTime.Now - startTime));
                        lastPrintTime = DateTime.Now;
                    }
                }
            }
        }

        private void StopRecording()
        {
            mIsRecordingCanceled.Set();
        }
    }
}
