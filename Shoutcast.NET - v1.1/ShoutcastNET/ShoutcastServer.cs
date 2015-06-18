using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace ShoucastNET.ShoutcastDetails
{
    /// <summary>
    /// Creates an instance of a shoutcast server and gives access to all the properties of the xml file
    /// </summary>
    public class ShoutcastServer : IDisposable
    {
        XDocument ShoutcastXml;
        WebData webdata;
        string _ServerUrl;
        /// <summary>
        /// Create an instance of this class
        /// </summary>
        /// <param name="ServerUrl">The Streaming Url of the server 
        /// Example : http://localhost:8000/admin.cgi?mode=viewxml&#x26;pass=adminpass </param>
        public ShoutcastServer(string ServerUrl)
        {
            try
            {

                this.ShoutcastServerUrl = ServerUrl;
                ShoutcastXml = XDocument.Load(Get_XMLFile());
                webdata = new WebData(this);
            }
            catch
            {
                throw new ServerDownException("Connection with Server failed. No Response received. " +
                "Please check if server is online, stream url is correct and/or Internet connection is available.");
            }
        }
        private TextReader Get_XMLFile()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.ShoutcastServerUrl);
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            try
            {
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                TextReader sw = new StreamReader(res.GetResponseStream());
                res = null;
                return sw;
            }
            catch
            {
                return null;
            }
            finally
            {
                req = null;
            }
        }
        ///<summary>
        ///The Streaming url of the shoutcast server
        ///Example : http://localhost:8000/admin.cgi?mode=viewxml&#x26;pass=adminpass
        ///</summary>
        public string ShoutcastServerUrl
        {
            get { return _ServerUrl; }
            set { _ServerUrl = value; }
        }
        private string GetProperty(XDocument x, ShoucastProperties Property)
        {
            try
            {
                var q = (from c in x.Descendants("SHOUTCASTSERVER")
                         select (string)c.Element(Property.ToString())).First();
                return q;
            }
            catch (NullReferenceException)
            {
                return "Undefined";
            }

        }
        protected string GetWebDataProperty(XDocument x, ShoucastWebDataProperties Property)
        {
            try
            {
                var q = (from c in x.Descendants("WEBDATA")
                         select (string)c.Element(Property.ToString())).First();
                return q;
            }
            catch (NullReferenceException)
            {
                return "-1";
            }

        }
        /// <summary>
        /// Gets an updated version of the Shoutcast xml file with update statistics
        /// More useful at WinForm applications
        /// </summary>
        public void Refresh()
        {
            ShoutcastXml = XDocument.Load(Get_XMLFile());
        }
        enum ShoucastProperties
        {
            SONGURL, IRC, ICQ, AIM, WEBHITS,
            STREAMHITS, STREAMSTATUS, BITRATE, CONTENT, VERSION,
            CURRENTLISTENERS, PEAKLISTENERS, MAXLISTENERS, REPORTEDLISTENERS,
            AVERAGETIME, SERVERGENRE, SERVERURL, SERVERTITLE, SONGTITLE
        }
        protected enum ShoucastWebDataProperties
        {
            INDEX, LISTEN, PALM7, LOGIN, LOGINFAIL, PLAYED, COOKIE, ADMIN, UPDINFO, KICKSRC,
            KICKDST, UNBANDST, BANDST, VIEWBAN, UNRIPDST, RIPDST, VIEWRIP, VIEWXML, VIEWLOG, INVALID
        }

        #region Properties
        public string SongUrl
        {
            get { return GetProperty(ShoutcastXml, ShoucastProperties.SONGURL); }
        }
        public string IRC
        {
            get { return GetProperty(ShoutcastXml, ShoucastProperties.IRC); }
        }
        public string ICQ
        {
            get { return GetProperty(ShoutcastXml, ShoucastProperties.ICQ); }
        }
        public string AIM
        {
            get { return GetProperty(ShoutcastXml, ShoucastProperties.AIM); }
        }
        public int WebHits
        {
            get { return Convert.ToInt32(GetProperty(ShoutcastXml, ShoucastProperties.WEBHITS)); }
        }
        public int StreamHits
        {
            get { return Convert.ToInt32(GetProperty(ShoutcastXml, ShoucastProperties.STREAMHITS)); }
        }
        public bool StreamStatus
        {
            get { return Convert.ToBoolean(Convert.ToInt32(GetProperty(ShoutcastXml, ShoucastProperties.STREAMSTATUS))); }
        }
        public int Bitrate
        {
            get { return Convert.ToInt32(GetProperty(ShoutcastXml, ShoucastProperties.BITRATE)); }
        }
        public string Content
        {
            get { return GetProperty(ShoutcastXml, ShoucastProperties.CONTENT); }
        }
        public string Version
        {
            get { return GetProperty(ShoutcastXml, ShoucastProperties.VERSION); }
        }
        public int CurrentListeners
        {
            get { return Convert.ToInt32(GetProperty(ShoutcastXml, ShoucastProperties.CURRENTLISTENERS)); }
        }
        public int PeakListeners
        {
            get { return Convert.ToInt32(GetProperty(ShoutcastXml, ShoucastProperties.PEAKLISTENERS)); }
        }
        public int MaxListeners
        {
            get { return Convert.ToInt32(GetProperty(ShoutcastXml, ShoucastProperties.MAXLISTENERS)); }
        }
        public int ReportedListeners
        {
            get { return Convert.ToInt32(GetProperty(ShoutcastXml, ShoucastProperties.REPORTEDLISTENERS)); }
        }
        public string ServerGenre
        {
            get { return GetProperty(ShoutcastXml, ShoucastProperties.SERVERGENRE); }
        }
        public string ServerTitle
        {
            get { return GetProperty(ShoutcastXml, ShoucastProperties.SERVERTITLE); }
        }
        public string ServerUrl
        {
            get { return GetProperty(ShoutcastXml, ShoucastProperties.SERVERURL); }
        }
        public int AverageTime
        {
            get { return Convert.ToInt32(GetProperty(ShoutcastXml, ShoucastProperties.AVERAGETIME)); }
        }
        public string SongTitle
        {
            get { return this.GetProperty(this.ShoutcastXml, ShoucastProperties.SONGTITLE); }
        }
        public List<Listener> Listeners
        {
            get
            {
                List<Listener> Listeners = new List<Listener>();
                var q = (from c in this.ShoutcastXml.Descendants("LISTENERS")
                         select c.Elements("LISTENER")).ToList();

                foreach (var item in q)
                {
                    item.ToList().ForEach(p =>
                         Listeners.Add(new Listener(p.Element("HOSTNAME").Value, p.Element("USERAGENT").Value
                         , p.Element("UNDERRUNS").Value, Convert.ToInt32(p.Element("CONNECTTIME").Value)
                         , p.Element("POINTER").Value, p.Element("UID").Value
                         )));
                }
                return Listeners;
            }
        }
        public List<Song> SongHistory
        {
            get
            {
                List<Song> SongHistory = new List<Song>();
                var q = (from c in this.ShoutcastXml.Descendants("SONGHISTORY")
                         select c.Elements("SONG")).ToList();
                foreach (var item in q)
                {
                    item.ToList().ForEach(p =>
                         SongHistory.Add(new Song(p.Element("TITLE").Value, ShoutcastServer.ConvertFromUnixTimestamp(Convert.ToDouble(p.Element("PLAYEDAT").Value)))));
                }
                return SongHistory;
            }
        }
        /// <summary>
        /// Contains all the values from the WEBDATA region of the xml file
        /// </summary>
        public WebData Webdata
        {
            get { return this.webdata; }
        }
        #endregion
        public class WebData : IDisposable
        {
            ShoutcastServer s;
            internal WebData(ShoutcastServer s)
            {
                this.s = s;
            }
            #region WebData Properties
            /// <summary>
            /// Gets the INDEX value from WEBDATA
            /// </summary>
            public int Index
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.INDEX)); }
            }
            /// <summary>
            /// Gets the LISTEN value from WEBDATA
            /// </summary>
            public int Listen
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.LISTEN)); }
            }
            /// <summary>
            /// Gets the PALM7 value from WEBDATA
            /// </summary>
            public int Palm7
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.PALM7)); }
            }
            /// <summary>
            /// Gets the LOGIN value from WEBDATA
            /// </summary>
            public int Login
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.LOGIN)); }
            }
            /// <summary>
            /// Gets the LOGINFAIL value from WEBDATA
            /// </summary>
            public int LoginFail
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.LOGINFAIL)); }
            }
            /// <summary>
            /// Gets the PLAYED value from WEBDATA
            /// </summary>
            public int Played
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.PLAYED)); }
            }
            /// <summary>
            /// Gets the COOKIE value from WEBDATA
            /// </summary>
            public int Cookie
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.COOKIE)); }
            }
            /// <summary>
            /// Gets the ADMIN value from WEBDATA
            /// </summary>
            public int Admin
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.ADMIN)); }
            }
            /// <summary>
            /// Gets the UPDINFO value from WEBDATA
            /// </summary>
            public int UpdInfo
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.UPDINFO)); }
            }
            /// <summary>
            /// Gets the KICKSRC value from WEBDATA
            /// </summary>
            public int KickSrc
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.KICKSRC)); }
            }
            /// <summary>
            /// Gets the KICKDST value from WEBDATA
            /// </summary>
            public int KickDst
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.KICKDST)); }
            }
            /// <summary>
            /// Gets the UNBANDST value from WEBDATA
            /// </summary>
            public int UnBanDst
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.UNBANDST)); }
            }
            /// <summary>
            /// Gets the BANDST value from WEBDATA
            /// </summary>
            public int BanDst
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.BANDST)); }
            }
            /// <summary>
            /// Gets the VIEWBAN value from WEBDATA
            /// </summary>
            public int ViewBan
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.VIEWBAN)); }
            }
            /// <summary>
            /// Gets the UNRIPDST value from WEBDATA
            /// </summary>
            public int UnRipDst
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.UNRIPDST)); }
            }
            /// <summary>
            /// Gets the RIPDST value from WEBDATA
            /// </summary>
            public int RipDst
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.RIPDST)); }
            }
            /// <summary>
            /// Gets the VIEWRIP value from WEBDATA
            /// </summary>
            public int ViewRip
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.VIEWRIP)); }
            }
            /// <summary>
            /// Gets the VIEWXML value from WEBDATA
            /// </summary>
            public int ViewXml
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.VIEWXML)); }
            }
            /// <summary>
            /// Gets the VIEWLOG value from WEBDATA
            /// </summary>
            public int ViewLog
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.VIEWLOG)); }
            }
            /// <summary>
            /// Gets the INVALID value from WEBDATA
            /// </summary>
            public int Invalid
            {
                get { return Convert.ToInt32(s.GetWebDataProperty(s.ShoutcastXml, ShoucastWebDataProperties.INVALID)); }
            }

            #endregion
            #region IDisposable Members
            public void Dispose()
            {
                s = null;
            }
            #endregion
        }
        #region IDisposable Members

        public void Dispose()
        {
            webdata.Dispose();
            ShoutcastXml = null;
            _ServerUrl = null;
        }
        static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
        #endregion
    }

    [global::System.Serializable]
    public class ServerDownException : Exception
    {
        public ServerDownException() { }
        public ServerDownException(string message) : base(message) { }
        public ServerDownException(string message, Exception inner) : base(message, inner) { }
        protected ServerDownException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

}
