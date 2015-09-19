using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Caliburn.Micro;

namespace ShoutcastDvr
{
    class JsonShowList : IShowList
    {
        private readonly string mFilename;
        public BindableCollection<Show> Shows
        {
            get;
            set;
        }

        public JsonShowList(string filename)
        {
            mFilename = filename;
            Shows = new BindableCollection<Show>();

            Load();
        }

        public void AddShow(Show show)
        {
            if (!Shows.Contains(show))
            {
                Shows.Add(show);
            }
        }

        public bool RemoveShow(Show show)
        {
            if (Shows.Contains(show))
            {
                Shows.Remove(show);
                return true;
            }

            return false;
        }

        public void Save()
        {
            var filename = GetFilename();
            var json = new JavaScriptSerializer().Serialize(Shows);
            File.WriteAllText(filename, json);
        }

        private void Load()
        {
            var filename = GetFilename();

            if( !File.Exists(filename) )
            {
                return;
            }

            var json = File.ReadAllText(filename);
            Shows = new JavaScriptSerializer().Deserialize<BindableCollection<Show>>(json);
        }

        private string GetFilename()
        {
            return !string.IsNullOrWhiteSpace(Model.Options.Instance.SchedulePath) ? Model.Options.Instance.SchedulePath : mFilename;
        }
    }
}
