using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ShoutcastDvr
{
    class DebugShowList : IShowList
    {
        private BindableCollection<Show> mShows = new BindableCollection<Show>
            {
                new Show { ShowName = "palette swapped" },
                new Show { ShowName = "othermusic" },
                new Show { ShowName = "blah"}
            };

        public BindableCollection<Show> Shows
        {
            get
            {
                return mShows;
            }
            set
            {
                mShows = value;
            }
        }

        public void AddShow(Show show)
        {
            throw new NotImplementedException();
        }

        public bool RemoveShow(Show show)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
