## File Downloader

#### This project help with downloading files from net in .net standard

#### Requirements are .net standard v1.4+ 

#### NuGet pack is comming soon, for now clone the src if you want to use the lib

### It's quite easy to use it:

```
using (var file = new FileDownloader.Objects.FileObject(Uri))
{
    await file.DownloadAsync();
    await file.SaveOnDiscAsync();
}
```
### Or:
```
using (var file = new FileDownloader.Collections.FileObjectCollection(Uris))
{
    await file.DownloadAndSaveAsync();
}
```
### Or:
```
using (var file = new FileDownloader.Collections.FileObjectCollection(Uris))
{
    await file.DownloadAndSaveAsync(Path);
}
```
### Or:
```
using (var file = new FileDownloader.Collections.FileObjectCollection(Uris))
{
    await file.DownloadAsync();
    await file.SaveAsync(Path);
}
```
### Or:
...

#### Enjoy!