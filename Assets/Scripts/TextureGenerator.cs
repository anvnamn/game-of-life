using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateTexture(Cell[,] cellArray)
    {
        int sizeX = cellArray.GetLength(0);
        int sizeY = cellArray.GetLength(1);

        print("Texture size:" + sizeX + " x " + sizeY);

        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        Texture2D texture = new Texture2D(sizeX, sizeY, TextureFormat.ARGB32, false);
        
        texture.filterMode = FilterMode.Point;

        Color[] colorArray = new Color[sizeX*sizeY];

        // set the pixel values
        for (int yCoord = 0; yCoord < sizeY; yCoord++)
        {
            for (int xCoord = 0; xCoord < sizeX; xCoord++)
            {
                if (cellArray[xCoord,yCoord].GetCurrentState()) colorArray[xCoord + sizeX*yCoord] = Color.black;
                else colorArray[xCoord + sizeX*yCoord] = Color.white;
            }
        }

        texture.SetPixels(0, 0, sizeX, sizeY, colorArray,0);

        // Apply all SetPixel calls
        texture.Apply();

        // connect texture to material of GameObject this script is attached to
        GetComponent<Renderer>().material.mainTexture = texture;
    }
}
