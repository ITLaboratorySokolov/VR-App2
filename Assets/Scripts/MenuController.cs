using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Image im;
    public DisplayLineController displayLineController;

    Brush[] brushes;
    int numberOfBrushes = 2;

    // Start is called before the first frame update
    void Start()
    {
        // Create width modifiers
        double xStep = 1.0 / 5.0;
        float[] yValues = new float[5 + 1];
        for (int i = 0; i < 5 + 1; i++)
        {
            double xValue = i * xStep;
            yValues[i] = Math.Abs((Mathf.Sin((float)(xValue * Mathf.PI)))) + 0.001f;
        }

        // Create texture
        Texture2D tx = GenerateSimpleGradient(6);

        // Manually create two brushes
        brushes = new Brush[numberOfBrushes];
        brushes[0] = new Brush() { Width = 0.05f, Color = Color.white, Texture = tx, Name = "BrushGrad2", TimePerIter = 5, WidthModifier = yValues };
        brushes[1] = new Brush() { Width = 0.05f, Color = Color.black, Texture = tx, Name = "Brush02", TimePerIter = 10, WidthModifier = yValues };

        LineExporter le = new LineExporter();

        le.ExportBrush(brushes[0]);

        LineImporter li = new LineImporter();

        // load brush from file
        Brush b = li.ImportBrush("BrushGrad");

        // write out to console
        Debug.Log(b.Name + " " + b.Width + " " + b.Color);
        WriteArray<float>(b.WidthModifier);

        im.sprite = LoadNewSprite(b.Texture);
        im.SetNativeSize();

    }

    private void WriteArray<T>(T[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
            Debug.Log(arr[i]);
    }

    private Texture2D GenerateSimpleGradient(int len)
    {
        Texture2D tx = new Texture2D(len, 20, TextureFormat.RGB24, false);

        var texPix = tx.GetPixels();

        int updates = 0;
        Color c = new Color(updates / 255.0f, 0.8f, 1 - updates / 255.0f, 1);
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < tx.height; j++)
            {
                texPix[j * tx.width + i] = c;
            }
            c = new Color(updates / 255.0f, 0.3f, 1 - updates / 255.0f, 1);
            updates += 20;
            updates %= 255;
        }

        tx.SetPixels(texPix);
        tx.Apply();

        return tx;
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

    public void OnApplyBTClick()
    {
        // TODO interpret code

        displayLineController.GenerateExampleLine(brushes[0]);
    }

}
