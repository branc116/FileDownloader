using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FileDownloader.Args;
using FileDownloader.Objects.Helper;
using FileDownloader.Enums;

namespace FileDownloader.Objects.Interfaces
{
    public interface IFileObject : IDisposable, IComparable
    {
        event EventHandler<UriChangedArgs> UriChange;
        event EventHandler<FileDownloadedArgs> FileDownloaded;
        event EventHandler<FileDownloadingArgs> FileDownloading;
        event EventHandler<FileRemovedArgs> FileRemoved;
        event EventHandler<FileSavingArgs> FileSaving;
        event EventHandler<FileSavedArgs> FileSaved;
        byte[] RawFile { get; }
        UriObject Uri { get; set; }
        string Name { get; set; }
        FileStatus Status { get;}
        object Tag { get; set; }
        IDeltaSizeInfo DownloadInfo { get;}
        IDeltaSizeInfo SaveInfo { get; }
        IList<string> PathsOnDisc { get; }
        void DeleteFromMemory();
        Task<DownloadStatus> DownloadAsync();
        Task<DiscWriteStatus> SaveOnDiscAsync(string Path);
        Task<DiscWriteStatus> SaveOnDiscAsync();
        Task<bool> RemoveFormDiscAsync();
    }
}
