using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRLookWalk : MonoBehaviour
{
    public Transform vrCamera;

    public float toggleAngle = 30.0f;

    public float speed = 5.0f;

    public bool moveForward = true;

    private CharacterController cc;
    
    [SerializeField] private bool moveByHands = false; // By default move by looking downwards

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveByHands)
        {
            moveByHand();
        }
        else
        {
            moveByLooking();
        }

    }

    private void moveByLooking()
    {
        if (vrCamera.eulerAngles.x >= toggleAngle && vrCamera.eulerAngles.x < 90.0f)
        {
            moveForward = true;

        }
        else
        {
            moveForward = false;
        }

        if (moveForward)
        {
            moveForwardFunc();
        }
    }
    
    private void moveByHand()
    {
        char predictChar = Predict_Script.getLetter();
        Debug.Log("The character returned is ---> :" + predictChar);

        // Move Forward
        if (predictChar == 'c' )
        {
            moveForwardFunc();
        }

        else if (predictChar == 'l')
        {
            //change the angle to left side
            rotateLeft();
        }
        else if (predictChar == 'v')
        {
            rotateRight();
        }
 
    }
    private void moveForwardFunc()
    {

        Vector3 forward = vrCamera.TransformDirection(Vector3.forward);

        cc.SimpleMove(forward * speed);
    }
    private void rotateLeft() {

        transform.Rotate(-Vector3.up * (speed * 10) * Time.deltaTime);
    }

    private void rotateRight()
    {
        transform.Rotate(Vector3.up * (speed * 10) * Time.deltaTime);
    }
}
