using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using System.IO;

public class TrainData : MonoBehaviour
{
    Mat frame = new Mat();
    Mat canny = new Mat();
    Mat threshHoldoutput = new Mat();
    OpenCvSharp.Point[][] featureImage;
    OpenCvSharp.HierarchyIndex[] trainHierarchy;

    int THRESH = 200;

    static Texture2D  image;

    static WebCamTexture webcam1;

    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (webcam1 == null)
            webcam1 = new WebCamTexture(devices[0].name);
        if (!webcam1.isPlaying)
            webcam1.Play();
        webcam1.requestedFPS = 30;
    }

    // Update is called once per frame
    void Update()
    {
    /*    GetComponent<Renderer>().material.mainTexture = webcam1;*/
        frame = OpenCvSharp.Unity.TextureToMat(webcam1);
        Train(frame);

    }
    void Train(Mat frame)
    {
/*        frame = OpenCvSharp.Unity.TextureToMat(webcam1);*/

        OpenCvSharp.Rect myROI = new OpenCvSharp.Rect(200, 200, 200, 200);
        
        Mat cropFrame = frame[myROI];
        Cv2.ImShow("Crop Frame", cropFrame);

        Cv2.Canny(cropFrame, canny, 50, 200);
        Cv2.ImShow("canny", canny);
        Cv2.Threshold(canny, threshHoldoutput, THRESH, 255, ThresholdTypes.Binary);
        
        Cv2.FindContours(threshHoldoutput, out featureImage, out trainHierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, new Point(0, 0));

        double largest_area = 0;

        for (int j = 0; j < featureImage.Length; j++)
        {
            double area = Cv2.ContourArea(featureImage[j], false); // Find the area of the contour
            if (area > largest_area)
            {
                largest_area = area;
                ComputerCamera.maxIndex = j; // Store the index of largest contour
            }
        }

        // creates Mat of Zeros = Black frame to draw on 
        Mat contourImg = Mat.Zeros(cropFrame.Size(), MatType.CV_8UC(3));
        // Draw Contours         
        Cv2.DrawContours(contourImg, featureImage, ComputerCamera.maxIndex, new Scalar(0, 0, 255), 2, LineTypes.Link8, trainHierarchy, 0, new Point(0, 0));

        Cv2.ImShow("Countour Img", contourImg);
        
        string key = Input.inputString;

        char character = key.ToCharArray()[0];
        
        if (character >= 'a' || character <= 'z') {

            int keyIndex = 0 + character - 'a';
  
            image = OpenCvSharp.Unity.MatToTexture(canny);

            byte[] Bytes = image.EncodeToPNG();
           
            File.WriteAllBytes(Application.dataPath + "/Resources/Train/" + keyIndex + ".png", Bytes);
        }
    }
    ~TrainData()
    {

        frame.Dispose();
        frame.Release();
        Cv2.DestroyAllWindows();
    }
}
