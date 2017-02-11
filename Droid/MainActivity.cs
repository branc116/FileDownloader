using Android.App;
using Android.Widget;
using Android.OS;
using FileDownloader;

namespace Droid
{
    [Activity(Label = "Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            Button b = FindViewById<Button>(Resource.Id.button1);
            TextView prog = FindViewById<TextView>(Resource.Id.textView2);
            SeekBar seekBar = FindViewById<SeekBar>(Resource.Id.seekBar1);
            TextView textView = FindViewById<TextView>(Resource.Id.textView1);
            int n = 0;
            FileDownloader.Collections.FileObjectCollection Collect= new FileDownloader.Collections.FileObjectCollection("http://www.w3schools.com/css/trolltunga.jpg",
                                                                                                                  "https://www.dialogfeed.com/wp-content/uploads/2016/10/links-1.jpg",
                                                                                                                  "http://lafourche911.com/wp-content/uploads/2013/10/iStock_000011579427XSmall.jpg",
                                                                                                                  "http://onthewatersydney.com/yahoo_site_admin/assets/images/links.150173011_std.jpg",
                                                                                                                  "http://onthewatersydney.com/yahoo_site_admin/assets/images/08025-168_copy.15045451.jpg",
                                                                                                                  "http://onthewatersydney.com/yahoo_site_admin/assets/images/2-CUMMINS-GENUINE-PARTS.150174932_sq_thumb_s.jpg",
                                                                                                                  "http://onthewatersydney.com/yahoo_site_admin/assets/images/769.15024718.jpg",
                                                                                                                  "http://onthewatersydney.com/yahoo_site_admin/assets/images/98390308.150135709_large.png",
                                                                                                                  "http://onthewatersydney.com/yahoo_site_admin/assets/images/Contact-4.150142434.jpg",
                                                                                                                  "http://eoimages.gsfc.nasa.gov/images/imagerecords/77000/77085/marble_east_vir_2012023_lrg.jpg",
                                                                                                                  "https://www.seedr.cc/zip/7841897?st=2d4c3c0e0e263a334d6c5b305c65c9c032f179e737c1b5428fc642c31b5a5f7d&e=1481582304");
            Collect.FileDownloading += (sender, args) =>
            {
                var inf = Collect.WholeSize;
                RunOnUiThread(() =>
                {
                    seekBar.Progress =  (int)((inf.CurrentSizeBytes + 1) / (float)(inf.SizeBytes + 1) * 100 );
                    prog.Text = inf.ToString();
                    textView.Text = n.ToString();    
                });

                n++;
            };
            b.Click += async (sender, args) =>
            {
                await Collect.DownloadAsync(true);
            };

            // Set our view from the "main" layout resource
            
        }
        
    }
}

