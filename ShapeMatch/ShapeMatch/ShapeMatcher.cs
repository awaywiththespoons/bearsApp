using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;

namespace ShapeMatch
{

    public struct Circle
    {
        public double X, Y, Radius;

        public override string ToString()
        {
            return string.Format("X:{0}, Y:{1}, Radius:{2}", X, Y, Radius);
        }
    }


    public class ShapeMatcher
    {
        public readonly List<Circle> Circles = new List<Circle>();

        public const int Threshold = 12; // 64;

        public void Match(int width, int height, byte[] data)
        {

            Circles.Clear();

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {

                //Grayscale grayscale = new Grayscale(0.2125, 0.7154, 0.0721);

                //using (UnmanagedImage bitmapData = new UnmanagedImage(handle.AddrOfPinnedObject(), width, height, width * 4, PixelFormat.Format32bppArgb))
                //using (UnmanagedImage grayscaleImage = grayscale.Apply(bitmapData))
                ////using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                //{                    
                //    // lock image
                //    //BitmapData bitmapData = bitmap.LockBits(
                //    //    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                //    //    ImageLockMode.ReadWrite, bitmap.PixelFormat);

                //    //var ptr = bitmapData.Scan0;

                //    //Marshal.Copy(data, 0, ptr, data.Length);

                //    // step 1 - turn background to black
                //    //ColorFiltering colorFilter = new ColorFiltering();

                //    //colorFilter.Red = new IntRange(0, Threshold);
                //    //colorFilter.Green = new IntRange(0, Threshold);
                //    //colorFilter.Blue = new IntRange(0, Threshold);
                //    //colorFilter.FillOutsideRange = false;
                //    //colorFilter.ApplyInPlace(bitmapData);

                //    //colorFilter.FillColor = new RGB(255, 255, 255);
                //    //colorFilter.FillOutsideRange = true;
                //    //colorFilter.ApplyInPlace(bitmapData);

                //    //System.Drawing.Color cornerColor = bitmapData.GetPixel(0, 0);

                //    //Threshold threshold = new Threshold(cornerColor.R + 10);
                //    //threshold.ApplyInPlace(grayscaleImage);

                //    //// create filter
                //    //Erosion erosion = new Erosion();
                //    //// apply the filter
                //    //erosion.ApplyInPlace(grayscaleImage);
                //    //erosion.ApplyInPlace(grayscaleImage);
                //    //erosion.ApplyInPlace(grayscaleImage);

                //    //// create filter
                //    //Dilatation dilatation = new Dilatation();
                //    //// apply the filter
                //    //dilatation.ApplyInPlace(grayscaleImage);

                //    // step 2 - locating objects
                //    BlobCounter blobCounter = new BlobCounter();

                //    blobCounter.FilterBlobs = true;
                //    blobCounter.MinHeight = 10;
                //    blobCounter.MinWidth = 10;
                //    blobCounter.MaxWidth = 62;
                //    blobCounter.MaxHeight = 62;

                //    blobCounter.ProcessImage(grayscaleImage);

                //    Blob[] blobs = blobCounter.GetObjectsInformation();

                //    //bitmap.UnlockBits(bitmapData);

                //    // step 3 - check objects' type and highlight
                //    SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

                //    for (int i = 0, n = blobs.Length; i < n; i++)
                //    {
                //        Rectangle bounds = blobs[i].Rectangle;

                //        Circles.Add(new Circle()
                //        {
                //            X = (bounds.Width * 0.5f + bounds.X) / (float)width,
                //            Y = (bounds.Height * 0.5f + bounds.Y) / (float)height,
                //            Radius = 1,
                //        });

                //        /*List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);

                //        AForge.Point center;
                //        float radius;

                //        List<IntPoint> corners;

                //        // is circle ?
                //        if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                //        {
                //            Circles.Add(new Circle()
                //            {
                //                X = center.X,
                //                Y = center.Y,
                //                Radius = radius,
                //            });
                //        }
                //        else 
                //        {

                //            Circles.Add(new Circle()
                //            {
                //                X = 1,
                //                Y = 1,
                //                Radius = 1,
                //            });
                //        }
                //        */
                //    }
                //}
            }
            finally
            {
                handle.Free();
            }

        }
    }
}
