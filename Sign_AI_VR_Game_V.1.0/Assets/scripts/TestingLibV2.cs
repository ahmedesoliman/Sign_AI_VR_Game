using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TestingLibV2 : MonoBehaviour
{
    [DllImport("UnityV2")]
    public static extern int RandomNumber();
    void Update()
    {
        Debug.Log("Yohooo: " + RandomNumber());
    }
}

