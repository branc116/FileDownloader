using FileDownloader.Objects;
using FileDownloader.Objects.Helper;
using FileDownloader.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileDownloader.Args
{
    public abstract class FileBaseArgs : EventArgs
    {
        public IFileObject File;
        public FileBaseArgs(IFileObject File)
        {
            this.File = File;
        }
    }
    public class FileAddedArgs : FileBaseArgs
    {
        public FileAddedArgs(IFileObject File) : base(File)
        {
        }
    }
    public class FileAddingArgs : FileBaseArgs
    {
        public FileAddingArgs(IFileObject File) : base(File)
        {
        }
    }
    public class FileRemovedArgs : FileBaseArgs
    {
        public FileRemovedArgs(IFileObject File) : base(File)
        {
        }
    }
    public class FileRemovingArgs : FileBaseArgs
    {
        public FileRemovingArgs(IFileObject File) : base(File)
        {
        }
    }
    public class FileChangeingArgs : FileBaseArgs
    {
        public FileChangeingArgs(IFileObject File) : base(File)
        {
        }
    }
    public class FileChangedArgs : FileBaseArgs
    {
        public FileChangedArgs(IFileObject File) : base(File)
        {
        }
    }
    public class FileDownloadedArgs : FileBaseArgs
    {
        public IDeltaSizeInfo DownloadInfo { get; private set; }
        public FileDownloadedArgs(IFileObject File) : base(File)
        {
        }
        public FileDownloadedArgs(IFileObject File, IDeltaSizeInfo DownloadInfo) : base(File)
        {
            this.DownloadInfo = DownloadInfo;
        }
    }
    public class FileDownloadingArgs : FileBaseArgs
    {
        public IDeltaSizeInfo DownloadInfo { get; set; }
        public FileDownloadingArgs(IFileObject File) : base(File)
        {
        }
        public FileDownloadingArgs(IFileObject File, IDeltaSizeInfo DownloadInfo) : base(File)
        {
            this.DownloadInfo = DownloadInfo;
        }
    }
    public class FileDisposedArgs : FileBaseArgs
    {
        public FileDisposedArgs(IFileObject File) : base(File)
        {
        }
    }
    public class DiscSizeArgs : FileBaseArgs
    {
        public IDeltaSizeInfo Info { get; set; }
        public DiscSizeArgs(IFileObject File) : base(File)
        {
        }
        public DiscSizeArgs(IFileObject File, IDeltaSizeInfo Info) : base(File)
        {
            this.Info = Info;
        }
    }
    public class FileSavingArgs : FileBaseArgs
    {
        public string Path { get; private set; }
        public FileSavingArgs(IFileObject File) : base(File)
        { 
        }
        public FileSavingArgs(IFileObject File, string Path) : base(File)
        {
            this.Path = Path;
        }
    }
    public class FileSavedArgs : FileBaseArgs
    {
        public string Path { get; private set; }
        public FileSavedArgs(IFileObject File) : base(File)
        {
        }
        public FileSavedArgs(IFileObject File, string Path) : base(File)
        {
            this.Path = Path;
        }
    }
    public class UriChangedArgs : EventArgs
    {
        public UriObject OldUri { get; }
        public UriObject NewUri { get; }
        public UriChangedArgs(UriObject OldUri, UriObject NewUri)
        {
            this.OldUri = OldUri;
            this.NewUri = NewUri;
        }
    }

}
 