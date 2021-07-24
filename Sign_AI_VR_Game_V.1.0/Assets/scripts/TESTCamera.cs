using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class TESTCamera : MonoBehaviour
{
    private WebCamTexture webcam;
    BackgroundSubtractorMOG2 fgbg = BackgroundSubtractorMOG2.Create(500, 16, true);

    // Start is called before the first frame update
    void Start()
    {
        webcam = CameraScript.getWebCamTexture();
    }
    private void Update()
    {
        if (webcam == null)
        {
            Debug.Log("Empty");
        }
        else
        {
            Mat camMatrix = OpenCvSharp.Unity.TextureToMat(webcam);
            predict(camMatrix);
        }
    }
    private void predict(Mat frame)
    {
        Cv2.ImShow("Camera", frame);

        Mat afterMOG2 = new Mat();
        fgbg.Apply(frame, afterMOG2);

        Cv2.ImShow("MOG2", afterMOG2);
    }


}
