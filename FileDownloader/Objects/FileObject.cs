using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using FileDownloader.Objects.Interfaces;
using FileDownloader.Args;
using FileDownloader.Objects.Helper;
using FileDownloader.EnvironmentHelp;
using FileDownloader.Enums;
using System.Threading;

namespace FileDownloader.Objects
{
    class FileMessageHandler : HttpMessageHandler
    {
        public FileMessageHandler() : base()
        {
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
    public class FileObject : IFileObject
    {
        public event EventHandler<UriChangedArgs> UriChange;
        public event EventHandler<FileDownloadedArgs> FileDownloaded;
        public event EventHandler<FileDownloadingArgs> FileDownloading;
        public event EventHandler<FileRemovedArgs> FileRemoved;
        public event EventHandler<DiscSizeArgs> DiscSizeChanged;
        public event EventHandler<FileSavingArgs> FileSaving;
        public event EventHandler<FileSavedArgs> FileSaved;
        public IDeltaSizeInfo DownloadInfo { get; private set; }

        public byte[] RawFile { get; private set; }

        public UriObject Uri { get; set; }
        public object Tag { get; set; }
        public IList<string> PathsOnDisc { get; } = new List<string>();
        public string Name
        {
            get
            {
                return _name ?? Uri?.Segments?.Last() ?? "Unamed";
            }
            set
            {
                _name = value;
            }
        }

        public FileStatus Status
        {
            get
            {
                if (RawFile == null && (PathsOnDisc?.Count ?? 0) == 0 && string.IsNullOrWhiteSpace(Uri?.AbsoluteUri))
                    return FileStatus.Nothing;
                else if (RawFile != null && (PathsOnDisc?.Count ?? 0) != 0)
                    return FileStatus.OnDiscInMemory;
                else if (RawFile != null)
                    return FileStatus.InMemory;
                else if ((PathsOnDisc?.Count ?? 0) != 0)
                    return FileStatus.OnDisc;
                else if (!string.IsNullOrWhiteSpace(Uri?.AbsoluteUri))
                    return FileStatus.HasUri;
                else
                    return FileStatus.Nothing;
            }
        }

        public IDeltaSizeInfo SaveInfo { get; private set; }

        private string _name;
        public FileObject()
        {

        }
        public FileObject(string Uri)
        {
            this.Uri = new UriObject(Uri);
        }
        public void DeleteFromMemory()
        {
            RawFile = null;
        }
        /// <summary>
        /// Download file only in memory. To save it on disc use SaveToDiscAsync methode
        /// </summary>
        /// <returns>Returns true if download was successful</returns>
        public async Task<DownloadStatus> DownloadAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Uri.AbsoluteUri))
                    return DownloadStatus.BadUri;
                //FileDownloading?.Invoke(this, new FileDownloadingArgs(this));
                HttpClient client = new HttpClient();
                
                using (var tempStream = new MemoryStream())
                {
                    var res = await client.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead);
                    
                    Task down =  res.Content.CopyToAsync(tempStream);
                    DownloadInfo = new DeltaSizeInfo(DateTime.Now)
                    {
                        SizeBytes = res.Content.Headers?.ContentLength ?? 0
                    };
                    Task Proggress = Task.Factory.StartNew(async () =>
                    {
                       while (!down.IsCompleted)
                       {
                            //var arr = tempStream.ToArray();
                           DownloadInfo.EndedAt = DateTime.Now;
                           DownloadInfo.CurrentSizeBytes = tempStream?.Seek(0, SeekOrigin.Current) ?? 0;
                           FileDownloading?.Invoke(this, new FileDownloadingArgs(this, DownloadInfo));
                           await Task.Delay(30);
                       }
                    });
                    await Task.WhenAll(down, Proggress);
                    await Task.Factory.StartNew(() =>
                   {
                       Task.WaitAll(down, Proggress);
                   });
                    tempStream.Seek(0, SeekOrigin.Begin);
                    RawFile = tempStream.ToArray();
                    DownloadInfo.EndedAt = DateTime.Now;
                    client.Dispose();
                }

                DownloadInfo.SizeBytes = RawFile.Length;
                FileDownloaded?.Invoke(this, new FileDownloadedArgs(this, DownloadInfo));
                return DownloadStatus.Ok;
            }
            catch (Exception ex)
            {
                return DownloadStatus.Error;
            }
        }
        /// <summary>
        /// Save it localy on disc, progress can be tracked with event DiscSizeChange
        /// </summary>
        /// <returns>Returns true if file was save successfuly</returns>
        public async Task<DiscWriteStatus> SaveOnDiscAsync(string Path)
        {
            FileSaving?.Invoke(this, new FileSavingArgs(this, Path));
            var Ok = DiscWriteStatus.Error;
            var finished = false;
            if (RawFile == null)
                return DiscWriteStatus.Error;

            using (var file = File.OpenWrite(Path))
            {
                var TimeOutTime = (RawFile.Length >> 20) * 2000 + 2000;
                long startPoz = file.Position;
                var delat = new DeltaSizeInfo(DateTime.Now);
                Task Writing = file.WriteAsync(RawFile, 0, RawFile.Length);
                Task Update = Task.Factory.StartNew(async () =>
                {

                    while (!finished)
                    {
                        try
                        {
                            var curPoz = file.Position;
                            delat.EndedAt = DateTime.Now;
                            delat.SizeBytes = curPoz - startPoz;
                            DiscSizeChanged?.Invoke(this, new DiscSizeArgs(this, delat));

                        }
                        catch (Exception ex)
                        {
                        }
                        await Task.Delay(1000);
                    }

                });
                var TimeOut = Task.Delay(TimeOutTime);
                await Task.WhenAny(Writing, Update, TimeOut).ContinueWith((tt) =>
                {
                    finished = true;
                    if (!TimeOut.IsCompleted)
                    {
                        delat.EndedAt = DateTime.Now;

                        Ok = DiscWriteStatus.Ok;
                        this.PathsOnDisc.Add(Path);
                        var curPozy = file.Position;

                        delat.SizeBytes = curPozy - startPoz;
                        this.SaveInfo = delat;
                        DiscSizeChanged?.Invoke(this, new DiscSizeArgs(this, delat));
                        FileSaved?.Invoke(this, new FileSavedArgs(this, Path));
                    }

                });
            }
            return Ok;
        }
        /// <summary>
        /// Save it localy on disc, progress can be tracked with event DiscSizeChange
        /// With path in Home/Downloads/{NameOfTheFile}
        /// </summary>
        /// <returns>Returns true if file was save successfuly</returns>
        public async Task<DiscWriteStatus> SaveOnDiscAsync()
        {
            return await SaveOnDiscAsync(Path.Combine(EnvironmentVariables.DownloadFolder, Name));
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                RawFile = null;
                PathsOnDisc.Clear();
                DownloadInfo = null;

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FileObject() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
        public int CompareTo(object obj)
        {
            return this.Uri.AbsoluteUri.CompareTo(obj.GetType() == typeof(IFileObject) ? (obj as IFileObject).Uri.AbsoluteUri : null);
        }

        public Task<bool> RemoveFormDiscAsync()
        {
            throw new NotImplementedException();
        }
    }
}
