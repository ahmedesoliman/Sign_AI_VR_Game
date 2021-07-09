using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runtime : MonoBehaviour
{
    public static int point = 0;
    public static int health;
    public static int maxHealth = 2;
    public static int currentLevel = 0;
    public static bool isLevel1 = false;
    public static bool isPreLevel2 = false;
    public static bool isLevel2 = false;
    public GameObject level1;
    public GameObject level2;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        unlockedLevel();
    }

    public static void resetValues()
    {
        point = 0;
        currentLevel = 0;
        health = maxHealth;
    }
    void unlockedLevel()
    {
        if (isLevel1)
        {
            level1.SetActive(false);
        }
        if (isLevel2)
        {
            level2.SetActive(false);
        }
    }
}
