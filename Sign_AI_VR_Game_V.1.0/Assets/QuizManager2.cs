using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizManager2 : MonoBehaviour
{
    // Question Title
    [SerializeField] public static TMP_Text textMeshPro;
    [SerializeField] private int timeInSeconds;

    private Question q1;
    private GameStatus.Status currentStatus;
    private GameStatus.Level currentLevel;

    private void Awake()
    {
        //if(changingText == null)
        //{
        //    Debug.Log("Question Title not enabled");
        //    this.enabled = false;
        //}
    }
    void Start()
    {
        textMeshPro = GetComponentInChildren<TextMeshPro>();
        q1 = new Question("What is the letter");
        StartCoroutine(randomizeQuestion());
    }

    // Update is called once per frame
    void Update()
    {


    }

    IEnumerator randomizeQuestion()
    {
        while (currentLevel == GameStatus.Level.LEVEL3)
        {
            if (currentStatus == GameStatus.Status.PLAYING)
            {
                q1.changeQuestion(Random.Range(97, 122));
                textMeshPro.text = q1.getQuestion();

                currentStatus = GameStatus.Status.NEXT;

              

            }
            else if(currentStatus == GameStatus.Status.NEXT)
            {
                yield return new WaitForSeconds(timeInSeconds);
            }
            else
            {
                continue;
            }
        }
    }
    //IEnumerator RandomPicture()
    //{
    //    while (true)
    //    {
    //        setRandom();
    //        yield return new WaitForSeconds(3);
    //    }
    //}
    //void setRandom()
    //{
    //    Texture2D texture = (Texture2D)myTexture[Random.Range(0, myTexture.Length)];
    //    rawImage.GetComponent<Renderer>().material.mainTexture = texture;
    //}

}
