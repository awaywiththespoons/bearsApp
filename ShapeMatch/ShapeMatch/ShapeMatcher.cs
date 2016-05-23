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

        public void Match(int width, int height, byte[] data)
        {

            Circles.Clear();

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                using (UnmanagedImage bitmapData = new UnmanagedImage(handle.AddrOfPinnedObject(), width, height, width * 4, PixelFormat.Format32bppArgb))
                //using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                {
                    // lock image
                    //BitmapData bitmapData = bitmap.LockBits(
                    //    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    //    ImageLockMode.ReadWrite, bitmap.PixelFormat);

                    //var ptr = bitmapData.Scan0;

                    //Marshal.Copy(data, 0, ptr, data.Length);

                    // step 1 - turn background to black
                    ColorFiltering colorFilter = new ColorFiltering();

                    colorFilter.Red = new IntRange(0, 64);
                    colorFilter.Green = new IntRange(0, 64);
                    colorFilter.Blue = new IntRange(0, 64);
                    colorFilter.FillOutsideRange = false;

                    colorFilter.ApplyInPlace(bitmapData);

                    // step 2 - locating objects
                    BlobCounter blobCounter = new BlobCounter();

                    blobCounter.FilterBlobs = true;
                    blobCounter.MinHeight = 5;
                    blobCounter.MinWidth = 5;

                    blobCounter.ProcessImage(bitmapData);

                    Blob[] blobs = blobCounter.GetObjectsInformation();

                    //bitmap.UnlockBits(bitmapData);

                    // step 3 - check objects' type and highlight
                    SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

                    for (int i = 0, n = blobs.Length; i < n; i++)
                    {
                        List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);

                        AForge.Point center;
                        float radius;

                        // is circle ?
                        if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                        {
                            Circles.Add(new Circle()
                            {
                                X = center.X,
                                Y = center.Y,
                                Radius = radius,
                            });
                        }
                    }
                }
            }
            finally
            {
                handle.Free();
            }

        }
    }
}
