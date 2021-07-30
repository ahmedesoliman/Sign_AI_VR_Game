using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectAnswer : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
       if(other.tag == "Player")
        {
            Debug.Log("Correct");
            Runtime.point++;
            Quiz.updateScore();
        }
    }
}
