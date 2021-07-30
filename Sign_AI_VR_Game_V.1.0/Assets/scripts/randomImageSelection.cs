using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;

public class randomImageSelection : MonoBehaviour
{
    // Start is called before the first frame update
    private UnityEngine.Object[] myTexture; // all the images
    private GameObject rawImage; // boxImage


    public UnityEngine.Object[] filebuffer;
    private QuestionTitle question = new QuestionTitle("Show the letter ");
    [SerializeField] private TMP_Text questionTitle;

    char correctChar;

    void Start()
    {
        myTexture = Resources.LoadAll("Alpha", typeof(Texture2D));
        rawImage = GameObject.Find("ImageBox");
        correctChar = Random();
        question.changeQuestion((int)correctChar);
        questionTitle.text = question.getQuestion();
        Debug.Log("Question: " + question.getQuestion());
    }

    // Update is called once per frame
    void Update()
    {
        waitForLetter();
    }


    void waitForLetter()
    {

        if (correctChar == Predict_Script.getLetter())
        {
            Runtime.point++;
            correctChar = Random();
            question.changeQuestion((int)correctChar);
            questionTitle.text = question.getQuestion();

/*            Debug.Log(question.getQuestion());*/
        }
        
    }
        

    public char Random()
    {
        Texture2D texture = (Texture2D)myTexture[UnityEngine.Random.Range(0, myTexture.Length)];
        rawImage.GetComponent<Renderer>().material.mainTexture = texture;

        return Convert.ToChar(texture.ToString().Substring(0, 1));
    }

}
