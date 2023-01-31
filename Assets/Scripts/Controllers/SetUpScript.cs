using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;

// TODO path to config file

/// <summary>
/// Script managing the set up of the application
/// - reads config file
/// </summary>
public class SetUpScript : MonoBehaviour
{
    [Header("Config")]
    /// <summary> Path to config file </summary>
    string pathToConfig;

    [Header("Python library")]
    /// <summary> User code processor </summary>
    [SerializeField]
    UserCodeProcessor userCodeProcessor;
    /// <summary> Python path </summary>
    private string pythonPath;

    /// <summary>
    /// Set up configuration1before application starts
    /// - read from config min and max recorded depth, horizontal and vertical pan, zoom and server url
    /// </summary>
    private void Awake()
    {
        pathToConfig = Directory.GetCurrentDirectory() + "\\config.txt";
        Debug.Log(pathToConfig);

        // Set culture -> doubles are written with decimal dot
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        ReadConfig();
    }

    private void ReadConfig()
    {
        if (File.Exists(pathToConfig))
        {
            Debug.Log("Loading config file...");
            string[] lines = File.ReadAllLines(pathToConfig);
            if (lines.Length >= 1)
            {
                pythonPath = lines[0].Trim();
                userCodeProcessor.pythonPath = pythonPath;
            }


        }
    }
}
