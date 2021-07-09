using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeserializeJSON
{
    public string operation, expression, result,text_out;

    public static DeserializeJSON CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<DeserializeJSON>(jsonString);
        
    }


    // Given JSON input:
    // {"name":"Dr Charles","lives":3,"health":0.8}
    // this example will return a PlayerInfo object with
    // name == "Dr Charles", lives == 3, and health == 0.8f.ll
}
