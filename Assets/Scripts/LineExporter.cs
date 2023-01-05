using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class LineExporter
{
    /// <summary>
    /// Export brush data to files
    /// - brush file named "<brushname>.xml"
    /// - texture file named "<brushname>.png"
    /// </summary>
    /// <param name="brush"> Brush </param>
    /// <param name="dirPath"> Export path </param>
    /// <returns> True if successful </returns>
    public bool ExportBrush(Brush brush, string dirPath)
    {
        // if path not defined, set default path
        if (dirPath == null || dirPath == "")
            dirPath = Application.dataPath;

        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        // Serialize brush information
        TextWriter tw = new StreamWriter(dirPath + "/" + brush.Name + ".xml");
        XmlSerializer x = new XmlSerializer(brush.GetType());
        x.Serialize(tw, brush);
        tw.Close();

        // Decompress texture
        if (brush.Texture != null)
        {
            Texture2D texture = DeCompress(brush.Texture);

            // Save To Disk as PNG
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(dirPath + "/" + brush.Name + ".png", bytes);
        }

        return true;
    }

    /// <summary>
    /// Decompress texture
    /// </summary>
    /// <param name="source"> Texure2D </param>
    /// <returns> Decopressed texture </returns>
    private Texture2D DeCompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}
