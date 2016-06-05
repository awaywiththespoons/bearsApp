using System;
using System.Collections.Generic;
using System.Text;

namespace System.Drawing
{
    public class Image
    {
        public static int GetPixelFormatSize(System.Drawing.Imaging.PixelFormat pixfmt)
        {
            return ((((int)pixfmt) >> 8) & 0xff);
        }
    }
}
