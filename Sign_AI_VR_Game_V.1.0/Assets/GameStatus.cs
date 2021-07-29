using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour
{
    public enum Status
    {
        PLAYING,
        NEXT,
        DONE
    }
    public enum Level
    {
        LEVEL1,
        LEVEL2,
        LEVEL3,
        LEVEL4
    }


    private static Level currentLevel;
    public static void goNextLevel()
    {
        currentLevel++;
    }

    // Returns the current level
    public static Level getCurrentLevel()
    {
        return currentLevel;
    }



    private void Awake()
    {
        // Set the values
        currentLevel = 0;

    }
}
