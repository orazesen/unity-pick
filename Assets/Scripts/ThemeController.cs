using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeController : MonoBehaviour
{

    public ColorPrefab coloringObjs;
    public Texture2D texture;
    public RawImage image;

    // Start is called before the first frame update
    void Start()
    {
        ChangeColor();
    }

    public void ChangeColor()
    {
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                GenerateColor(i,j);
            }
        }
        image.texture = texture;
    }

    void GenerateColor(int x, int y)
    {
        Color pixelColor = texture.GetPixel(x, y);
        if (pixelColor.a == 0)
        {
            return;
        }
        pixelColor = coloringObjs.leftColor;
    }
}
