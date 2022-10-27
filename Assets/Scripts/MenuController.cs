using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Image im;

    // Start is called before the first frame update
    void Start()
    {
        LineImporter li = new LineImporter();

        // load brush from file
        Brush b = li.ImportBrush("Test brush");

        // write out to console
        Debug.Log(b.Name + " " + b.Width + " " + b.Color);
        WriteKeys(b.WidthCurve);

        im.sprite = LoadNewSprite(b.Texture);
        im.SetNativeSize();

    }

    public Sprite LoadNewSprite(Texture2D texture, float PixelsPerUnit = 100.0f)
    {
        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), PixelsPerUnit);
        //Instantiate(newSprite);
        return newSprite;
    }

    private void WriteKeys(AnimationCurve c)
    {
        Keyframe[] ks = c.keys;
        Debug.Log(ks.Length);
        for (int i = 0; i < ks.Length; i++)
            Debug.Log(ks[i].time + " " + ks[i].value);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
