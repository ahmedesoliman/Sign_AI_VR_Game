using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class randomImageSelection : MonoBehaviour
{
    // Start is called before the first frame update
    private UnityEngine.Object[] myTexture; // all the images
    private GameObject rawImage; // boxImage


    public UnityEngine.Object[] filebuffer;

    bool gameStatus = false;

    void Start()
    {
        myTexture = Resources.LoadAll("Test", typeof(Texture2D));
        rawImage = GameObject.Find("ImageBox");
        wiatforLetter();

    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void wiatforLetter()
    {
        var dir = new DirectoryInfo(@"./Assets/Resources/Alpha");
        var info = dir.GetFiles("*.png*");

        char predictChar = Predict_Script.getLetter();
        
            foreach (var file in info)
            {
                string fileName = Path.GetFileName(file.ToString());

                string extracted = fileName.Substring(0, 1);

                char fileChar = Convert.ToChar(extracted);

                Debug.Log("---------------:" + fileChar);

                if (predictChar == fileChar)
                {
                    Debug.Log("Letter has been found in the DIR");
                }
            }
            StartCoroutine(RandomPicture());
    }


    IEnumerator RandomPicture()
    {
            setRandom();
            yield return new WaitForSeconds(3);
        
    }
    void setRandom()
    {
        Texture2D texture = (Texture2D)myTexture[UnityEngine.Random.Range(0, myTexture.Length)];
        rawImage.GetComponent<Renderer>().material.mainTexture = texture;
    }

}
