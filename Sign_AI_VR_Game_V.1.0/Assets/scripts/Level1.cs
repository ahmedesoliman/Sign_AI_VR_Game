using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : MonoBehaviour
{
    public GameObject thePlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }
    // This method is activated when the collider tag pre-defined at Inspector tab.

    void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == thePlayer.name)
            {
                Debug.Log("Changed variable");
                Runtime.isLevel1 = true;
                Runtime.currentLevel++;
            }
        }
}
