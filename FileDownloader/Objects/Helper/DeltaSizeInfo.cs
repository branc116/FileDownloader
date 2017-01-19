using FileDownloader.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileDownloader.Objects.Helper
{
    class DeltaSizeInfo : IDeltaSizeInfo
    {
        private static string[] _prefixes = new string[7] { "", "K", "M", "G", "T", "P", "E" };
        /// <summary>
        /// Change of size in bytes
        /// </summary>
        public long SizeBytes { get; set; }
        /// <summary>
        /// Change of size in Gbytes
        /// </summary>
        public int SizeGBytes
        {
            get
            {
                return (int)(SizeBytes >> 30);
            }
        }
        /// <summary>
        /// Change of size in Kbytes
        /// </summary>
        public int SizeKBytes
        {
            get
            {
                return (int)(SizeBytes >> 10);
            }
        }
        /// <summary>
        /// Change of size in Mbytes
        /// </summary>
        public int SizeMBytes
        {
            get
            {
                return (int)(SizeBytes >> 20);
            }
        }
        public string SizeAuto
        {
            get
            {
                int count = 0;
                long temp = SizeBytes;
                while ((temp = (temp >> 10)) > 0) count++;
                return $"{SizeBytes >> (10 * count)}{_prefixes[count]}B";
            }
        }
        /// <summary>
        /// Speed of change of change of size in bytes/s
        /// </summary>
        public int SpeedBps
        {
            get
            {
                return (int)(CurrentSizeBytes/(this.Duration.TotalSeconds + double.Epsilon));
            }
        }
        /// <summary>
        /// Speed of change of change of size in Kbytes/s
        /// </summary>
        public float SpeedKBps
        {
            get
            {
                return (int)(SizeBytes / (this.Duration.TotalSeconds + double.Epsilon)) >>10;
            }
        }
        /// <summary>
        /// Speed of change of change of size in Mbytes/s
        /// </summary>
        public float SpeedMBps
        {
            get
            {
                return (int)(SizeBytes / (this.Duration.TotalSeconds + double.Epsilon)) >> 20;
            }
        }
        public string SpeedAuto
        {
            get
            {
                int count = 0;
                long temp = SpeedBps;
                while ((temp = (temp >> 10)) > 0) count++;
                return $"{SpeedBps >> (10 * count)}{_prefixes[count]}Bps";
            }
        }
        /// <summary>
        /// Duration of changing size
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return EndedAt - StartedAt;
            }
        }
        /// <summary>
        /// End of change
        /// </summary>
        public DateTime EndedAt { get; set; }
        /// <summary>
        /// Start of change
        /// </summary>
        public DateTime StartedAt { get; set; }
        /// <summary>
        /// Current size
        /// </summary>
        public string CurrentSizeAuto
        {
            get
            {
                int count = 0;
                long temp = CurrentSizeBytes;
                while ((temp = (temp >> 10)) > 0) count++;
                return $"{CurrentSizeBytes >> (10 * count)}{_prefixes[count]}B";
            }
        }
        /// <summary>
        /// Current size in Bytes
        /// </summary>
        public long CurrentSizeBytes { get; set; } = 0;
        /// <summary>
        /// Current size in KBytes
        /// </summary>
        public int CurrentSizeKBytes
        {
            get
            {
                return (int)(CurrentSizeBytes >> 10);
            }
        }
        /// <summary>
        /// Current size in MBytes
        /// </summary>
        public int CurrentSizeMBytes
        {
            get
            {
                return (int)(CurrentSizeBytes >> 20);
            }
        }
        /// <summary>
        /// Current size in GBytes
        /// </summary>
        public int CurrentSizeGBytes
        {
            get
            {
                return (int)(CurrentSizeBytes >> 30);
            }
        }

        public DeltaSizeInfo(DateTime StartTime)
        {
            this.StartedAt = StartTime;
        }
        public override string ToString()
        {
            return $"TimeDelta: {Duration.Seconds}s, Size: {SizeAuto}, Speed: {SpeedAuto}";
        }
    }
}
