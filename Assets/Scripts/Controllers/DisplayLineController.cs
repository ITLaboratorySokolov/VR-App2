using UnityEngine;

/// <summary>
/// Class used to display preview of a brush stroke
/// </summary>
public class DisplayLineController : MonoBehaviour
{
    /// <summary> Start of the line in scene </summary>
    [SerializeField]
    public Transform beginning;
    /// <summary> End of the line in scene </summary>
    [SerializeField]
    public Transform end;
    /// <summary> Line object </summary>
    [SerializeField]
    public GameObject lineObj;

    /// <summary> Triangle strip generator </summary>
    TriangleStripGenerator tg;

    // Start is called before the first frame update
    void Start()
    {
        tg = new TriangleStripGenerator();
    }

    /// <summary>
    /// Generate example line going from beggining to end drawn with brush
    /// </summary>
    /// <param name="brush"> Brush to draw the line with </param>
    public void GenerateExampleLine(Brush brush)
    {
        TriangleStrip ts = new TriangleStrip(beginning.position);
        
        // set color and texture
        lineObj.GetComponent<Renderer>().material.SetTexture("_MainTex", brush.Texture);
        lineObj.GetComponent<Renderer>().material.SetColor("_Color", brush.Color);

        // make a triangle strip from beggining to end
        float step = 0.05f;
        int modLen = brush.WidthModifier.Length;
        float widthModifier = brush.WidthModifier[0];
        float distBE = Vector3.Distance(beginning.transform.position, end.transform.position);
        float distanceStep = distBE / (modLen-1);

        float i = 0;
        Vector3 newPos = beginning.position;
        while (newPos != end.transform.position)
        {
            newPos = beginning.position + i * (end.position - beginning.position);
            float distT = Vector3.Distance(beginning.transform.position, newPos);

            // Get width
            int startIn = 0;
            int endIn = 0;
            float t = 0;

            if (modLen > 1)
            {
                startIn = (int)((distT)/ distanceStep);
                endIn = startIn + 1;
                if (newPos == end.transform.position)
                    widthModifier = brush.WidthModifier[modLen - 1];
                else
                {
                    t = (distT % distanceStep) / distanceStep;
                    widthModifier = Mathf.Lerp(brush.WidthModifier[startIn], brush.WidthModifier[endIn], t);
                }
            }

            // Get UVs
            float v = 0.5f;
            float uSt = 0;
            float uEn = 0;
            if (modLen > 1)
            {
                uSt = startIn / (float)(modLen - 1);
                uEn = endIn / (float)(modLen - 1);
            }
            float u = Mathf.Lerp(uSt, uEn, t);

            // Generate next part of the triangle strip
            tg.AddPointToLine(newPos, brush.Width * widthModifier, beginning.transform.up.normalized, ts, u, v);

            i += step;
            if (distT > distBE)
                newPos = end.transform.position;
        }

        lineObj.GetComponent<MeshFilter>().mesh = ts.mesh;
    }
}
