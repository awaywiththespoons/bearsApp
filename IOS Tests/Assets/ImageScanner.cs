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

    public class MatchTemplate
    {
        public readonly bool[] Holes;

        public int PageIndex;

        public string Name;

        public MatchTemplate(string name, int pageIndex, int[] holes)
        {
            Name = name;
            PageIndex = pageIndex;

            Holes = new bool[holes.Length];

            for (int i = 0; i < holes.Length; i++)
            {
                Holes[i] = holes[i] > 0;
            }
        }
    }


    public class Matcher
    {
        List<MatchTemplate> matchTemplates = new List<MatchTemplate>();

        public readonly List<Circle> Circles = new List<Circle>();

        public Matcher()
        {

            //matchTemplates.Add(new MatchTemplate("Tree", 1,
            //    new int[] {
            //        1, 1, 1,
            //        1, 1, 0,
            //        0, 0, 1
            //    }));

            matchTemplates.Add(new MatchTemplate("Tree", 1,
                new int[] {
                    0, 1, 1,
                    1, 1, 0,
                    0, 0, 1
                }));

            matchTemplates.Add(new MatchTemplate("Weed", 2,
                new int[] {
                    1, 1, 1,
                    1, 1, 0,
                    1, 0, 1
                }));

            matchTemplates.Add(new MatchTemplate("House", 3,
                new int[] {
                    0, 1, 1,
                    1, 0, 1,
                    0, 1, 1
                }));

            //matchTemplates.Add(new MatchTemplate("Dune", 4,
            //    new int[] {
            //        1, 1, 1,
            //        0, 0, 0,
            //        1, 1, 1
            //    }));

            //matchTemplates.Add(new MatchTemplate("Dune", 4,
            //    new int[] {
            //        1, 1, 1,
            //        1, 1, 1,
            //        0, 0, 0,
            //    }));
            //matchTemplates.Add(new MatchTemplate("Dune", 4,
            //    new int[] {
            //        1, 0, 1,
            //        1, 0, 1,
            //        1, 0, 1,
            //    }));

            matchTemplates.Add(new MatchTemplate("Dune", 4,
                new int[] {
                    1, 1, 0,
                    1, 1, 0,
                    1, 1, 0,
                }));

            matchTemplates.Add(new MatchTemplate("Stalk", 5,
                new int[] {
                    1, 0, 1,
                    0, 1, 0,
                    1, 0, 1
                }));

            //matchTemplates.Add(new MatchTemplate("Stalk", 5,
            //    new int[] {
            //        1, 1, 0,
            //        1, 1, 1,
            //        1, 1, 1
            //    }));
            //matchTemplates.Add(new MatchTemplate("Stalk", 5,
            //    new int[] {
            //        1, 0, 1,
            //        0, 1, 0,
            //        0, 0, 1
            //    }));

            //matchTemplates.Add(new MatchTemplate("Stalk", 5,
            //    new int[] {
            //        0, 0, 1,
            //        0, 1, 0,
            //        1, 0, 1
            //    }));
        }

        public void AddCircles(ShapeMatcher matcher)
        {
            Circles.AddRange(matcher.Circles);
        }

        public void TryResolvePage(out string name, out int pageIndex, out float confidence, out int circleCount)
        {
            float consolidateDistanceX = 0.111111f;
            float consolidateDistanceY = 0.1484375f * 1.2f;

            List<Circle> originalCircles = new List<Circle>(Circles);
            List<Circle> circles = new List<Circle>();

            confidence = 0;
            pageIndex = -1;
            name = "Unknown";

            //for (int i = originalCircles.Count - 1; i >= 0; i--)
            while (originalCircles.Count > 0)
            {                
                Circle circle = originalCircles[originalCircles.Count - 1];

                originalCircles.RemoveAt(originalCircles.Count - 1); 

                for (int j = originalCircles.Count - 1; j >= 0; j--)
                {
                    Circle other = originalCircles[j];

                    if (Math.Abs(other.X - circle.X) > consolidateDistanceX ||
                        Math.Abs(other.Y - circle.Y) > consolidateDistanceY)
                    {
                        continue; 
                    }

                    originalCircles.RemoveAt(j); 
                }

                circles.Add(circle); 
            }

            float[] gridXOffsets = new float[3];
            float[] gridYOffsets = new float[3];

            float lastOffset = -1; 
            for (int x = 0; x < 3; x++)
            {
                float min = float.MaxValue; 

                foreach (Circle circle in circles)
                {
                    if (circle.X < lastOffset + consolidateDistanceX)
                    {
                        continue; 
                    }

                    min = Mathf.Min(min, (float)circle.X);
                }

                gridXOffsets[x] = min;
                lastOffset = min; 
            }

            lastOffset = -1;
            for (int y = 0; y < 3; y++)
            {
                float min = float.MaxValue;

                foreach (Circle circle in circles)
                {
                    if (circle.Y < lastOffset + consolidateDistanceY)
                    {
                        continue;
                    }

                    min = Mathf.Min(min, (float)circle.Y);
                }

                gridYOffsets[y] = min;
                lastOffset = min;
            }

            bool[] shape = new bool[9];

            for (int y = 0; y < gridYOffsets.Length; y++)
            {
                for (int x = 0; x < gridXOffsets.Length; x++)
                {
                    foreach (Circle circle in circles)
                    {
                        if (Math.Abs(gridXOffsets[x] - circle.X) > consolidateDistanceX ||
                            Math.Abs(gridYOffsets[y] - circle.Y) > consolidateDistanceY)
                        {
                            continue;
                        }

                        shape[y * gridXOffsets.Length + x] = true; 
                    }
                }
            }

            float scoreIncrement = 1f / 9f;

            MatchTemplate bestMatch = null;
            confidence = 0; 

            foreach (MatchTemplate template in matchTemplates)
            {
                float score = 0; 

                for (int i = 0; i < 9; i++)
                {
                    float scoreFactor = 0;

                    bool templateHasHole = template.Holes[i];
                    bool matchHasHole = shape[i];

                    if (templateHasHole == true && matchHasHole == true)
                    {
                        scoreFactor = 1.2f;
                    }
                    else if (templateHasHole == false && matchHasHole == false)
                    {
                        scoreFactor = 1.2f;
                    }
                    else if (templateHasHole == false && matchHasHole == true)
                    {
                        scoreFactor = -1;
                    }
                    else if (templateHasHole == true && matchHasHole == false)
                    {
                        scoreFactor = -1;
                    }

                    //else if (shape[i] == true)
                    //{
                    //    scoreFactor = -1;
                    //}
                    //else
                    //{
                    //    scoreFactor = -1;
                    //}

                    score += scoreIncrement * scoreFactor; 
                }

                if (bestMatch == null || score > confidence)
                {
                    bestMatch = template;
                    confidence = score; 
                }
            }

            name = bestMatch.Name; 
            pageIndex = bestMatch.PageIndex;
            circleCount = circles.Count; 
        }
    }

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

                    float minSize = (23f / 352f) * width; 
                   // float maxSize = (58f / 352f) * width;
					float maxSize = (70f / 352f) * width;

                    blobCounter.FilterBlobs = true;
                    blobCounter.MinHeight = (int)minSize;
                    blobCounter.MinWidth = (int)minSize;
                    blobCounter.MaxWidth = (int)maxSize;
                    blobCounter.MaxHeight = (int)maxSize;
                    
                    blobCounter.ProcessImage(bitmapData);

                    Blob[] blobs = blobCounter.GetObjectsInformation();
                    
                    SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

                    for (int i = 0, n = blobs.Length; i < n; i++)
                    {
                        Rectangle bounds = blobs[i].Rectangle;

                        Circles.Add(new Circle()
                        {
                            X = (bounds.Width * 0.5f + bounds.X) / (float)width,
                            Y = 1f - ((bounds.Height * 0.5f + bounds.Y) / (float)height),
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

    ShapeMatcher imageScanner = new ShapeMatcher();
    Matcher matcher = new Matcher();
    PageSelector pageSelector; 

    WebCamTexture webCamTexture;
    byte[] colorBytes = new byte[0];
    int captureOctave = 0;

    int lastMatch = -1;
    int matchFrames = 0;
    float accumulativeConfidence = 0;
    int lowConfidenceFrames = 0;

    //byte[] tempColorBytes = new byte[0];

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
        pageSelector = GetComponent<PageSelector>(); 

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
                    //tempColorBytes = new byte[colorBytes.Length]; 
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
                    //tempColorBytes = new byte[colorBytes.Length];
                }

                Color32[] colorData = webCamTexture.GetPixels32();

                GetArrayBytes_Greyscale(colorData, webCamTexture.width * webCamTexture.height, colorBytes);

                //handle = GCHandle.Alloc(colorBytes, GCHandleType.Pinned);
            }

            //    AddCircles(ShapeMatcher matcher)
            //{
            //        Circles.AddRange(matcher.Circles);
            //    }

            //public void TryResolvePage(out string name, out int pageIndex, out float confidence)

            int pageIndex = -1;

            if (captureOctave == 0)
            {
                matcher.Circles.Clear();

                //Array.Copy(colorBytes, tempColorBytes, colorBytes.Length); 
                imageScanner.Match(0, CurrentTexture.width, CurrentTexture.height, colorBytes); // handle.AddrOfPinnedObject());
                matcher.AddCircles(imageScanner);
                //countMax = Mathf.Max(countMax, imageScanner.Circles.Count);
            }
            else if (captureOctave == 1)
            {
                //Array.Copy(colorBytes, tempColorBytes, colorBytes.Length);
                imageScanner.Match(50, CurrentTexture.width, CurrentTexture.height, colorBytes); // handle.AddrOfPinnedObject());
                //countMax = Mathf.Max(countMax, imageScanner.Circles.Count);
                matcher.AddCircles(imageScanner);
            }
            else if (captureOctave == 2)
            {
                //Array.Copy(colorBytes, tempColorBytes, colorBytes.Length);
                imageScanner.Match(100, CurrentTexture.width, CurrentTexture.height, colorBytes); // handle.AddrOfPinnedObject());
                //countMax = Mathf.Max(countMax, imageScanner.Circles.Count);
                matcher.AddCircles(imageScanner);
            }

            captureOctave = (captureOctave + 1) % 3;

            if (captureOctave == 0)
            {
                string name;
                float confidence;
                int circleCount;

                matcher.TryResolvePage(out name, out pageIndex, out confidence, out circleCount);

                if (DebugText != null)
                {
                    DebugText.text = pageIndex.ToString(); //  matcher.Circles.Count.ToString();
                }

                print("Image: " + CurrentTexture.width + "x" + CurrentTexture.height + ", Page name: " + name + ", Page index: " + pageIndex.ToString() + ", Circle Count: " + circleCount + ", Confidence: " + confidence);

                if (confidence < 0.5)
                {
                    lowConfidenceFrames++;
                }
                else
                {
                    lowConfidenceFrames = 0; 
                }

                if (lastMatch != pageIndex)
                {
                    lastMatch = pageIndex;
                    matchFrames = 0;
                    accumulativeConfidence = confidence;
                }
                else
                {
                    matchFrames++;
                    accumulativeConfidence += confidence;

                    if (matchFrames > 4 && accumulativeConfidence / matchFrames > 0.5)
                    {
                        pageSelector.SelectPage(pageIndex);
                    }
                }

                if (lowConfidenceFrames > 10)
                {
                    pageSelector.SelectPage(0);
                }
            }
        }
        catch (Exception ex)
        {
            if (DebugText != null)
            {
                DebugText.text = ex.Message;
            }

            Debug.LogException(ex); 
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

