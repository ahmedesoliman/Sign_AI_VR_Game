using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using TMPro;

public class getText : MonoBehaviour
{

    public TMP_Text changingText;
/*    string output = "";*/

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(callText());
    }
    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator callText()
    {
        StreamReader reader = new StreamReader(@".\Assets\StreamingAssets\RecallText\Alphabets.txt");

        // Read entire text file with ReadToEnd.
        int counter = 0;
        int maxCount = 3;
        string contents = reader.ReadToEnd();


        for (int i = 0; i <= contents.Length; i++)
        {
            if (contents[i] == contents[i + 1])
            {
                counter++;
            }
            else if (contents[i] == contents.Length) {

                i = 0;
            }

            if (counter == maxCount)
            {
                Debug.Log(contents[i]);

                changingText.text = char.ToString(contents[i]);
                yield return new WaitForSeconds(1);
                counter = 0;
            }

        }
        changingText.text = "Finish";
        /*       StartCoroutine (processTask());*/
    }

}



/*        string readFromFilePath = Application.streamingAssetsPath + "/RecallText/" + "Alphabets" + ".txt";
        List<string> fileLine = File.ReadAllLines(readFromFilePath).ToList();

        foreach (string line in fileLine)
        {

            Instantiate(recallTextObject, contentwindow);
            recallTextObject.GetComponent<Text>().text = line;
        }*/
