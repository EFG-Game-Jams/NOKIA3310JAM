using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class NokiaImageEffects : MonoBehaviour
{
    public Color blackResult;
    public Color whiteResult;

    private Texture2D readbackTexture;

    private void Start()
    {
        readbackTexture = new Texture2D(84, 48);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // RT => texture
        Graphics.SetRenderTarget(source);
        readbackTexture.ReadPixels(new Rect(0, 0, 84, 48), 0, 0);

        // color switching
        Debug.Assert(source.width == 84 && source.height == 48);
        for (int j = 0; j < 48; ++j)
        {
            for (int i = 0; i < 84; ++i)
            {
                Color raw = readbackTexture.GetPixel(i, j);
                Color result = (raw.r > .5f ? whiteResult : blackResult);
                readbackTexture.SetPixel(i, j, result);
            }
        }

        // texture => RT
        readbackTexture.Apply();
        Graphics.Blit(readbackTexture, destination);
    }
}
