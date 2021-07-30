using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectAnswer : Quiz
{
    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {
       if(other.tag == "Player")
        {
            Debug.Log("Correct");
            Runtime.point++;
            setTheObjects();
            teleportToBase();

        }
    }
    void Update()
    {

    }

}
