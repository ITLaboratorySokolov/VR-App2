using System.Xml.Serialization;
using UnityEngine;
using PythonExecutionLibrary;
using Python.Runtime;
using System.Globalization;
using System;

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

            // color
            Color = ConvertToColor(obj[0], baseCulture);

            Width = (float)obj[1].ToDouble(baseCulture);
            Width = MathF.Max(1, MathF.Min(100, Width));
            Width *= 0.001f;

            // width modifier
            long wLen = obj[2].Length();
            WidthModifier = new float[wLen];
            for (int i = 0; i < wLen; i++)
                WidthModifier[i] = (float)obj[2][i].ToDouble(baseCulture);

            TimePerIter = (float)obj[3].ToDouble(baseCulture);

            if (obj.Length() == 6)
            {
                // tex size
                int width = obj[4][0].ToInt32(baseCulture);
                int height = obj[4][1].ToInt32(baseCulture);
                Texture2D tex = new Texture2D(width, height);

                // pixels
                var cols = tex.GetPixels();
                PyObject pxs = obj[5];
                for (int i = 0; i < pxs.Length(); i++)
                {
                    PyObject c = pxs[i];
                    Color col = ConvertToColor(c, baseCulture);
                    cols[i] = col;
                }

                tex.SetPixels(cols);
                tex.Apply();

                Texture = tex;
            }

        }
        catch (Exception ex)
        {
           Debug.LogError(ex.Message);
           return false;
        }

        return true;
    }

    private Color ConvertToColor(PyObject pyC, CultureInfo baseCulture)
    {
        float r = (float)pyC[0].ToDouble(baseCulture);
        float g = (float)pyC[1].ToDouble(baseCulture);
        float b = (float)pyC[2].ToDouble(baseCulture);
        float a = 1;

        if (pyC.Length() > 3)
            a = (float)pyC[3].ToDouble(baseCulture);
        Color c = new Color(r, g, b, a);

        return c;
    }
}
