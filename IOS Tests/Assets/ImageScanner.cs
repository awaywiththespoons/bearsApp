using UnityEngine;
using System.Collections;

public class ImageScanner : MonoBehaviour
{
    WebCamTexture webCamTexture;

    [SerializeField]
    public bool UseWebCam = true;

    [SerializeField]
    public Texture2D TestImage;

    [SerializeField]
    public MeshRenderer CameraTest;

    [SerializeField]
    public Texture CurrentTexture;

    // Use this for initialization
    void Start()
    {
        webCamTexture = new WebCamTexture();

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
        if (UseWebCam == false)
        {
            CurrentTexture = TestImage;
        }
        else
        {
            CurrentTexture = webCamTexture;
        }


        if (CameraTest != null)
        {
            CameraTest.material.mainTexture = CurrentTexture;
        }
    }
}
