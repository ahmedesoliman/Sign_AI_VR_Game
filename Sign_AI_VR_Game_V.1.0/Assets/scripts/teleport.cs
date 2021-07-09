using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour
{
    public Transform teleportTarget;
    public GameObject thePlayer;
    public bool delay; 
    public float delayTime = 0;
    public bool isCountPoint;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == thePlayer.name)
        {
            if (delay){
                Invoke("DoTransform", delayTime);
            }
            else
            {
                DoTransform();
            }
            if (isCountPoint)
            {
                Runtime.point++;
            }
            else{
                Runtime.health--;
            }
        }
        
    }
    
    void DoTransform()
    {
        thePlayer.transform.position = teleportTarget.transform.position;
    }
}
