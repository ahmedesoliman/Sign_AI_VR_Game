using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using System;
using System.Windows;
using System.Numerics;
using System.IO;


public class ComputerCamera : MonoBehaviour
{
    static WebCamTexture webcam1;
    static Texture2D webcam2;
    public GameObject cam1;
    public GameObject cam2;

    CascadeClassifier cascade;

    Mat frame;
    Mat threshold_output = new Mat();
    Mat fgMaskMOG2 = new Mat();

    double THRESH = 200;
    int maxIndex = 0;
    int MAX_LETTERS = 26;

    OpenCvSharp.Point[][] letters;
    OpenCvSharp.Point[][] feature_image;
    OpenCvSharp.Point[][] contours;
    
    OpenCvSharp.HierarchyIndex[] hierarchy;

    BackgroundSubtractorMOG2 backGroundMOG2;
    BackgroundSubtractorKNN backgroundKNN;
    BackgroundSubtractorMOG backgroundMOG;
    BackgroundSubtractorGMG backgroundGMG;

    Ptr<BackgroundSubtractor> backgroundSubtractor;

    


    // Start is called before the first frame update
    void Start()
    {
        //while (true)
        //{
            WebCamDevice[] devices = WebCamTexture.devices;
            if (webcam1 == null)
                webcam1 = new WebCamTexture(devices[1].name);


            /*        cascade = new CascadeClassifier(Application.dataPath + @"haarcascade_frontalface_defualt.xml");
            */

            if (!webcam1.isPlaying)
                webcam1.Play();
            webcam1.requestedFPS = 30;

            predict(frame);
        //}
    }

    // Update is called once per frame
    void Update()
    {

        GetComponent<Renderer>().material.mainTexture = webcam1;
        frame = OpenCvSharp.Unity.TextureToMat(webcam1);
/*
        load_ASL();*/

    }

    void load_ASL()
    {
        //*** Preload letter train images starts ***//
        int ascii = 97;

        for (int i = 0; i < MAX_LETTERS; i++)
        {
            String format = @"C:\\Unity_Projects\\Sign_AI_VR_Game_V.1.0\\Assets\\Resources\\Train\\{0}.png";
            
            string filename = "";

            Debug.Log(string.Format(format, Convert.ToChar(ascii + i)));
            Debug.Log("This is : " + i);

            filename = Path.GetFileName(string.Format(format, Convert.ToChar(ascii + i)));

            Debug.Log(filename);

            Mat img1 = new Mat();
             img1 = Cv2.ImRead(@"C:\\Unity_Projects\\Sign_AI_VR_Game_V.1.0\\Assets\\Resources\\Train\\vCopy.jpg", ImreadModes.Color);
/*
            Cv2.ImShow("img1", img1);*/

            if (img1.Data != null)
            {
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
            }
        }
        //***Preload letter train images ends***//

        //*** learn starts ***//

        backGroundMOG2 = BackgroundSubtractorMOG2.Create(10000, 200, false);

        //***learn ends  ***//

    } /* end of asl_init()*/
    void predict(Mat frame)
    {
            /*    Cv2.ImShow("Frame ", frame);*/
            cam1.GetComponent<Renderer>().material.mainTexture = webcam1;
            frame = OpenCvSharp.Unity.TextureToMat(webcam1);

            //Creates MOG2 Background Subtractor.
            backGroundMOG2 = BackgroundSubtractorMOG2.Create(10000, 200, false);

            // Crop Frame to smaller region using the rectangle of interest method

            OpenCvSharp.Rect myROI = new OpenCvSharp.Rect(200, 200, 200, 200);

            Mat cropFrame = frame[myROI];

            Cv2.ImShow("Crop Frame", cropFrame);

            // Update the background model
            backGroundMOG2.Apply(cropFrame, fgMaskMOG2, 1);

            webcam2 = OpenCvSharp.Unity.MatToTexture(fgMaskMOG2);

            Cv2.ImShow("Foregound Mask", fgMaskMOG2);

            cam2.GetComponent<Renderer>().material.mainTexture = webcam2;

            // Detect edges using Threshold

            Cv2.Threshold(fgMaskMOG2, threshold_output, THRESH, 255, ThresholdTypes.Binary);

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
            //    Scalar sums = sum(drawing1);
            //int s = sums[0] + sums[1] + sums[2] + sums[3];
            //if (s >= RESET_THRESH)
            //{
            //    backGroundMOG2 = createBackgroundSubtractorMOG2(10000, 200, false);
            //    continue;
            //}


            //if (contourImg.rows > 0)

            //imshow("Contour", contourImg);

            //key = waitKey(1);

            // Manual reset the keyboard
            //if (key == ' ')
            //backGroundMOG2 = createBackgroundSubtractorMOG2(10000, 200, false);

            /*       //f3_identify_letter
                   if (feature_image.size() > 0 && frames++ > SAMPLE_RATE && feature_image[maxIndex].size() >= 5) { {
                           RotatedRect testRect = fitEllipse(feature_image[maxIndex]);
                           //fits an ellipse around a set of 2d points. The function calculates the ellipse that fits(in a least-sense) a set of 2D points best of all.
                           //it returns the rotated rectangle in which the ellipse is inscribed. the first algorithm - Param(points input 2d point set, stored in std::vector<> or Mat)

                           frames = 0;

                           double lowestDiff = HUGE_VAL;

                           for (int i = 0; i < MAX_LETTERS; i++)
                           {
                               if (letters[i].size() == 0)
                                   continue;

                               double difference = distance(letters[i], feature_image[maxIndex]);

                               if (difference < lowestDiff)
                               {
                                   lowestDiff = difference;
                                   asl_letter = 'a' + i;
                               }
                           }

                           if (lowestDiff > DIFF_THRESH)
                           { // Dust
                               asl_letter = 0;
                           }

                           //ofstream myfile;
                           //myfile.open("output.txt", ios::out | ios::app);
                           //myfile << asl_letter;
                           //cout << "The letter is: " << asl_letter << " | difference: " << lowestDiff << endl;
                           //cout << "Writing the letter: " << asl_letter << " -> to a file.\n";
                           // myfile.close();
                           //displayLetter();
                       }*/

    }

    ~ComputerCamera() {

        frame.Dispose();
        frame.Release();
    }
}

       