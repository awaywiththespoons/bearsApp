﻿using UnityEngine;
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

                //if (colorBytes.Length != (TestImage.width * 4) * TestImage.height)
                //{
                //    colorBytes = new byte[(TestImage.width * 4) * TestImage.height];
                //}

                if (colorBytes.Length != TestImage.width * TestImage.height)
                {
                    colorBytes = new byte[TestImage.width * TestImage.height];
                }

                Color32[] colorData = TestImage.GetPixels32();

                GetArrayBytes_Greyscale(colorData, TestImage.width * TestImage.height, colorBytes);

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

                //if (colorBytes.Length != (webCamTexture.width * 4) * webCamTexture.height)
                //{
                //    colorBytes = new byte[(webCamTexture.width * 4) * webCamTexture.height];
                //}

                if (colorBytes.Length != webCamTexture.width * webCamTexture.height)
                {
                    colorBytes = new byte[webCamTexture.width * webCamTexture.height];
                }

                Color32[] colorData = webCamTexture.GetPixels32();

                GetArrayBytes_Greyscale(colorData, webCamTexture.width * webCamTexture.height, colorBytes);

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

    private static void GetArrayBytes_Greyscale(Color32[] array, int count, byte[] bytes)
    {
        for (int i = 0; i < array.Length; i++)
        {
            bytes[i] = Max(array[i]); 
        }
    }

    private static byte Max(Color32 color32)
    {
        return (byte)Mathf.Max(color32.r, color32.g, color32.b); 
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

