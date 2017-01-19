using System;
using System.Collections.Generic;
using System.Text;

namespace FileDownloader.Objects.Interfaces
{
    public interface IDeltaSizeInfo
    {
        DateTime StartedAt { get; set; }
        DateTime EndedAt { get; set; }
        TimeSpan Duration { get; }
        string SizeAuto { get; }
        long SizeBytes { get; set; }
        int SizeKBytes { get; }
        int SizeMBytes { get; }
        int SizeGBytes { get; }
        string CurrentSizeAuto { get; }
        long CurrentSizeBytes { get; set; }
        int CurrentSizeKBytes { get; }
        int CurrentSizeMBytes { get; }
        int CurrentSizeGBytes { get; }
        string SpeedAuto { get; }
        float SpeedMBps { get; }
        float SpeedKBps { get; }
        int SpeedBps { get; }

    }
}
