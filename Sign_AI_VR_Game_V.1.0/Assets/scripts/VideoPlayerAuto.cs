using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerAuto : MonoBehaviour
{
    private VideoPlayer video;
    public GameObject videoPlayerObject;
    // Start is called before the first frame update
    void Start()
    {
        video = gameObject.GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Runtime.isPreLevel2)
        Invoke("disablePlayer",5);
    }

    void OnTriggerEnter(Collider other)
    {
        Runtime.isPreLevel2 = true;
    }
    void disablePlayer()
    {
        if (!(video.isPlaying) && Runtime.isPreLevel2)
        {
            videoPlayerObject.SetActive(false);
        }
    }
}





