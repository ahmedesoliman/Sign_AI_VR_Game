﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRLookWalk : MonoBehaviour
{
    public Transform vrCamera;
    public Transform currentObject;

    public float toggleAngle = 30.0f;

    public float speed = 5.0f;

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
            moveLeft();
        }
        else if (predictChar == 'v')
        {
            moveRight();
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
    private void moveLeft()
    {
        Quaternion left = currentObject.rotation;
        left.y -= 1;
        currentObject.rotation = left;

    }

private void moveRight()
    {
        Quaternion right = currentObject.rotation;
        right.y -= 1;
        currentObject.rotation = right;
    }
}
