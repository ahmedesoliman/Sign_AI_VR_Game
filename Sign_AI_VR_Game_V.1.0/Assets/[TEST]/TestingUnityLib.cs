using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TestingUnityLib : MonoBehaviour
{
    [DllImport("UnityLib")]
    public static extern float Multiply(float a, float b);
    void Update()
    {
        Debug.Log("Yohooo: " + Multiply(10, 10));
    }
}

