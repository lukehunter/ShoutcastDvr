using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ripper
{
    public class EpisodeRecording
    {
        public string SaveLocation
        {
            get;
            set;
        }

        public string ShowName
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public DateTime StartTime
        {
            get;
            set;
        }

        public TimeSpan Duration
        {
            get;
            set;
        }

        public DateTime EndTime
        {
            get
            {
                return StartTime + Duration;
            }
        }

        public DateTime ShowDate
        {
            get
            {
                return GetEpisodeDate(StartTime, EndTime);
            }
        }

        private static DateTime GetEpisodeDate(DateTime startTime, DateTime endTime)
        {
            var midpointTicks = startTime.Ticks + ((endTime.Ticks - startTime.Ticks) / 2);
            var midpoint = new DateTime(midpointTicks);

            return midpoint;
        }
    }
}
