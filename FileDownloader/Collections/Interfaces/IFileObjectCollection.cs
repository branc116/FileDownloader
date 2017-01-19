using FileDownloader.Args;
using FileDownloader.Objects.Helper;
using FileDownloader.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloader.Collections.Interfaces
{
    interface IFileObjectCollection : IDisposable, IEnumerable<IFileObject>
    {
        event EventHandler<FileAddingArgs> FileAdding;
        event EventHandler<FileAddedArgs> FileAdded;
        /// <summary>
        /// Called when File is about to be downloaded
        /// </summary>
        event EventHandler<FileDownloadingArgs> FileDownloading;
        /// <summary>
        /// Called when File has been downloaded
        /// </summary>
        event EventHandler<FileDownloadedArgs> FileDownloaded;
        /// <summary>
        /// Called when File is about to be removed to the collection
        /// </summary>
        event EventHandler<FileRemovingArgs> FileRemoving;
        /// <summary>
        /// Called when File has been removed from the collection
        /// </summary>
        event EventHandler<FileRemovedArgs> FileRemoved;
        event EventHandler<FileChangeingArgs> FileChanging;
        event EventHandler<FileChangedArgs> FileChanged;
        /// <summary>
        /// Called when the whole collection is about to be emptyed
        /// </summary>
        event EventHandler<EventArgs> CollectionCleaning;
        /// <summary>
        /// Called when the whole collection has been emptyed
        /// </summary>
        event EventHandler<EventArgs> CollectionCleaned;
        /// <summary>
        /// Called when a FileObject has successfuly save a copy of itselfe on disc
        /// </summary>
        event EventHandler<FileSavedArgs> FileSaved;
        /// <summary>
        /// Called when a FileObject has stated a process of saving itselfe on disc
        /// </summary>
        event EventHandler<FileSavingArgs> FileSaving;
        IFileObject this[string Key] { get;set; }
        /// <summary>
        /// Number of files in collection
        /// </summary>
        int Count { get; }
        /// <summary>
        /// All of the Uris in the collection
        /// </summary>
        ICollection<string> Uris { get; }
        /// <summary>
        /// All of the FileObjects in the collection
        /// </summary>
        ICollection<IFileObject> Files { get; }
        /// <summary>
        /// Add File without downloading it
        /// </summary>
        /// <param name="item">File to add in collection</param>
        void Add(IFileObject item);
        /// <summary>
        /// Add File without downloading it
        /// </summary>
        /// <param name="Uri">File Uri</param>
        void Add(UriObject Uri);
        /// <summary>
        /// Add File and downloade it
        /// </summary>
        /// <param name="Uri"></param>
        /// <returns></returns>
        Task AddAsync(UriObject Uri);
        /// <summary>
        /// Delets all of the Files in this collection from memory, not from disc.
        /// If you want to remove them from disc use ClearAsync
        /// </summary>
        void Clear();
        /// <summary>
        /// Remove all of the Files in this collection from memory and from disc
        /// </summary>
        /// <returns>Number of deleted files</returns>
        Task<int> ClearAsync();
        /// <summary>
        /// Check if collection contains File
        /// </summary>
        /// <param name="item">File object</param>
        /// <returns></returns>
        bool Contains(IFileObject item);
        /// <summary>
        /// Check if collection contains File
        /// </summary>
        /// <returns></returns>
        bool Contains(UriObject Uri);
        /// <summary>
        /// Check if collection contains File
        /// </summary>
        /// <returns></returns>
        bool Contains(string Uri);
        /// <summary>
        /// Remove File only from memory, not from disc, use RemoveAsync to remove the File from disc
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        void Remove(string Uri);
        /// <summary>
        /// Remove File only from memory, not from disc, use RemoveAsync to remove the File from disc
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        void Remove(IFileObject item);
        /// <summary>
        /// Removes File from memory and any copy of an File on disc, made only in current session
        /// 
        /// Side note: Maybe I shuld make it rembre where all the copys are in future versions
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Number of deleted copys, not including the copy in memory</returns>
        Task<int> RemoveAsync(IFileObject item);
        /// <summary>
        /// Removes File from memory and any copy of an File on disc, made only in current session
        /// 
        /// Side note: Maybe I shuld make it rembre where all the copys are in future versions
        /// </summary>
        /// <param name="Uri"></param>
        /// <returns>Number of deleted copys, not including the copy in memory</returns>
        Task<int> RemoveAsync(string Uri);
        /// <summary>
        /// Download all of the files in the collection 
        /// </summary>
        /// <param name="force">Download the file again if the file is alredy downladed. Recomended is false</param>
        /// <returns>Number of downloaded files</returns>
        Task<int> DownloadAsync(bool force = false);
        /// <summary>
        /// Save all of the downloaded files on the disc. Files will be saved on the location home/{NameOfTheFile}
        /// </summary>
        /// <returns>Number of saved files</returns>
        Task<int> SaveAsync();
        /// <summary>
        /// Save all of the downloaded files on the disc. Files will be saved on the location Path/{NameOfTheFile}
        /// </summary>
        /// <param name="SaveLocation">Location on which files will be saved</param>
        /// <returns>Number of saved files</returns>
        Task<int> SaveAsync(string SaveLocation);
        /// <summary>
        /// Download and save all of the files in the collection. Files will be saved on the location home/{NameOfTheFile}
        /// </summary>
        /// <param name="force"></param>
        /// <returns>Number of saved files</returns>
        Task<int> DownloadAndSaveAsync(bool force = false);
        /// <summary>
        /// Download and save all of the files in the collection. Files will be saved on the location home/{NameOfTheFile}
        /// </summary>
        /// <param name="force"></param>
        /// <returns>Number of saved files</returns>
        Task<int> DownloadAndSaveAsync(string SaveLocation, bool force = false);
    }
}