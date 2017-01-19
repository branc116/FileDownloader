using System;
using System.Collections.Generic;
using System.Text;

namespace FileDownloader.Objects.Helper
{
    public class UriObject : Uri
    {
        public UriObject(string Uri) : base(Uri)
        {

        }
        public override int GetHashCode()
        {
            return base.AbsoluteUri.GetHashCode();
        }
    }
}
