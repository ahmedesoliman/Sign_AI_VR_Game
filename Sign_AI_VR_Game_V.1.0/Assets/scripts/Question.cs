using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question 
{
    // Pattern :  -> What is the letter + <char> + ? 
    private string initialString;
    private char letter;

    // Constructors
    public Question(string defaultTitle, char letter = (char)0)
    {
        initialString = defaultTitle + " ";
        this.letter = letter;
    }

    public void changeQuestion(int asciiVal)
    {
        if (asciiVal < 97 || asciiVal > 122)
        {
            Debug.LogError("Error character not supported");
        }
        else { 
            letter = (char)asciiVal;
        }
    }
    public string getQuestion()
    {
        return initialString + letter + " ?";
    }

}
