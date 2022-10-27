using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class LineExporter
{
    public bool ExportBrush(Brush brush)
    {
        // TODO dirPath!!
        var dirPath = Application.dataPath;
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        // Serialize brush information
        TextWriter tw = new StreamWriter(brush.Name + ".xml");
        XmlSerializer x = new XmlSerializer(brush.GetType());
        x.Serialize(tw, brush);
        tw.Close();

        // Decompress texture
        Texture2D texture = DeCompress(brush.Texture);
        
        // Save To Disk as PNG
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(brush.Name + ".png", bytes);

        return true;
    }

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
