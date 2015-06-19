using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using DevHost.Shoutcast;

namespace ripper
{
    public class RecordShowTask : IResult
    {
        private readonly EpisodeRecording mEpisodeRecording;
        private Thread mRecordingThread;
        private CoroutineExecutionContext mContext;

        public RecordShowTask(EpisodeRecording episodeRecording)
        {
            mEpisodeRecording = episodeRecording;
        }

        public void Execute(CoroutineExecutionContext context)
        {
            mContext = context;
            StartRecording();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;

        private void StartRecording()
        {
            //mIsRecordingCanceled = new AutoResetEvent(false);
            mRecordingThread = new Thread(() => 
                DoRecording(
                    mEpisodeRecording.Url, 
                    TimeSpan.FromSeconds(30), 
                    Log, 
                    mEpisodeRecording.SaveLocation, 
                    () => Completed(this, new ResultCompletionEventArgs())))
            {
                IsBackground = true,
                Name = "Ripper recording thread"
            };
            mRecordingThread.Start();
        }

        //private void StopRecording()
        //{
        //    mIsRecordingCanceled.Set();
        //}

        private static void DoRecording(string url, TimeSpan duration, Action<string> log, string savePath, System.Action whenDone)
        {
            var started = false;

            using (var output = File.OpenWrite(savePath))
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

                    //if (mIsRecordingCanceled.WaitOne(0))
                    //{
                    //    log("Recording cancelled!");
                    //    break;
                    //}

                    if (startTime + duration < DateTime.Now)
                    {
                        log("Recording finished.");
                        break;
                    }

                    if (DateTime.Now - lastPrintTime <= TimeSpan.FromSeconds(2))
                    {
                        continue;
                    }

                    log(string.Format("Recording for {0} seconds..", DateTime.Now - startTime));
                    lastPrintTime = DateTime.Now;
                }
            }

            log("Recording thread exiting...");
            whenDone();
        }

        private void Log(string s)
        {
            //var msg = s + "\n";
            //if (LogView.Dispatcher.CheckAccess())
            //{
            //    LogView.Text += msg;
            //}
            //else
            //{
            //    LogView.Dispatcher.Invoke(() => LogView.Text += msg);
            //}
            Debug.WriteLine(s);
        }
    }
}
