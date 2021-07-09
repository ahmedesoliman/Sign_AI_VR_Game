using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;   // conversion int -> string
using TMPro;


public class API : MonoBehaviour {  
    private string result;
    public TMP_Text text;
    private int counter =0;
    
    void Start() {
        StartCoroutine(GetText());
    }
    
    void Update(){
  // 	Debug.Log("Counter: " + counter);
//	text.text = counter.ToString();
	
//	counter++;

    	text.text = result;	    
    }    

    IEnumerator GetText() {
        UnityWebRequest www = UnityWebRequest.Get("https://newton.now.sh/api/v2/simplify/89*2");
        yield return www.SendWebRequest();
 
        // Error case
        if(www.isNetworkError || www.isHttpError) {
           Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Create object
           DeserializeJSON obj1 = DeserializeJSON.CreateFromJSON(www.downloadHandler.text);
            Debug.Log("Result: " + obj1.result);
            result = obj1.result;
	}
    }
}
