using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomImageSelection : MonoBehaviour
{
    // Start is called before the first frame update
    private Object[] myTexture; // all the images
    private GameObject rawImage; // boxImage

    void Start()
    {

        myTexture = Resources.LoadAll("Alpha", typeof(Texture2D));
        rawImage = GameObject.Find("ImageBox");
        StartCoroutine(RandomPicture());

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator RandomPicture()
    {
        while (true)
        {
            setRandom();
            yield return new WaitForSeconds(3);
        }
    }
    void setRandom()
    {
        Texture2D texture = (Texture2D)myTexture[Random.Range(0, myTexture.Length)];
        rawImage.GetComponent<Renderer>().material.mainTexture = texture;
    }

}
