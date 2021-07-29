using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRLookWalk : MonoBehaviour
{
    public Transform vrCamera;
    public Transform currentObject;

    public float toggleAngle = 30.0f;

    public float speed = 5.0f;
    public float rotationSpeed;

    public bool moveForward;

    private CharacterController cc;
    
    [SerializeField] private bool moveByHands = false; // By default move by looking downwards

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        currentObject = GetComponent<Transform>();
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
        else
        {
           
        }
    }
    private void moveForwardFunc()
    {

        Vector3 forward = vrCamera.TransformDirection(Vector3.forward);

        cc.SimpleMove(forward * speed);
    }
    private void rotateLeft() { 

        char predictChar = Predict_Script.getLetter();
    
  
        float horizontalInput =1f;
        float verticalInput = 1f;

/*
        if (predictChar == 'l')
        {
            float horizontalInput = horizontalMaxInput--;
        }

        if (predictChar = 'w')
        {

            float verticalInput = verticalMinInput++;
        }*/


        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        movementDirection.Normalize();

        transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

    }

    private void rotateRight()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        movementDirection.Normalize();

        transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

    }
}
