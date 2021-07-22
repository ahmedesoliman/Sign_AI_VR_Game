using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using System.IO;

public class Train_Script : MonoBehaviour
{
    Mat frame = new Mat();
    Mat canny = new Mat();
    Mat threshHoldoutput = new Mat();
    OpenCvSharp.Point[][] featureImage;
    OpenCvSharp.HierarchyIndex[] trainHierarchy;

    int THRESH = 200;
    int maxIndex = 0;

    static Texture2D  image;
    static Texture2D tex1;
    static Texture2D tex2;


    public GameObject Train_display1;
    public GameObject Train_display2;

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
      /*  GetComponent<Renderer>().material.mainTexture = webcam1;*/
        frame = OpenCvSharp.Unity.TextureToMat(webcam1);
        Train(frame);

    }
    void Train(Mat frame)
    {
/*      frame = OpenCvSharp.Unity.TextureToMat(webcam1);*/

        OpenCvSharp.Rect myROI = new OpenCvSharp.Rect(200, 200, 200, 200);
        
        Mat cropFrame = frame[myROI];

        tex1 = OpenCvSharp.Unity.MatToTexture(cropFrame);

        Train_display1.GetComponent<Renderer>().material.mainTexture = tex1;

        Cv2.ImShow("Train Crop Frame", cropFrame);

        Cv2.Canny(cropFrame, canny, 50, 200);

        Cv2.ImShow("Train canny", canny);
        tex2 = OpenCvSharp.Unity.MatToTexture(canny);

        Train_display2.GetComponent<Renderer>().material.mainTexture = tex2;

        Cv2.Threshold(canny, threshHoldoutput, THRESH, 255, ThresholdTypes.Binary);
        
        Cv2.FindContours(threshHoldoutput, out featureImage, out trainHierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, new Point(0, 0));

        double largest_area = 0;

        for (int j = 0; j < featureImage.Length; j++)
        {
            double area = Cv2.ContourArea(featureImage[j], false); // Find the area of the contour
            if (area > largest_area)
            {
                largest_area = area;
                maxIndex = j; // Store the index of largest contour
            }
        }

        // creates Mat of Zeros = Black frame to draw on 
        Mat contourImg = Mat.Zeros(cropFrame.Size(), MatType.CV_8UC(3));
        // Draw Contours         
        Cv2.DrawContours(contourImg, featureImage, maxIndex, new Scalar(0, 0, 255), 2, LineTypes.Link8, trainHierarchy, 0, new Point(0, 0));

        /*        Cv2.ImShow("Train Img", contourImg);*/

        if (Input.anyKey && !(Input.GetMouseButton(0) || Input.GetMouseButton(1)))
        {
            int minimumKey = (int)KeyCode.A;
            int maximumKey = (int)KeyCode.Z;
            int i = minimumKey;

            var character = ' ';

            for (int j = i; j <= maximumKey; j++)
            {
                if (Input.GetKeyDown((KeyCode)j))
                {

                    character = (char)(KeyCode)j;
                }
            }

            if (character >= 'A' || character <= 'Z')
            {
                Debug.Log("Char is: -----------> " + character);

                image = OpenCvSharp.Unity.MatToTexture(canny);

                byte[] Bytes = image.EncodeToPNG();

                File.WriteAllBytes(Application.dataPath + "/Resources/Train/" + character + ".png", Bytes);
            }
        }
    }
    ~Train_Script()
    {
/*        frame.Dispose();
        frame.Release();*/
        Cv2.DestroyAllWindows();
    }
}
