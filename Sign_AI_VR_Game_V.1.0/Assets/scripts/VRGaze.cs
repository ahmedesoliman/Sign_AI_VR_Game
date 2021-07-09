using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRGaze : MonoBehaviour
{
    public Image imgGaze;

    public float totalTime = 2;
    bool gvrStatus;
    float gvrTimer;
    bool activated = false;
    
    public GameObject wall;
    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (gvrStatus)
        {
            gvrTimer += Time.deltaTime;
            imgGaze.fillAmount = gvrTimer/totalTime;


        }
        if (imgGaze.fillAmount == 1 && !activated)
        {
            wall.SetActive(false);
            GVROff();
        }
    }

    public void GVROn()
    {
        gvrStatus = true;  
    }

    public void GVROff()
    {
        gvrStatus = false;
        gvrTimer = 0; 
        imgGaze.fillAmount = 0;
    }
}
