using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script deals with camera and returns frames to other scripts to be used.
public class CameraScript : MonoBehaviour
{
 
    private static WebCamDevice[] devices; // List of cameras devices connected to the Unity
    private static WebCamTexture webcam;  // Texture for CAMERA
    private static int FPS = 30;
    private static bool CameraSetup = false;
    [SerializeField] private int CameraNumber = 0; // Please set the correct camera index in the UNITY EDITOR.
                                                   // To locate where the script is located = RightClick -> Find References In Scene
                                                   // For identify correct camera call printCameraList()

    // Start is called before the first frame update
    void Start()
    {
        setupCamera(CameraNumber);
        printCameraList();

    }
    private static void setupCamera(int numberCamera = 0)
    {
        // Check camera number
        if (numberCamera < 0)
        {
            throw new UnityException("Invalid Camera Option");
        }

       devices = WebCamTexture.devices;
        if (webcam == null)
            webcam = new WebCamTexture(devices[numberCamera].name);
        if (!webcam.isPlaying)
            webcam.Play();
        webcam.requestedFPS = FPS;
        CameraSetup = true;
    }


    // First time run method for check if the camera is properly setup to be used on other scripts
    private static void checkCameraSetup()
    {
        if (!(CameraSetup))
        {
            throw new UnityException("Camera not setup correctly");
        }
    }
    // ************************************ PUBLIC METHODS ************************************

    /// <summary>
    // Returns the WebCamTexture;
    /// </summary>
    public static WebCamTexture getWebCamTexture()
    {
        checkCameraSetup();
        return webcam;
    }
    /// <summary>
    // Prints a list of camera available;
    /// </summary>
    public static void printCameraList()
    {
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log(string.Format("[{0}] Camera Index: {1} -> Camera Name: {2}",arg0: "CameraScript.cs" ,arg1: i, arg2: devices[i].name));
        }
    }
}
