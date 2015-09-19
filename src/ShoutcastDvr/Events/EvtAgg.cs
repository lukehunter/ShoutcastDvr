using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ShoutcastDvr
{
    public static class EvtAgg
    {
        private static IEventAggregator _Current = new EventAggregator();
        public static IEventAggregator Current
        {
            get
            {
                return _Current;
            }
        }
    }
}
