using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoutcastDvr
{
    public class ShowRecordingStatusChangedEvent
    {
        public string ShowId
        {
            get;
            private set;
        }

        public bool IsRecording
        {
            get;
            private set;
        }

        public ShowRecordingStatusChangedEvent(string showId, bool isRecording)
        {
            ShowId = showId;
            IsRecording = isRecording;
        }
    }
}
