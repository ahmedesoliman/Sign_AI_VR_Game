using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class TrainData : MonoBehaviour
{
    Mat canny = new Mat();
    Mat threshHoldoutput = new Mat();
    OpenCvSharp.Point[][] featureImage;
    OpenCvSharp.HierarchyIndex[] trainHierarchy;

    int THRESH = 200;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Train(Mat frame)
    {
        frame = OpenCvSharp.Unity.TextureToMat(webcam1);

        OpenCvSharp.Rect myROI = new OpenCvSharp.Rect(200, 200, 200, 200);
        
        Mat cropFrame = frame[myROI];
        Cv2.ImShow("Crop Frame", cropFrame);
        Cv2.Canny(cropFrame, canny, 50, 200);
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


    } 
}
