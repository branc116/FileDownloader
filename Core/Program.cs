using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using FileDownloader;
using System.Threading.Tasks;
using FileDownloader.Objects;
using FileDownloader.Collections;
using System.IO;
using FileDownloader.Args;
using FileDownloader.Enums;
using ConsoleTables.Core;

class Program
{
    static void Main(string[] args)
    {
        var tests = new Tests();
        Task t;
        //t = tests.TestOneDownloadAsync("https://cdn.spacetelescope.org/archives/images/large/heic1509a.jpg");
        //Console.ReadLine();
        t = tests.DownloadCollectionAsync(//"http://www.w3schools.com/css/trolltunga.jpg",
                                          //"https://www.dialogfeed.com/wp-content/uploads/2016/10/links-1.jpg",
                                          //"http://lafourche911.com/wp-content/uploads/2013/10/iStock_000011579427XSmall.jpg",
                                          //"http://onthewatersydney.com/yahoo_site_admin/assets/images/links.150173011_std.jpg",
                                          //"http://onthewatersydney.com/yahoo_site_admin/assets/images/08025-168_copy.15045451.jpg",
                                          //"http://onthewatersydney.com/yahoo_site_admin/assets/images/2-CUMMINS-GENUINE-PARTS.150174932_sq_thumb_s.jpg",
                                          //"http://onthewatersydney.com/yahoo_site_admin/assets/images/769.15024718.jpg",
                                          //"http://onthewatersydney.com/yahoo_site_admin/assets/images/98390308.150135709_large.png",
                                          //"http://onthewatersydney.com/yahoo_site_admin/assets/images/Contact-4.150142434.jpg",
                                          //"http://eoimages.gsfc.nasa.gov/images/imagerecords/77000/77085/marble_east_vir_2012023_lrg.jpg",
                                          "https://www.seedr.cc/zip/7841897?st=2d4c3c0e0e263a334d6c5b305c65c9c032f179e737c1b5428fc642c31b5a5f7d&e=1481582304");
        Console.ReadLine();
    }

}
public class Tests
{
    private ConsoleTable _table;

    public Tests()
    {
        _table = new ConsoleTables.Core.ConsoleTable("Name", "Down Speed", "Write Speed", "Status", "Size", "CurrentSize");
    }
    public async Task TestOneDownloadAsync(string Uri)
    {
        //var filsz = new FileObjectCollection();
        using (var file = new FileObject(Uri))
        {
            var ok = await file.DownloadAsync();
            file.DiscSizeChanged += File_DiscSizeChanged;
            Console.WriteLine($"Download {(ok == DownloadStatus.Ok ? "ok" : "not ok")}{(ok == DownloadStatus.Ok ? ", " + file.DownloadInfo : "")}");
            var oki = await file.SaveOnDiscAsync();
        }
    }
    public async Task DownloadCollectionAsync(params string[] Uris)
    {
        using (var file = new FileObjectCollection(Uris))
        {
            for (int i = 0; i < Uris.Length; i++)
            {
                var temp = file[Uris[i]];
                temp.Tag = i;
                _table.AddRow(temp.Name, 0, 0, temp.Status, 0, 0);
            }
            file.FileDownloaded += File_FileDownloaded;
            file.FileSaved += File_FileSaved;
            file.FileDownloading += File_FileDownloading;
            file.FileSaving += File_FileSaving;
            Task t1 = file.DownloadAndSaveAsync(true);
            Task t2 = Task.Factory.StartNew(async () =>
           {
               while (!(t1.IsCompleted))
               {

                   Console.Clear();
                   Console.SetCursorPosition(0, 0);
                   Console.WriteLine(_table.ToStringAlternative());
                   await Task.Delay(1000);
               }
           });
            await Task.Factory.StartNew(async () =>
            {
               await Task.WhenAll(t1, t2).ContinueWith(async (t) =>
               {
                   await Task.Delay(100);
                   Console.Clear();
                   Console.SetCursorPosition(0, 0);
                   Console.WriteLine(_table.ToStringAlternative());
                   Console.WriteLine("Finished!");
               });
            });
        }
    }

    private void File_FileSaving(object sender, FileSavingArgs e)
    {
        ChangeTable((int)e.File.Tag, 3, "Saving", true);
    }

    private void File_FileDownloading(object sender, FileDownloadingArgs e)
    {
        ChangeTable((int)e.File.Tag, 3, "Downloading");
        ChangeTable((int)e.File.Tag, 1, e.DownloadInfo.SpeedAuto);
        ChangeTable((int)e.File.Tag, 5, e.DownloadInfo.CurrentSizeAuto);
        ChangeTable((int)e.File.Tag, 4, e.DownloadInfo.SizeAuto);
    }

    private void File_FileSaved(object sender, FileSavedArgs e)
    {
        ChangeTable((int)e.File.Tag, 3, "Saved");
        ChangeTable((int)e.File.Tag, 2, e.File.SaveInfo.SpeedAuto);
        ChangeTable((int)e.File.Tag, 4, e.File.SaveInfo.SizeAuto, true);
    }

    private void File_FileDownloaded(object sender, FileDownloadedArgs e)
    {
        
        ChangeTable((int)e.File.Tag, 3, "Downloaded");
        ChangeTable((int)e.File.Tag, 1, e.File.DownloadInfo.SpeedAuto);
        ChangeTable((int)e.File.Tag, 4, e.File.DownloadInfo.SizeAuto, true);
        //_table.Rows[(int)e.File.Tag][3] = "Saving";
        //_table.Rows[(int)e.File.Tag][1] = e.File.DownloadInfo.SpeedAuto;
        //_table.Rows[(int)e.File.Tag][4] = e.File.DownloadInfo.SizeAuto;
        //Console.WriteLine(_table.ToStringAlternative());
    }

    private void File_DiscSizeChanged(object sender, DiscSizeArgs e)
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.WriteLine($"Saving: {e.File.Uri.Query}, {e.Info}");
    }
    private void ChangeTable(int i, int j, object o, bool print = false)
    {
        lock (this)
        {
            _table.Rows[i][j] = o;
            //if (print)
            //    Console.WriteLine(_table.ToStringAlternative());
        }
    }
}