using UnityEngine;
using System.Collections;
using OpenCVForUnity;
using System;
using System.Collections.Generic;

public class WebCamTest : MonoBehaviour {
    WebCamTexture webCamTexture;
    Mat erodeMat = new Mat();
    Texture2D texture = null; 

    [SerializeField]
    public Texture2D[] Sources;

    [SerializeField]
    public Texture CurrentTexture;

    [SerializeField]
    public bool UseWebCam = false;

    int[] face = {Core.FONT_HERSHEY_SIMPLEX, Core.FONT_HERSHEY_PLAIN, Core.FONT_HERSHEY_DUPLEX, Core.FONT_HERSHEY_COMPLEX,
            Core.FONT_HERSHEY_TRIPLEX, Core.FONT_HERSHEY_COMPLEX_SMALL, Core.FONT_HERSHEY_SCRIPT_SIMPLEX,
            Core.FONT_HERSHEY_SCRIPT_COMPLEX, Core.FONT_ITALIC};

    // Use this for initialization
    void Start () {
        webCamTexture = new WebCamTexture();

        //GetComponent<MeshRenderer>().material.mainTexture = texture;

        //texture.Play(); 

        //Texture2D imgTexture = Resources.Load("detect_blob") as Texture2D;
    }

    // Update is called once per frame
    void Update() {

        if (UseWebCam == true && webCamTexture.isPlaying == false)
        {
            webCamTexture.Play();
        }
        else if (UseWebCam == false && webCamTexture.isPlaying == true)
        {
            webCamTexture.Stop();
        }

        if (Input.GetButtonDown("Jump") == true)
        {
            TryProcessImage(UnityEngine.Random.Range(0, Sources.Length)); 
        }
	}

    private void TryProcessImage(int index)
    {
        if (UseWebCam == false)
        {
            CurrentTexture = Sources[index];
        }
        else
        {
            CurrentTexture = webCamTexture; 
        }

        using (Mat imgMat = new Mat(CurrentTexture.height, CurrentTexture.width, CvType.CV_8UC1))
        using (FeatureDetector blobDetector = FeatureDetector.create(FeatureDetector.SIMPLEBLOB))
        using (Mat outImgMat = new Mat())
        using (MatOfKeyPoint keypoints = new MatOfKeyPoint())
        {
            if (CurrentTexture is Texture2D)
            {
                Utils.texture2DToMat(CurrentTexture as Texture2D, imgMat);
            }
            else if (CurrentTexture is WebCamTexture)
            {
                Utils.webCamTextureToMat(CurrentTexture as WebCamTexture, imgMat);
            }
            else
            {
                Utils.textureToMat(CurrentTexture, imgMat);
            }

            Debug.Log("imgMat dst ToString " + imgMat.ToString());

            Imgproc.threshold(imgMat, imgMat, 0, 255, Imgproc.THRESH_BINARY | Imgproc.THRESH_OTSU);
            Imgproc.erode(imgMat, imgMat, erodeMat, new Point(1, 1), 5);            

            blobDetector.read(Utils.getFilePath("blobparams.yml"));

            blobDetector.detect(imgMat, keypoints);            

            Features2d.drawKeypoints(imgMat, keypoints, outImgMat);

            KeyPoint[] points = keypoints.toArray();

            ProcessKeyPoints(points, outImgMat);

            Mat finalMat = outImgMat; 

            if (texture != null &&
                (texture.width != finalMat.cols() || texture.height != finalMat.rows()))
            {
                DestroyImmediate(texture);

                texture = null;
            }

            if (texture == null)
            {
                texture = new Texture2D(finalMat.cols(), finalMat.rows(), TextureFormat.RGBA32, false);
            }

            Utils.matToTexture2D(finalMat, texture);

            gameObject.GetComponent<Renderer>().material.mainTexture = texture;
        }
    }

    class BlobSpot : IComparable<BlobSpot> 
    {
        public Vector2 Position; 
        public int Tag;

        public int CompareTo(BlobSpot other)
        {
            return ((int)(Position.y * 10000f) + (int)(Position.x * 1000f)) - ((int)(other.Position.y * 10000f) + (int)(other.Position.x * 1000f));
        }
    }

    private void ProcessKeyPoints(KeyPoint[] points, Mat outImgMat)
    {
        List<BlobSpot> initialSpots = new List<BlobSpot>();
        List<int> tags = new List<int>();

        int tagIndex = 0; 
        foreach (KeyPoint point in points)
        {
            tags.Add(tagIndex); 
            initialSpots.Add(new BlobSpot() { Position = new Vector2((float)point.pt.x / (float)outImgMat.width(), (float)point.pt.y / (float)outImgMat.height()), Tag = tagIndex++ });
        }

        float distanceThreshold = 0.125f; 

        foreach (BlobSpot spot in initialSpots)
        {
            foreach (BlobSpot other in initialSpots)
            {
                if ((other.Position - spot.Position).magnitude < distanceThreshold)
                {
                    int newTag = Math.Min(spot.Tag, other.Tag);
                    int deadTag = Math.Max(spot.Tag, other.Tag);

                    spot.Tag = newTag;
                    other.Tag = newTag;

                    if (newTag != deadTag)
                    {
                        tags.Add(deadTag);
                    }
                }
            }
        }

        List<BlobSpot> resolvedSpots = new List<BlobSpot>();
        List<BlobSpot> scratchSpots = new List<BlobSpot>(); 

        foreach (int tag in tags)
        {
            scratchSpots.Clear(); 

            foreach (BlobSpot spot in initialSpots)
            {
                if (spot.Tag == tag)
                {
                    scratchSpots.Add(spot); 
                }
            }

            resolvedSpots.Add(MergeSpots(tag, scratchSpots));
        }

        foreach (BlobSpot spot in resolvedSpots)
        {
            Imgproc.rectangle(outImgMat, GetPoint(outImgMat, spot, -20, -20), GetPoint(outImgMat, spot, 20, 20), new Scalar(0, 255, 255));
        }

        MatchShape(outImgMat, resolvedSpots); 

        //
    }

    private void MatchShape(Mat img, List<BlobSpot> spots)
    {
        spots.Sort();

        Imgproc.putText(img, "OK?", new Point(50, 50), face[0], 1.2, new Scalar(255, 255, 0), 2, Imgproc.LINE_AA, false);

        Point lastPoint = GetPoint(img, spots[0], 0, 0);

        for (int i = 1; i < spots.Count; i++)
        {
            BlobSpot spot = spots[i];

            Point nextPoint = GetPoint(img, spot, 0, 0);

            Imgproc.line(img, lastPoint, nextPoint, new Scalar(0, 255, 255), 3);

            lastPoint = nextPoint; 
        }
    }

    private Point GetPoint(Mat img, BlobSpot spot, int offsetX, int offsetY)
    {
        return new Point(
            spot.Position.x * (float)img.width() + offsetX,
            spot.Position.y * (float)img.height() + offsetY
            ); 
    }

    private BlobSpot MergeSpots(int tag, List<BlobSpot> spots)
    {
        Vector2 center = Vector2.zero;

        foreach (BlobSpot spot in spots)
        {
            center += spot.Position; 
        }

        center /= (float)spots.Count;

        return new BlobSpot() { Position = center, Tag = tag }; 
    }
}
