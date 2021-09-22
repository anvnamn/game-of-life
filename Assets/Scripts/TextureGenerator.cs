using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{

    Texture2D texture;

    int sizeX, sizeY;

    public void InitTexture(Cell[,] cellArray)
    {
        sizeX = cellArray.GetLength(0);
        sizeY = cellArray.GetLength(1);
        texture = new Texture2D(sizeX, sizeY, TextureFormat.ARGB32, false);
    }

    public void UpdateTexture(Cell[,] cellArray)
    {
        texture.filterMode = FilterMode.Point;
        Color[] colorArray = new Color[sizeX*sizeY];

        for (int yCoord = 0; yCoord < sizeY; yCoord++)
        {
            for (int xCoord = 0; xCoord < sizeX; xCoord++)
            {
                if (cellArray[xCoord,yCoord].GetCurrentState()) colorArray[xCoord + sizeX*yCoord] = Color.black;
                else colorArray[xCoord + sizeX*yCoord] = Color.white;
            }
        }

        texture.SetPixels(0, 0, sizeX, sizeY, colorArray,0);

        texture.Apply();
        GetComponent<Renderer>().material.mainTexture = texture;
    }
}
