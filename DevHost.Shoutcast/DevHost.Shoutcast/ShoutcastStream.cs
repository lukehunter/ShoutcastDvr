using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace DevHost.Shoutcast
{
    /// <summary>
    ///     Provides the functionality to receive a shoutcast media stream
    /// </summary>
    public class ShoutcastStream : Stream
    {
        private readonly int mMetaInt;
        private readonly Stream mNetStream;
        private bool mConnected;
        private int mReceivedBytes;

        private string mStreamTitle;

        /// <summary>
        ///     Creates a new ShoutcastStream and connects to the specified Url
        /// </summary>
        /// <param name="url">Url of the Shoutcast stream</param>
        public ShoutcastStream(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Clear();
            request.Headers.Add("Icy-MetaData", "1");
            request.KeepAlive = false;
            request.UserAgent = "VLC media player";

            var response = (HttpWebResponse)request.GetResponse();

            mMetaInt = int.Parse(response.Headers["Icy-MetaInt"]);
            mReceivedBytes = 0;

            mNetStream = response.GetResponseStream();

            mConnected = true;
        }

        /// <summary>
        ///     Gets a value that indicates whether the ShoutcastStream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return mConnected;
            }
        }

        /// <summary>
        ///     Gets a value that indicates whether the ShoutcastStream supports seeking.
        ///     This property will always be false.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Gets a value that indicates whether the ShoutcastStream supports writing.
        ///     This property will always be false.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Gets the title of the stream
        /// </summary>
        public string StreamTitle
        {
            get
            {
                return mStreamTitle;
            }
        }

        /// <summary>
        ///     Gets the length of the data available on the Stream.
        ///     This property is not currently supported and always thows a <see cref="NotSupportedException" />.
        /// </summary>
        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        ///     Gets or sets the current position in the stream.
        ///     This property is not currently supported and always thows a <see cref="NotSupportedException" />.
        /// </summary>
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        /// <summary>
        ///     Is fired, when a new StreamTitle is received
        /// </summary>
        public event EventHandler StreamTitleChangedEvent;
        /// <summary>
        ///     Parses the received Meta Info
        /// </summary>
        /// <param name="metaInfo"></param>
        private void ParseMetaInfo(byte[] metaInfo)
        {
            string metaString = Encoding.ASCII.GetString(metaInfo);

            string newStreamTitle = Regex.Match(metaString, "(StreamTitle=')(.*)(';StreamUrl)").Groups[2].Value.Trim();
            if (!newStreamTitle.Equals(mStreamTitle))
            {
                mStreamTitle = newStreamTitle;
                OnStreamTitleChanged();
            }
        }

        /// <summary>
        ///     Fires the StreamTitleChanged event
        /// </summary>
        protected virtual void OnStreamTitleChanged()
        {
            if (StreamTitleChangedEvent != null)
            {
                StreamTitleChangedEvent(this, EventArgs.Empty);
            }
        }
        /// <summary>
        ///     Flushes data from the stream.
        ///     This method is currently not supported
        /// </summary>
        public override void Flush()
        {
            return;
        }

        /// <summary>
        ///     Reads data from the ShoutcastStream.
        /// </summary>
        /// <param name="buffer">An array of bytes to store the received data from the ShoutcastStream.</param>
        /// <param name="offset">The location in the buffer to begin storing the data to.</param>
        /// <param name="count">The number of bytes to read from the ShoutcastStream.</param>
        /// <returns>The number of bytes read from the ShoutcastStream.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                if (mReceivedBytes == mMetaInt)
                {
                    int metaLen = mNetStream.ReadByte();
                    if (metaLen > 0)
                    {
                        var metaInfo = new byte[metaLen * 16];
                        int len = 0;
                        while ((len += mNetStream.Read(metaInfo, len, metaInfo.Length - len)) < metaInfo.Length)
                        {
                            ;
                        }
                        ParseMetaInfo(metaInfo);
                    }
                    mReceivedBytes = 0;
                }

                int bytesLeft = ((mMetaInt - mReceivedBytes) > count) ? count : (mMetaInt - mReceivedBytes);
                int result = mNetStream.Read(buffer, offset, bytesLeft);
                mReceivedBytes += result;
                return result;
            }
            catch (Exception e)
            {
                mConnected = false;
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        /// <summary>
        ///     Closes the ShoutcastStream.
        /// </summary>
        public override void Close()
        {
            mConnected = false;
            mNetStream.Close();
        }

        /// <summary>
        ///     Sets the current position of the stream to the given value.
        ///     This Method is not currently supported and always throws a <see cref="NotSupportedException" />.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Sets the length of the stream.
        ///     This Method always throws a <see cref="NotSupportedException" />.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Writes data to the ShoutcastStream.
        ///     This method is not currently supported and always throws a <see cref="NotSupportedException" />.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}