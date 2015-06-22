using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ShoutcastDvr
{
    [Export(typeof(SettingsViewModel))]
    public class SettingsViewModel : PropertyChangedBase 
    {
        private string mSaveFolder;

        /// <summary>
        /// 
        /// </summary>
        public string SaveFolder
        {
            get
            {
                return mSaveFolder;
            }
            set
            {
                mSaveFolder = value;
                NotifyOfPropertyChange(() => SaveFolder);
            }
        }

        public void Save()
        {
            Properties.Settings.Default.SaveFolder = mSaveFolder;
            Properties.Settings.Default.Save();
        }
    }
}
