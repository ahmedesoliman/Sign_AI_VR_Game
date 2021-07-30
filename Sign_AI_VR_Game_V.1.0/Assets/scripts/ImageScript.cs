using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using UnityEngine.UI;


public class ImageScript : MonoBehaviour

{
    public List<Image> Images;
    public List<Sprite> Sprites;

    // Start is called before the first frame update
    void Start()
    {
        AssignSprites(Shuffle(Sprites), Images);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AssignSprites(IList<Sprite> sprites, IList<Image> images)
    {
        for (int i = 0; i < images.Count && i < sprites.Count; ++i)
            images[i].sprite = sprites[i];
    }

    private static IList<T> Shuffle<T>(IList<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        
        IList<T> result = new List<T>(list);
        
        int n = result.Count;
        
        while (n > 1)
        {
            byte[] box = new byte[1];
            
            do provider.GetBytes(box);
            
            while (!(box[0] < n * (byte.MaxValue / n)));
            
            int k = (box[0] % n);
            
            n--;

            T value = result[k];
            
            result[k] = result[n];
            result[n] = value;
        }
        return result;
    }
}
