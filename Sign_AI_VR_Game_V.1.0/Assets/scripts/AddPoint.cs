using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // This method is activated when the collider tag called "Player" collides. Any other tag that is not defined as "Player will not have effect.
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player"){
             Runtime.point++;
            Destroy(gameObject);
        }
    }
}
