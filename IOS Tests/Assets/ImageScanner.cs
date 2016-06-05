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
using ShapeMatch;

public class ImageScanner : MonoBehaviour
{
    public struct Circle
    {
        public double X, Y, Radius;

        public override string ToString()
        {
            return string.Format("X:{0}, Y:{1}, Radius:{2}", X, Y, Radius);
        }
    }

    //public class MatchTemplate 
    //{
    //    public readonly bool[] Holes;

    //    public int PageIndex;

    //    public string Name; 

    //    public MatchTemplate(string name, int pageIndex, int[] holes)
    //    {
    //        Name = name; 
    //        PageIndex = pageIndex;

    //        Holes = new bool[holes.Length];

    //        for (int i = 0; i < holes.Length; i++)
    //        {
    //            Holes[i] = holes[i] > 0;
    //        }
    //    }
    //}


    //public class Matcher 
    //{
    //    List<MatchTemplate> matches = new List<MatchTemplate>();

    //    public readonly List<Circle> Circles = new List<Circle>(); 

    //    public Matcher()
    //    {
    //        matches.Add(new MatchTemplate("Tree", 1, 
    //            new int[] {
    //                0, 1, 1,
    //                1, 1, 0,
    //                0, 0, 1
    //            }));

    //        matches.Add(new MatchTemplate("Weed", 2,
    //            new int[] {
    //                1, 1, 1,
    //                1, 1, 0,
    //                1, 0, 1
    //            }));

    //        matches.Add(new MatchTemplate("House", 3,
    //            new int[] {
    //                0, 1, 1,
    //                1, 0, 1,
    //                0, 1, 1
    //            }));

    //        matches.Add(new MatchTemplate("Dune", 4,
    //            new int[] {
    //                1, 1, 1,
    //                0, 0, 0,
    //                1, 1, 1
    //            }));

    //        matches.Add(new MatchTemplate("Stalk", 5,
    //            new int[] {
    //                1, 1, 1,
    //                1, 1, 1,
    //                0, 1, 1
    //            }));
    //    }

    //    public void AddCircles(ShapeMatcher matcher)
    //    {
    //        Circles.AddRange(matcher.Circles); 
    //    }

    //    public void TryResolvePage(out string name, out int pageIndex, out float confidence)
    //    {
    //        float consol = 

    //        List<Circle> circles = new List<Circle>();


    //    }
    //}

    public class ShapeMatcher
    {
        public readonly List<Circle> Circles = new List<Circle>();

        public const int Threshold = 12; // 64;

        public void Match(int thresholdValue, int width, int height, byte[] data)
        {

            Circles.Clear();

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                using (UnmanagedImage bitmapData = new UnmanagedImage(handle.AddrOfPinnedObject(), width, height, width, PixelFormat.Format8bppIndexed))
                {
                    if (thresholdValue == 0)
                    {
                        System.Drawing.Color cornerColor = bitmapData.GetPixel(0, 0);

                        thresholdValue = cornerColor.R + 10;
                    }

                    Threshold threshold = new Threshold(thresholdValue); //  cornerColor.R + 10);
                    threshold.ApplyInPlace(bitmapData);
                    
                    BlobCounter blobCounter = new BlobCounter();

                    blobCounter.FilterBlobs = true;
                    blobCounter.MinHeight = 10;
                    blobCounter.MinWidth = 10;
                    blobCounter.MaxWidth = 60;
                    blobCounter.MaxHeight = 60;
                    
                    blobCounter.ProcessImage(bitmapData);

                    Blob[] blobs = blobCounter.GetObjectsInformation();
                    
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
                    }
                }
            }
            finally
            {
                handle.Free();
            }

        }
    }

    ShapeMatcher matcher = new ShapeMatcher(); 

    WebCamTexture webCamTexture;
    byte[] colorBytes = new byte[0];
    byte[] tempColorBytes = new byte[0];

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

                //if (colorBytes.Length != (TestImage.width * 4) * TestImage.height)
                //{
                //    colorBytes = new byte[(TestImage.width * 4) * TestImage.height];
                //}

                if (colorBytes.Length != TestImage.width * TestImage.height)
                {
                    colorBytes = new byte[TestImage.width * TestImage.height];
                    tempColorBytes = new byte[colorBytes.Length]; 
                }

                Color32[] colorData = TestImage.GetPixels32();

                GetArrayBytes_Greyscale(colorData, TestImage.width * TestImage.height, colorBytes);

                //handle = GCHandle.Alloc(colorBytes, GCHandleType.Pinned);
            }
            else
            {
                CurrentTexture = webCamTexture;

                if (webCamTexture.didUpdateThisFrame == false)
                {
                    return;
                }

                //if (colorBytes.Length != (webCamTexture.width * 4) * webCamTexture.height)
                //{
                //    colorBytes = new byte[(webCamTexture.width * 4) * webCamTexture.height];
                //}

                if (colorBytes.Length != webCamTexture.width * webCamTexture.height)
                {
                    colorBytes = new byte[webCamTexture.width * webCamTexture.height];
                    tempColorBytes = new byte[colorBytes.Length];
                }

                Color32[] colorData = webCamTexture.GetPixels32();

                GetArrayBytes_Greyscale(colorData, webCamTexture.width * webCamTexture.height, colorBytes);

                //handle = GCHandle.Alloc(colorBytes, GCHandleType.Pinned);
            }

            int countMax = 0;

            Array.Copy(colorBytes, tempColorBytes, colorBytes.Length); 
            matcher.Match(0, CurrentTexture.width, CurrentTexture.height, tempColorBytes); // handle.AddrOfPinnedObject());
            countMax = Mathf.Max(countMax, matcher.Circles.Count);
            
            Array.Copy(colorBytes, tempColorBytes, colorBytes.Length);
            matcher.Match(50, CurrentTexture.width, CurrentTexture.height, tempColorBytes); // handle.AddrOfPinnedObject());
            countMax = Mathf.Max(countMax, matcher.Circles.Count);
            
            Array.Copy(colorBytes, tempColorBytes, colorBytes.Length);
            matcher.Match(100, CurrentTexture.width, CurrentTexture.height, tempColorBytes); // handle.AddrOfPinnedObject());
            countMax = Mathf.Max(countMax, matcher.Circles.Count);

            if (DebugText != null)
            {
                DebugText.text = countMax.ToString(); //  matcher.Circles.Count.ToString();
            }

            print(countMax.ToString()); 
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

    private static void GetArrayBytes_Greyscale(Color32[] array, int count, byte[] bytes)
    {
        for (int i = 0; i < array.Length; i++)
        {
            bytes[i] = Max(array[i]); 
        }
    }

    private static byte Max(Color32 color32)
    {
        //return (byte)(((float)color32.r + (float)color32.g + (float)color32.b) * 0.33333f);// (byte)Mathf.Max(color32.r, color32.g, color32.b); 
        return (byte)(((float)color32.g + (float)color32.b) * 0.5f);// (byte)Mathf.Max(color32.r, color32.g, color32.b); 
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

