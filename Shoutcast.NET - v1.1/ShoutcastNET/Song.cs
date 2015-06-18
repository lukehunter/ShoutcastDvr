using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoucastNET.ShoutcastDetails
{
    /// <summary>
    /// Represents a node from the PLAYEDAT node collection of the shoucast xml
    /// </summary>
    public class Song : IDisposable
    {
        string _SongTitle;
        DateTime _PlayedAt;
        public string SongTitle
        {
            get
            {
                return this._SongTitle;
            }
        }
        public DateTime PlayedAt
        {
            get
            {
                return this._PlayedAt;
            }
        }
        public Song(string SongTitle, DateTime PlayedAt)
        {
            this._SongTitle = SongTitle;
            this._PlayedAt = PlayedAt;
        }

        #region IDisposable Members
        public void Dispose()
        {
            this._SongTitle = null;
        }
        #endregion
    }
}
