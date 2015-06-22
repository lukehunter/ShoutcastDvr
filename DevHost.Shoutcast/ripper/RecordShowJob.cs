using System;
using System.Diagnostics;
using System.IO;
using DevHost.Shoutcast;
using Quartz;
using log4net;

namespace ripper
{
    class RecordShowJob : IJob
    {
        private static ILog mLogger = LogManager.GetLogger(typeof(RecordShowJob));

        public void Execute(IJobExecutionContext context)
        {
            var show = context.MergedJobDataMap.Get(Show.Job_Data_Key) as Show;

            if (show == null)
            {
                return;
            }

            var saveFolder = Properties.Settings.Default.SaveFolder;
            var basename = string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd"), show.ShowName);
            var filename = string.Format("{0}.mp3", basename);
            var fullname = Path.Combine(saveFolder, filename);
            var counter = 1;

            while (File.Exists(fullname))
            {
                filename = string.Format("{0}-{1}.mp3", basename, counter);
                fullname = Path.Combine(saveFolder, filename);

                counter++;
            }

            Log(string.Format("{0} Attempting to record {1} for {2} minutes", DateTime.Now, show.ShowName, show.Duration));
            DoRecording(show.Url, TimeSpan.FromMinutes(show.Duration), Log, fullname);
        }

        private static void DoRecording(string url, TimeSpan duration, Action<string> log, string savePath)
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

                    //log(string.Format("Recording for {0} seconds..", DateTime.Now - startTime));
                    lastPrintTime = DateTime.Now;
                }
            }
        }

        private void Log(string s)
        {
            mLogger.Info(s);
        }
    }
}
