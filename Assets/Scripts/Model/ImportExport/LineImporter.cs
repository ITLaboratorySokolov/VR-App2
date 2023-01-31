using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class LineImporter
{
    public Brush ImportBrush(string brushname, string dirPath)
    {
        if (dirPath == null || dirPath == "")
            dirPath = Application.dataPath;

        if (!File.Exists(dirPath + "/" + brushname + ".xml"))
            throw new FileNotFoundException();    

        Brush b;
        XmlSerializer serializer = new XmlSerializer(typeof(Brush));

        // Call the Deserialize method to restore the object's state.
        using (Stream reader = new FileStream(brushname + ".xml", FileMode.Open))
            b = (Brush)serializer.Deserialize(reader);

        // Read texture from file with brush name
        Texture2D tex = null;
        byte[] fileData;
        if (File.Exists(dirPath + "/" + brushname + ".png"))
        {
            fileData = File.ReadAllBytes(brushname + ".png");
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        b.Texture = tex;

        return b;
    }
}