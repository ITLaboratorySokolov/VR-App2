using System.Xml.Serialization;
using UnityEngine;
using Python.Runtime;
using System.Globalization;
using System;
using ZCU.PythonExecutionLibrary;

/// <summary> Class representing a brush </summary>
public class Brush : IDrawInstrument, IReturnable
{
    /// <summary>
    /// Brush color
    /// </summary>
    Color color;
    public Color Color { get => color; set => color = value; }

    /// <summary>
    /// Brush texture
    /// </summary>
    Texture2D texture;
    [XmlIgnore]
    public Texture2D Texture { get => texture; set => texture = value; }
    [XmlIgnore]
    Color[] textureData;
    [XmlIgnore]
    int width;
    [XmlIgnore]
    int height;

    /// <summary> Width modifiers, values 0-1 - percentage of width at constant timesteps </summary>
    float[] widthModifier;
    public float[] WidthModifier { get => widthModifier; set => widthModifier = value; }

    /// <summary> Time per one iteration of width </summary>
    float timePerIter;
    public float TimePerIter { get => timePerIter; set => timePerIter = value; }

    /// <summary>
    /// Expecting a python list [color, width, widthModifier, time, texSize, pixels]
    /// where - name is a string, color is a list [r, g, b], widthModifier is a list [], texSize is list of ints [width, height], pixels is a list of colors [color, color, color...]
    /// 
    /// </summary>
    /// <param name="obj"> python list </param>
    /// <returns></returns>
    public bool SetParameters(PyObject obj)
    {
        // get
        CultureInfo baseCulture = new CultureInfo("en-US"); 

        try
        {
            if (obj.Length() < 4 || obj.Length() > 6)
                throw new Exception("Wrong returned arguments");

            // color - rgb 0-1
            Color = ConvertToColor(obj[0], baseCulture);

            // width - <1-100>
            Width = (float)obj[1].ToDouble(baseCulture);
            Width = ConvertorHelper.Clamp(Width, 1, 100);
            Width *= 0.001f; // multiplied 0.001 for better displaying in scene

            // width modifier <0-1>
            long wLen = obj[2].Length();
            WidthModifier = new float[wLen];
            for (int i = 0; i < wLen; i++)
            {
                WidthModifier[i] = (float)obj[2][i].ToDouble(baseCulture);
                WidthModifier[i] = ConvertorHelper.Clamp(WidthModifier[i], 0, 1);
            }

            // time per iteration
            TimePerIter = (float)obj[3].ToDouble(baseCulture);

            // texture
            if (obj.Length() == 6)
            {
                // tex size - >0
                width = obj[4][0].ToInt32(baseCulture);
                width = (int)ConvertorHelper.Clamp(width, 1, int.MaxValue);
                height = obj[4][1].ToInt32(baseCulture);
                height = (int)ConvertorHelper.Clamp(height, 1, int.MaxValue);
                
                // Texture2D tex = new Texture2D(width, height);

                // pixels - rgbs <0-1>
                // var cols = tex.GetPixels();
                textureData = new Color[width * height];
                PyObject pxs = obj[5];
                for (int i = 0; i < pxs.Length(); i++)
                {
                    PyObject c = pxs[i];
                    Color col = ConvertToColor(c, baseCulture);
                    textureData[i] = col;
                }

                // tex.SetPixels(cols);
                // tex.Apply();

                // Texture = tex;
            }

        }
        catch (Exception ex)
        {
           Debug.LogError(ex.Message);
           return false;
        }

        return true;
    }

    public void SetTexture()
    {
        Texture2D tex = new Texture2D(width, height);
        tex.SetPixels(textureData);
        tex.Apply();
        Texture = tex;
    }

    /// <summary>
    /// Convert pyObject to Color
    /// </summary>
    /// <param name="pyC"> PyObject </param>
    /// <param name="baseCulture"> Used base culture </param>
    /// <returns> Created color </returns>
    private Color ConvertToColor(PyObject pyC, CultureInfo baseCulture)
    {
        float r = (float)pyC[0].ToDouble(baseCulture);
        float g = (float)pyC[1].ToDouble(baseCulture);
        float b = (float)pyC[2].ToDouble(baseCulture);
        float a = 1;

        r = ConvertorHelper.Clamp(r, 0, 1);
        g = ConvertorHelper.Clamp(g, 0, 1);
        b = ConvertorHelper.Clamp(b, 0, 1);

        if (pyC.Length() > 3)
            a = (float)pyC[3].ToDouble(baseCulture);
        a = ConvertorHelper.Clamp(a, 0, 1);

        Color c = new Color(r, g, b, a);
        return c;
    }
}
