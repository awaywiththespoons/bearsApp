using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System;
using AForge.Imaging;
using System.Collections.Generic;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using AForge;
using AForge.Math.Geometry;
using AForge.Math;
using System.Drawing;

public class ImageScanner : MonoBehaviour
{
    ShapeMatcher matcher = new ShapeMatcher(); 

    WebCamTexture webCamTexture;
    byte[] colorBytes = new byte[0]; 

    [SerializeField]
    public bool UseWebCam = true;

    [SerializeField]
    public Texture2D TestImage;

    [SerializeField]
    public MeshRenderer CameraTest;

    [SerializeField]
    public Texture CurrentTexture;

    [SerializeField]
    public Text DebugText; 

    // Use this for initialization
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        WebCamDevice selected = default(WebCamDevice);

        bool found = false;

        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log(devices[i].name);

            if (devices[i].isFrontFacing == false)
            {
                continue;
            }

            selected = devices[i];
            found = true;
        }

        if (found == false && devices.Length > 0)
        {
            selected = devices[0];
            found = true; 
        }

        if (found == true)
        {
            webCamTexture = new WebCamTexture(selected.name, 128, 128, 10);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (UseWebCam == true && webCamTexture.isPlaying == false)
        {
            webCamTexture.Play();
        }
        else if (UseWebCam == false && webCamTexture.isPlaying == true)
        {
            webCamTexture.Stop();
        }

        //if (Input.GetButtonDown("Jump") == true)
        {
            TryProcessImage();
        }
    }

    private void TryProcessImage()
    {
        //GCHandle handle = default(GCHandle);

        try
        {
            if (UseWebCam == false)
            {
                CurrentTexture = TestImage;

                if (colorBytes.Length != (TestImage.width * 4) * TestImage.height)
                {
                    colorBytes = new byte[(TestImage.width * 4) * TestImage.height];
                }

                Color32[] colorData = TestImage.GetPixels32();

                GetArrayBytes(colorData, TestImage.width * TestImage.height, colorBytes);

                //handle = GCHandle.Alloc(colorBytes, GCHandleType.Pinned);

                matcher.Match(CurrentTexture.width, CurrentTexture.height, colorBytes); // handle.AddrOfPinnedObject());
            }
            else
            {
                CurrentTexture = webCamTexture;

                if (webCamTexture.didUpdateThisFrame == false)
                {
                    return;
                }

                if (colorBytes.Length != (webCamTexture.width * 4) * webCamTexture.height)
                {
                    colorBytes = new byte[(webCamTexture.width * 4) * webCamTexture.height];
                }

                Color32[] colorData = webCamTexture.GetPixels32();

                GetArrayBytes(colorData, webCamTexture.width * webCamTexture.height, colorBytes);

                //handle = GCHandle.Alloc(colorBytes, GCHandleType.Pinned);

                matcher.Match(CurrentTexture.width, CurrentTexture.height, colorBytes); // handle.AddrOfPinnedObject());
            }

            if (DebugText != null)
            {
                DebugText.text = matcher.Circles.Count.ToString();
            }
        }
        catch (Exception ex)
        {
            if (DebugText != null)
            {
                DebugText.text = ex.Message;
            }
        }
        finally
        {
            //if (handle.IsAllocated == true)
            //{
            //    handle.Free(); 
            //}

            if (CameraTest != null)
            {
                CameraTest.material.mainTexture = CurrentTexture;
            }
        }
    }

    private static void GetArrayBytes<T>(T[] array, int count, byte[] bytes)
    {
        int index = 0; 
        int size = Marshal.SizeOf(typeof(T));
        int totalSize = size * count;

        GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);

        try
        {
            Marshal.Copy(handle.AddrOfPinnedObject(), bytes, index, totalSize);
        }
        finally
        {
            handle.Free();
        }

        index += totalSize;
    }
}


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
            Grayscale grayscale = new Grayscale(0.2125, 0.7154, 0.0721);

            using (UnmanagedImage bitmapData = new UnmanagedImage(handle.AddrOfPinnedObject(), width, height, width * 4, PixelFormat.Format32bppArgb))
            using (UnmanagedImage grayscaleImage = grayscale.Apply(bitmapData))
            //using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                // lock image
                //BitmapData bitmapData = bitmap.LockBits(
                //    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                //    ImageLockMode.ReadWrite, bitmap.PixelFormat);

                //var ptr = bitmapData.Scan0;

                //Marshal.Copy(data, 0, ptr, data.Length);

                // step 1 - turn background to black
                //ColorFiltering colorFilter = new ColorFiltering();

                //colorFilter.Red = new IntRange(0, Threshold);
                //colorFilter.Green = new IntRange(0, Threshold);
                //colorFilter.Blue = new IntRange(0, Threshold);
                //colorFilter.FillOutsideRange = false;
                //colorFilter.ApplyInPlace(bitmapData);

                //colorFilter.FillColor = new RGB(255, 255, 255);
                //colorFilter.FillOutsideRange = true;
                //colorFilter.ApplyInPlace(bitmapData);
                
                System.Drawing.Color cornerColor = bitmapData.GetPixel(0, 0);

                Threshold threshold = new Threshold(cornerColor.R + 10);
                threshold.ApplyInPlace(grayscaleImage);

                //// create filter
                //Erosion erosion = new Erosion();
                //// apply the filter
                //erosion.ApplyInPlace(grayscaleImage);
                //erosion.ApplyInPlace(grayscaleImage);
                //erosion.ApplyInPlace(grayscaleImage);

                //// create filter
                //Dilatation dilatation = new Dilatation();
                //// apply the filter
                //dilatation.ApplyInPlace(grayscaleImage);

                // step 2 - locating objects
                BlobCounter blobCounter = new BlobCounter();

                blobCounter.FilterBlobs = true;
                blobCounter.MinHeight = 20;
                blobCounter.MinWidth = 20;
                blobCounter.MaxWidth = 42;
                blobCounter.MaxHeight = 42;

                blobCounter.ProcessImage(grayscaleImage);

                Blob[] blobs = blobCounter.GetObjectsInformation();

                //bitmap.UnlockBits(bitmapData);

                // step 3 - check objects' type and highlight
                SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

                for (int i = 0, n = blobs.Length; i < n; i++)
                {
                    Rectangle bounds = blobs[i].Rectangle;

                    Circles.Add(new Circle()
                    {
                        X = (bounds.Width * 0.5f + bounds.X) / (float)width,
                        Y = (bounds.Height * 0.5f + bounds.Y) / (float)height,
                        Radius = 1,
                    });

                    /*List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);

                    AForge.Point center;
                    float radius;

                    List<IntPoint> corners;

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
                    else 
                    {
                        
                        Circles.Add(new Circle()
                        {
                            X = 1,
                            Y = 1,
                            Radius = 1,
                        });
                    }
                    */
                }
            }
        }
        finally
        {
            handle.Free();
        }

    }
}


//public class ShapeMatcher
//{
//    public readonly List<Circle> Circles = new List<Circle>();

//    public void Match(int width, int height, IntPtr data)
//    {

//        Circles.Clear();

//        try
//        {
//            using (UnmanagedImage bitmapData = new UnmanagedImage(data, width, height, width * 4, PixelFormat.Format32bppArgb))
//            {
//                // step 1 - turn background to black
//                ColorFiltering colorFilter = new ColorFiltering();

//                colorFilter.Red = new IntRange(0, 12);
//                colorFilter.Green = new IntRange(0, 12);
//                colorFilter.Blue = new IntRange(0, 12);
//                colorFilter.FillOutsideRange = false;

//                colorFilter.ApplyInPlace(bitmapData);

//                /*
//                // create filter
//                Erosion erosion = new Erosion();
//                // apply the filter
//                erosion.ApplyInPlace(bitmapData);

//                // create filter
//                Dilatation dilatation = new Dilatation();
//                // apply the filter
//                dilatation.ApplyInPlace(bitmapData);
//                */ 

//                // create filter
//                //Threshold filter = new Threshold(64);
//                // apply the filter
//                //filter.ApplyInPlace(bitmapData);

//                // step 2 - locating objects
//                BlobCounter blobCounter = new BlobCounter();

//                blobCounter.FilterBlobs = true;
//                blobCounter.MinHeight = 5;
//                blobCounter.MinWidth = 5;

//                blobCounter.ProcessImage(bitmapData);

//                Blob[] blobs = blobCounter.GetObjectsInformation();

//                // step 3 - check objects' type and highlight
//                SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

//                for (int i = 0, n = blobs.Length; i < n; i++)
//                {
//                    List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);

//                    AForge.Point center;
//                    float radius;

//                    // is circle ?
//                    if (shapeChecker.IsCircle(edgePoints, out center, out radius))
//                    {
//                        Circles.Add(new Circle()
//                        {
//                            X = center.X,
//                            Y = center.Y,
//                            Radius = radius,
//                        });
//                    }
//                }
//            }
//        }
//        finally
//        {
//        }
//    }
//}
