using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoucastNET.ShoutcastDetails
{
    /// <summary>
    /// Represents a listener with details such as Hostname, UserAgent etc.
    /// </summary>
    public class Listener : IDisposable
    {
        public Listener(string HostName, string UserAgent, string Underruns, int ConnectTime, string Pointer, string UID)
        {
            this._HostName = HostName;
            this._UserAgent = UserAgent;
            this._Underruns = Underruns;
            this._ConnectTime = ConnectTime;
            this._Pointer = Pointer;
            this._UID = UID;
        }
        string _HostName, _UserAgent, _Underruns, _Pointer, _UID;
        int _ConnectTime;
        public string HostName
        {
            get { return _HostName; }
        }
        public string UserAgent
        {
            get { return _UserAgent; }
        }
        public string Underruns
        {
            get { return _Underruns; }
        }
        public int ConnectTime
        {
            get { return _ConnectTime; }
        }
        public string Pointer
        {
            get { return _Pointer; }
        }
        public string UID
        {
            get { return _UID; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this._HostName = null;
            this._Pointer = null;
            this._UID = null;
            this._Underruns = null;
            this._UserAgent = null;
        }

        #endregion
    }
}
