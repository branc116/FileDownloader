using System;
using System.Collections.Generic;
using System.Text;

namespace FileDownloader.Enums
{
    public enum FileStatus
    {
        Nothing,
        HasUri,
        Downloading,
        InMemory,
        OnDisc,
        OnDiscInMemory
    }
    public enum DownloadStatus
    {
        Ok,
        BadUri,
        TimeOut,
        Error,
    }
    public enum DiscWriteStatus
    {
        Ok,
        BadPath,
        TimeOut,
        Error
    }
}
