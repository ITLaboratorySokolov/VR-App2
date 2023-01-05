using System;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField()]
    DisplayLineController displayLineController;
    [SerializeField()]
    FileBrowserController fileBrowserController;

    [Header("Input fields")]
    [SerializeField()]
    TMP_InputField brushName;
    [SerializeField()]
    TMP_InputField path;

    Brush currentBrush;
    LineExporter le;

    // Start is called before the first frame update
    void Start()
    {
        // generate a brush with constant width and black color
        float[] yValues = new float[1];
        yValues[0] = 1;
        currentBrush = new Brush() { Width = 0.05f, Color = Color.black, Name = "SampleBrush", TimePerIter = 5, WidthModifier = yValues };

        // display the brush
        displayLineController.GenerateExampleLine(currentBrush);
        brushName.text = currentBrush.Name;
        path.text = Application.dataPath;

        le = new LineExporter();
    }

    /// <summary>
    /// Generate simple gradient texture
    /// </summary>
    /// <param name="len"> Length of texture </param>
    /// <returns> Gradient texture </returns>
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

    /// <summary>
    /// Apply button clicked
    /// - interpret user code
    /// </summary>
    public void OnApplyBTClick()
    {
        // TODO interpret code

        double xStep = 1.0 / 5.0;
        float[] yValues = new float[5 + 1];
        for (int i = 0; i < 5 + 1; i++)
        {
            double xValue = i * xStep;
            yValues[i] = Math.Abs((Mathf.Sin((float)(xValue * Mathf.PI)))) + 0.001f;
        }

        // Create texture
        Texture2D tx = GenerateSimpleGradient(6);

        currentBrush.Color = Color.white;
        currentBrush.TimePerIter = 5;
        currentBrush.WidthModifier = yValues;
        currentBrush.Texture = tx;

        displayLineController.GenerateExampleLine(currentBrush);
    }

    /// <summary>
    /// React to brush name changed
    /// </summary>
    public void OnBrushNameChanged()
    {
        // escape string - only allows a-z, A-Z 0-9, _ and a space
        string newName = Regex.Replace(brushName.text, "[^a-zA-Z0-9_ ]+", "", RegexOptions.Compiled);
        
        // set name
        if (newName.Length != 0)
        {
            currentBrush.Name = newName;
            brushName.text = newName;
        }
    }

    /// <summary>
    /// React to browse button pressed
    /// - open folder explorer
    /// </summary>
    public void OnBrowseBT()
    {
        string p = path.text.Trim();
        fileBrowserController.OpenFolder(p, UpdatePathTXT);
    }

    /// <summary>
    /// Update export path 
    /// </summary>
    /// <param name="p"> New export path </param>
    public void UpdatePathTXT(string p)
    {
        path.text = p;
    }

    /// <summary>
    /// React to export button pressed
    /// - export brush and its texture
    /// </summary>
    public void OnExportBT()
    {
        le.ExportBrush(currentBrush, path.text.Trim());
    }
}
