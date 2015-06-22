using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ShoutcastDvr
{
    public interface IShowList
    {
        BindableCollection<Show> Shows
        {
            get;
            set;
        }

        void AddShow(Show show);

        bool RemoveShow(Show show);

        void Save();
    }
}
