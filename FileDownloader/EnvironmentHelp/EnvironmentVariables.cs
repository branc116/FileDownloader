using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileDownloader.EnvironmentHelp
{
    public static class EnvironmentVariables
    {
        public static string Home
        {
            get
            {
                return (Environment.GetEnvironmentVariable("USERPROFILE") ?? Environment.GetEnvironmentVariable("HOME") ?? "DUNNO").ToString();
                
            }
        }
        public static string DownloadFolder
        {
            get
            {
                return Path.Combine(Home, "Downloads");
            }
        }
    }
}