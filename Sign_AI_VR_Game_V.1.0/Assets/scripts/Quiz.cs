﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class Quiz : MonoBehaviour
{
    // Start is called before the first frame update
    private List<Quiz_Question.Question<string>> listOfQuestion; // Question type includes answers
    private int numbersOfCorrectAnswers = 0;       // Numbers of correct answer which user got
    private UnityEngine.Object[] buffer;    // Holds all 2d textures of ASL images
    private int numberOfQuestions;
    // Folder settings
    [SerializeField] private string folderName = "Alpha";

    // Objects which user will step in
    [SerializeField] private int numberOfObjects = 4;  // By default is 4
    [SerializeField] private GameObject[] platforms = new GameObject[4];

    [SerializeField] private TMP_Text questionTitle;
    [SerializeField] private GameObject correctPlatform;

    private GameObject player;
    


    void Awake()
    {
        buffer = Resources.LoadAll(folderName, typeof(Texture2D));
        player = GameObject.FindGameObjectWithTag("Player");
        numberOfQuestions = buffer.Length;
        setTheObjects();

        Debug.Log("[QUIZ 3] Loaded components successfully");


    }
    void Start()
    {
    }
    private void Update()
    {
    }

    private void setTheObjects()
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, buffer.Length);
            platforms[i].transform.Find("Image").gameObject.GetComponent<Renderer>().material.mainTexture = (Texture2D)buffer[randomIndex];
            platforms[i].name = buffer[randomIndex].ToString().Substring(0,1); // Convert the object to respectively char letter
            
            
        }
    }
    private void endGame()
    {

    }
}

namespace Quiz_Question
{
    /*Each question by default will have:
        1. 1(One) Question text
        2. 2(two) answers. Can add more by calling addAnswer(<T> answer, bool isCorrectAnswer)*
        ! Instantiating question object: Question object requires specific data type which will be used on answers

    Method #1:  
        addAnswer(<T> answer bool isCorrectAnswer) - add additional answers to the question.
        Parameter: set the answer as correct answer (OPTIONAL) -> BY DEFAULT is INCORRECT
    */
    public class Question<T>: ScriptableObject
    {
        private string questionText;

        // Dictionary acts as Map
        // Arg1 = answer
        // Arg2 = correct Answer
        private Dictionary<T, bool> answers = new Dictionary<T, bool>(); 
    
        public Question(string questionTitle, T answer1, bool correct1, T answer2, bool correct2)
        {
            questionText = questionTitle; 
            answers.Add(answer1, correct1);
            answers.Add(answer2, correct2);
        }


        public void addAnswer(T answer, bool isCorrectAnswer = false)
        {
            answers.Add(answer, isCorrectAnswer);
        }

        // Returns list of answers 
        public List<T> getAnswers()
        {
            return answers.Keys.ToList();
        }

        // Returns wether the answer provided is correct or not
        public bool checkAnswer(T answer)
        {
            if (!(answers.ContainsKey(answer))){
                throw new UnityException("Key doesnot exist");
            }
            return answers[answer]; 
        }

            
    }
}