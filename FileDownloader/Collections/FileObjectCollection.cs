using FileDownloader.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;
using FileDownloader.Objects.Interfaces;
using FileDownloader.Args;
using FileDownloader.Objects.Helper;
using FileDownloader.Collections.Interfaces;
using System.IO;
using FileDownloader.Enums;
using FileDownloader.EnvironmentHelp;

namespace FileDownloader.Collections
{
    public class FileObjectCollection : IFileObjectCollection
    {
        Dictionary<string, IFileObject> _dict = new Dictionary<string, IFileObject>();
        /// <summary>
        /// Called when new File is about to be added to the collection
        /// </summary>
        public event EventHandler<FileAddingArgs> FileAdding;
        /// <summary>
        /// Called when new File is added to the collection
        /// </summary>
        public event EventHandler<FileAddedArgs> FileAdded;
        /// <summary>
        /// Called when File is about to be downloaded
        /// </summary>
        public event EventHandler<FileDownloadingArgs> FileDownloading;
        /// <summary>
        /// Called when File has been downloaded
        /// </summary>
        public event EventHandler<FileDownloadedArgs> FileDownloaded;
        /// <summary>
        /// Called when File is about to be removed to the collection
        /// </summary>
        public event EventHandler<FileRemovingArgs> FileRemoving;
        /// <summary>
        /// Called when File has been removed from the collection
        /// </summary>
        public event EventHandler<FileRemovedArgs> FileRemoved;
        public event EventHandler<FileChangeingArgs> FileChanging;
        public event EventHandler<FileChangedArgs> FileChanged;
        /// <summary>
        /// Called when the whole collection is about to be emptyed
        /// </summary>
        public event EventHandler<EventArgs> CollectionCleaning;
        /// <summary>
        /// Called when the whole collection has been emptyed
        /// </summary>
        public event EventHandler<EventArgs> CollectionCleaned;
        /// <summary>
        /// Called when a FileObject has successfuly save a copy of itselfe on disc
        /// </summary>
        public event EventHandler<FileSavedArgs> FileSaved;
        /// <summary>
        /// Called when a FileObject has stated a process of saving itselfe on disc
        /// </summary>
        public event EventHandler<FileSavingArgs> FileSaving;

        public IFileObject this[string key]
        {
            get
            {
                return _dict[key];

            }

            set
            {
                FileChanging?.Invoke(this, new FileChangeingArgs(value));
                _dict[key] = value;
                FileChanged?.Invoke(this, new FileChangedArgs(value));
            }
        }
        /// <summary>
        /// Number of files in collection
        /// </summary>
        public int Count
        {
            get
            {
                return _dict.Count;
            }
        }
        public IDeltaSizeInfo WholeSize
        {
            get
            {
                var ds = new DeltaSizeInfo(DateTime.Now) { EndedAt = DateTime.Now };
                foreach(var f in this)
                {
                    ds = ds + (f?.DownloadInfo ?? new DeltaSizeInfo(DateTime.Now));
                }
                return ds;
            }
        }
        /// <summary>
        /// All of the Uris in the collection
        /// </summary>
        public ICollection<string> Uris
        {
            get
            {
                return _dict.Keys;
            }
        }
        /// <summary>
        /// All of the FileObjects in the collection
        /// </summary>
        public ICollection<IFileObject> Files
        {
            get
            {
                return _dict.Values;
            }
        }

        public FileObjectCollection()
        {

        }
        public FileObjectCollection(params string[] Uris)
        {
            string eMsg = string.Empty;
            foreach (var uri in Uris) {

                try
                {
                    this.Add(new FileObject(uri));
                }catch(Exception ex)
                {
                    eMsg += ex.Message + Environment.NewLine;
                }
            }
            if (eMsg != string.Empty)
                throw new Exception(eMsg);
        }
        /// <summary>
        /// Add File without downloading it
        /// </summary>
        /// <param name="item">File to add in collection</param>
        public void Add(IFileObject item)
        {
            if (item.Uri == null)
                throw new ArgumentNullException("Uri");
            if (string.IsNullOrWhiteSpace(item.Uri.AbsoluteUri))
                throw new ArgumentException("Uri path is not valid");
            if (_dict.ContainsKey(item.Uri.AbsoluteUri))
                throw new ArgumentException("Uri is alredy in the collection");

            FileAdding?.Invoke(this, new FileAddingArgs(item));
            _dict.Add(item.Uri.AbsoluteUri, item);
            item.FileDownloaded += OnFileDownloaded;
            item.FileDownloading += OnFileDownloading;
            item.UriChange += OnUriChange;
            item.FileRemoved += OnFileRemoved;
            item.FileSaved += OnFileSaved;
            item.FileSaving += OnFileSaving;
            FileAdded?.Invoke(this, new FileAddedArgs(item));

        }
        /// <summary>
        /// Add File without downloading it
        /// </summary>
        /// <param name="Uri">File Uri</param>
        public void Add(UriObject Uri)
        {
            var temp = new FileObject(Uri.AbsoluteUri);
            Add(temp);
        }
        /// <summary>
        /// Add File and downloade it
        /// </summary>
        /// <param name="Uri"></param>
        /// <returns></returns>
        public async Task AddAsync(UriObject Uri)
        {
            Add(Uri);
            var temp = _dict[Uri.AbsoluteUri];
            await temp.DownloadAsync();
        }
        /// <summary>
        /// Delets all of the Files in this collection from memory, not from disc.
        /// If you want to remove them from disc use ClearAsync
        /// </summary>
        public void Clear()
        {
            CollectionCleaning?.Invoke(this, EventArgs.Empty);
            _dict.Clear();
            CollectionCleaned?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Remove all of the Files in this collection from memory and from disc
        /// </summary>
        /// <returns>Number of deleted files</returns>
        public Task<int> ClearAsync()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Check if collection contains File
        /// </summary>
        /// <param name="item">File object</param>
        /// <returns></returns>
        public bool Contains(IFileObject item)
        {
            return Contains(item.Uri.AbsoluteUri);
        }
        /// <summary>
        /// Check if collection contains File
        /// </summary>
        /// <returns></returns>
        public bool Contains(string Uri)
        {
            return _dict.ContainsKey(Uri);
        }
        /// <summary>
        /// Check if collection contains File
        /// </summary>
        /// <returns></returns>
        public bool Contains(UriObject Uri)
        {
            return Contains(Uri.AbsoluteUri);
        }
        /// <summary>
        /// Remove File only from memory, not from disc, use RemoveAsync to remove the File from disc
        /// </summary>
        /// <param name="Uri">Uri of file to remove</param>
        public void Remove(string Uri)
        {
            if (_dict.ContainsKey(Uri))
            {
                var temp = _dict[Uri];
                FileRemoving?.Invoke(this, new FileRemovingArgs(temp));
                _dict.Remove(Uri);
                FileRemoved?.Invoke(this, new FileRemovedArgs(temp));
            }
            else
                throw new ArgumentException("Uri isn't in the collection");
        }
        /// <summary>
        /// Remove File only from memory, not from disc, use RemoveAsync to remove the File from disc
        /// </summary>
        /// <param name="item">File to remove</param>
        public void Remove(IFileObject item)
        {
            _dict.Remove(item.Uri.AbsoluteUri);
        }
        /// <summary>
        /// Removes File from memory and any copy of an File on disc, made only in current session
        /// 
        /// Side note: Maybe I shuld make it rembre where all the copys are in future versions
        /// </summary>
        /// <param name="item">File to remove</param>
        /// <returns>Number of deleted copys, not including the copy in memory</returns>
        public async Task<int> RemoveAsync(IFileObject item)
        {
            Remove(item);
            int count = 0;
            List<Task> tasks = new List<Task>();
            foreach (var path in item.PathsOnDisc)
            {
                Task tt = Task.Delay(5000);
                tasks.Add(Task.WhenAny(Task.Factory.StartNew(() =>
                {
                    try
                    {
                        System.IO.File.Delete(path);
                        count++;
                    }
                    catch
                    {

                    }
                }), tt));
            }
            await Task.WhenAll(tasks);
            return count;
        }
        /// <summary>
        /// Removes File from memory and any copy of an File on disc, made only in current session
        /// 
        /// Side note: Maybe I shuld make it rembre where all the copys are in future versions
        /// </summary>
        /// <param name="Uri">Uri of a file to remove</param>
        /// <returns>Number of deleted copys, not including the copy in memory</returns>
        public async Task<int> RemoveAsync(string Uri)
        {
            return await RemoveAsync(this[Uri]);
        }
        /// <summary>
        /// Download all of the files in the collection 
        /// </summary>
        /// <param name="force">Download the file again if the file is alredy downladed. Recomended is false</param>
        /// <returns>Number of downloaded files</returns>
        public async Task<int> DownloadAsync(bool force = false)
        {
            List<Task> Downs = new List<Task>(this.Count);
            int sum = 0;
            foreach (var file in this)
            {
                Task Down = Task.Factory.StartNew(() => {
                    if (force || file.RawFile == null)
                    {
                        Task.WaitAll(file.DownloadAsync());
                        if (file.Status == FileStatus.InMemory || file.Status == FileStatus.OnDiscInMemory)
                            sum++;
                    }
                    return;
                });
                Downs.Add(Down);
            }
            await Task.WhenAll(Downs.AsEnumerable());
            return sum;
        }
        /// <summary>
        /// Save all of the downloaded files on the disc. Files will be saved on the location home/{NameOfTheFile}
        /// </summary>
        /// <returns>Number of saved files</returns>
        public async Task<int> SaveAsync()
        {
            List<Task> Saves = new List<Task>(this.Count);
            int sum = 0;
            foreach (var file in this)
            {

                Task Save = Task.Factory.StartNew(() =>
                {
                    Task.WaitAll(file.SaveOnDiscAsync());
                    sum++;
                });
                Saves.Add(Save);
            }
            await Task.WhenAll(Saves);
            return sum;
        }
        /// <summary>
        /// Save all of the downloaded files on the disc. Files will be saved on the location Path/{NameOfTheFile}
        /// </summary>
        /// <param name="SaveLocation">Location on which files will be saved</param>
        /// <returns>Number of saved files</returns>
        public async Task<int> SaveAsync(string SaveLocation)
        {
            List<Task> Saves = new List<Task>(this.Count);
            int sum = 0;
            foreach (var file in this)
            {
                Task TimeOut = Task.Delay(10000);

                Task Save = Task.Factory.StartNew(() =>
                {
                    var ok = file.SaveOnDiscAsync(Path.Combine(SaveLocation, file.Name));
                    Task.WaitAll(ok);
                    if (file.Status == FileStatus.OnDisc || file.Status == FileStatus.OnDiscInMemory)
                        sum++;
                });
                Task full = Task.WhenAny(Save);
                Saves.Add(full);
            }
            await Task.WhenAll(Saves);
            return sum;
        }
        /// <summary>
        /// Download and save all of the files in the collection. Files will be saved on the location home/{NameOfTheFile}
        /// </summary>
        /// <param name="force">Force download files if the file is alredy downloaded</param>
        /// <returns>Number of saved files</returns>
        public async Task<int> DownloadAndSaveAsync(bool force = false)
        {
            return await DownloadAndSaveAsync(EnvironmentVariables.DownloadFolder, force);
        }
        /// <summary>
        /// Download and save all of the files in the collection. Files will be saved on the location home/{NameOfTheFile}
        /// </summary>
        /// <param name="SaveLocation">Folder in which the filed will be saved</param>
        /// <param name="force">Force download files if the file is alredy downloaded</param>
        /// <returns>Number of saved files</returns>
        public async Task<int> DownloadAndSaveAsync(string SaveLocation, bool force = false)
        {
            List<Task> Downs = new List<Task>(this.Count);
            int sum = 0;
            foreach (var file in this)
            {
                Task Down = Task.Factory.StartNew(() => {
                    if (force || file.RawFile == null)
                        Task.WaitAll(file.DownloadAsync());
                    Task.WaitAll(file.SaveOnDiscAsync(Path.Combine(SaveLocation, file.Name)));
                    if (file.Status == FileStatus.OnDisc || file.Status == FileStatus.OnDiscInMemory)
                        sum++;
                    return;
                });
                Downs.Add(Down);
            }
            await Task.WhenAll(Downs.ToArray());
            return sum;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }
        public IEnumerator<IFileObject> GetEnumerator()
        {
            foreach(var im in _dict)
            {
                yield return im.Value;
            }
        }

        #region EventHandle
        private void OnFileRemoved(object sender, FileRemovedArgs e)
        {
            _dict.Remove(e.File.Uri.AbsoluteUri);
            FileRemoved?.Invoke(this, e);
        }

        private void OnUriChange(object sender, UriChangedArgs e)
        {
            _dict.Remove(e.OldUri.AbsoluteUri);
            _dict.Add(e.NewUri.AbsoluteUri, sender as IFileObject);
        }

        private void OnFileDownloading(object sender, FileDownloadingArgs e)
        {
            FileDownloading?.Invoke(this, e);
        }

        private void OnFileDownloaded(object sender, FileDownloadedArgs e)
        {
            FileDownloaded?.Invoke(this, e);
        }

        private void OnFileSaving(object sender, FileSavingArgs e)
        {
            FileSaving?.Invoke(this, e);
        }

        private void OnFileSaved(object sender, FileSavedArgs e)
        {
            FileSaved?.Invoke(this, e); 
        }

        #endregion
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _dict = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                GC.SuppressFinalize(this);

            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FileObjectCollection() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        
        #endregion
    }
}
