﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using System;
using System.IO;
using System.Windows;
using System.Numerics;
using System.Drawing;


public class ComputerCamera : MonoBehaviour
{
    static WebCamTexture webcam1;
    public UnityEngine.Object[] buffer;
    static Texture2D webcam2;
    public GameObject cam1;
    public GameObject cam2;


    Mat frame;
    Mat threshold_output = new Mat();
    Mat fgMaskMOG2 = new Mat();

    char asl_letter;
    int DIFF_THRESH = 230;
    double THRESH = 200;
    int maxIndex = 0;
    int MAX_LETTERS = 26;
    int frames = 0;   // frames varaible to count how many frames processed
    int SAMPLE_RATE = 1;

    OpenCvSharp.Point[][] letters;
    OpenCvSharp.Point[][] feature_image;
    OpenCvSharp.Point[][] contours;
    
    OpenCvSharp.HierarchyIndex[] hierarchy;

    BackgroundSubtractorMOG2 backgroundMOG2;
    BackgroundSubtractorKNN backgroundKNN;
    BackgroundSubtractorMOG backgroundMOG;
    BackgroundSubtractorGMG backgroundGMG;

    Ptr<BackgroundSubtractorMOG2> ptrBackgroundMOG2;

    Ptr<BackgroundSubtractor> backgroundSubtractor;



    // Start is called before the first frame update
    void Start()
    {

            WebCamDevice[] devices = WebCamTexture.devices;
            if (webcam1 == null)
                webcam1 = new WebCamTexture(devices[0].name);
            if (!webcam1.isPlaying)
                webcam1.Play();
            webcam1.requestedFPS = 25;
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log(devices[i].name);
        }
 
    }

    // Update is called once per frame
    void Update()
    {
/*        load_ASL();*/
        GetComponent<Renderer>().material.mainTexture = webcam1;
        frame = OpenCvSharp.Unity.TextureToMat(webcam1);
        predict(frame);

    }

    void load_ASL()
    {

        buffer = Resources.LoadAll("Train", typeof(Texture2D));
        int i = 0;
        Debug.Log(buffer.Length);
        foreach (var image in buffer)
        {
            
            Mat img1 = new Mat();

            img1 = OpenCvSharp.Unity.TextureToMat((Texture2D)buffer[i]);

            Cv2.ImShow("Img1", img1);

            Mat img2 = new Mat(), threshold_output = new Mat();

            Cv2.CvtColor(img1, img2, ColorConversionCodes.RGB2GRAY);

            // Detect edges using Threshold
            //The threshold method returns two outputs. The first is the threshold that was used and the second output is the thresholded image.
            Cv2.Threshold(img2, threshold_output, THRESH, 255, ThresholdTypes.Binary);

            //findcontours() function retrieves contours from the binary image using the openCV algorithm[193].
            //The contours are a useful tool for shape analysisand object and detectionand recognition.
            Cv2.FindContours(threshold_output, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, new Point(0, 0));

            letters[i] = contours[0];
            //contours returns a vector<vector<point>>
            i++;
            Debug.Log(i);
        }

/*        var backGroundMOG2 = BackgroundSubtractorMOG2.Create(2000, 20, true);*/
    }

    void predict(Mat frame)
    {
        Cv2.ImShow("Frame ", frame);
        cam1.GetComponent<Renderer>().material.mainTexture = webcam1;

        frame = OpenCvSharp.Unity.TextureToMat(webcam1);

        //Creates MOG2 Background Subtractor.
        backgroundMOG2 = BackgroundSubtractorMOG2.Create();

        // Crop Frame to smaller region using the rectangle of interest method

        OpenCvSharp.Rect myROI = new OpenCvSharp.Rect(200, 200, 200, 200);

        Mat cropFrame = frame[myROI];

        Cv2.ImShow("Crop Frame", cropFrame);

        Mat canny = new Mat();
        Cv2.Canny(cropFrame, canny, 50, 200);
        Cv2.ImShow("Canny", canny);

        // Update the background model
        backgroundMOG2.Apply(cropFrame, fgMaskMOG2, 1);

        webcam2 = OpenCvSharp.Unity.MatToTexture(fgMaskMOG2);

        Cv2.ImShow("Foregound Mask", fgMaskMOG2);

        cam2.GetComponent<Renderer>().material.mainTexture = webcam2;

        // Detect edges using Threshold

        Cv2.Threshold(canny, threshold_output, THRESH, 255, ThresholdTypes.Binary);

        // Find contours
        Cv2.FindContours(threshold_output, out feature_image, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, new Point(0, 0));

        double largest_area = 0;

        for (int j = 0; j < feature_image.Length; j++)
        {
            double area = Cv2.ContourArea(feature_image[j], false); // Find the area of the contour
            if (area > largest_area)
            {
                largest_area = area;
                maxIndex = j; // Store the index of largest contour
            }
        }

        // creates Mat of Zeros = Black frame to draw on 
        Mat contourImg = Mat.Zeros(cropFrame.Size(), MatType.CV_8UC(3));
        // Draw Contours         
        Cv2.DrawContours(contourImg, feature_image, maxIndex, new Scalar(0, 0, 255), 2, LineTypes.Link8, hierarchy, 0, new Point(0, 0));

        Cv2.ImShow("Countour Img", contourImg);


        //Reset if too much noise
        /*    Scalar sums = sum(drawing1);
                s = sums[0] + sums[1] + sums[2] + sums[3];
              if (s >= RESET_THRESH){
              backGroundMOG2 = createBackgroundSubtractorMOG2(10000, 200, false);
               continue;
            }*/


        //if (contourImg.rows > 0)

        //imshow("Contour", contourImg);

        //key = waitKey(1);

        // Manual reset the keyboard
        //if (key == ' ')
        //backGroundMOG2 = createBackgroundSubtractorMOG2(10000, 200, false);

        if (feature_image[0].Length > 0 && frames++ > SAMPLE_RATE && feature_image[maxIndex].Length >= 5)
        {
            {
                RotatedRect testRect = Cv2.FitEllipse(feature_image[maxIndex]);
                //fits an ellipse around a set of 2d points. The function calculates the ellipse that fits(in a least-sense) a set of 2D points best of all.
                //it returns the rotated rectangle in which the ellipse is inscribed. the first algorithm - Param(points input 2d point set, stored in std::vector<> or Mat)

                frames = 0;

                double lowestDiff = double.MaxValue;

                for (int i = 0; i < MAX_LETTERS; i++)
                {
                    if (letters[i].Length == 0)
                        continue;

                    double difference = distance(letters[i], feature_image[maxIndex]);

                    if (difference < lowestDiff)
                    {
                        lowestDiff = difference;
                        asl_letter = (char)(((int)'a') + i);
                        
                    }
                }

                if (lowestDiff > DIFF_THRESH)
                { // Dust
                    asl_letter = (char)(((int)0));
                }


               // ofstream myfile;
               //if (isalpha(asl_letter)){
               //  myfile.open("C:\\Unity_Projects\\Sign_AI_VR_Game\\Sign_AI_VR_Game_V.1.0\\Assets\\StreamingAssets\\RecallText\\Alphabets.txt", ios::out | ios::app);
               //myfile << asl_letter;
               //myfile.close();

                Debug.Log("The letter is: " + asl_letter + " | difference: " + lowestDiff);
                
            }
        }
    }
    double distance(Point[] a, Point[] b){

                int maxDistAB = distance_2(a, b);

                int maxDistBA = distance_2(b, a);

                int maxDist = Math.Max(maxDistAB, maxDistBA);

                return Math.Sqrt((double)maxDist);
    }
    int distance_2(Point[] a, Point[] b)
            {
                int maxDist = 0;
                for (int i = 0; i < a.Length; i++)
                {
                    int min = 1000000;
                    for (int j = 0; j < b.Length; j++)
                    {
                        int dx = (a[i].X - b[j].X);

                        int dy = (a[i].Y - b[j].Y);

                        int tmpDist = dx * dx + dy * dy;

                        if (tmpDist < min)
                        {
                            min = tmpDist;
                        }

                        if (tmpDist == 0)
                        {
                            break; // can't get better than equal.
                        }
                    }
                    maxDist += min;
                }
                return maxDist;
            } /* end of distance_2()*/

~ComputerCamera() {

        frame.Dispose();
        frame.Release();
    }

}

       