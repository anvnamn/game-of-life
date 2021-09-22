using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Orchestrator : MonoBehaviour
{
    // Config Params
    static int sizeX = 640;
    static int sizeY = 480;
    [SerializeField] AudioClip tick;
    [SerializeField] GameObject gamePlane;
    [SerializeField] GameObject stepDelaySliderObject;

    // State vars
    bool gameIsRunning = false;
    Cell[,] cellArray;
    float previousGameStep = 0f;
    public float gameStepDelay;
    float ratioOfLiveCells = 0f;
    float cameraSize = 60f;

    // Cached reference
    AudioSource audioSource;
    TextureGenerator textureGenerator;
    Slider stepDelaySlider;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        textureGenerator = gamePlane.GetComponent<TextureGenerator>();
        stepDelaySlider = stepDelaySliderObject.GetComponent<Slider>();

        cellArray = new Cell[sizeX, sizeY];
        textureGenerator.InitTexture(cellArray);
        InitiateCells();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameIsRunning ^= true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            DoGameStep();
            gameIsRunning = false;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            RandomizeCells();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            KillAllCells();
            gameIsRunning = false;
        }

        cameraSize = cameraSize + Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 500;
        if (cameraSize < 1f) cameraSize = 1f;
        else if (cameraSize > 60f) cameraSize = 60f;
        Camera.main.orthographicSize = cameraSize;


        gameStepDelay = Mathf.Pow(stepDelaySlider.value, 10f);

        if(gameIsRunning && Time.time > previousGameStep + gameStepDelay)
        {
            previousGameStep = Time.time;
            if (gameIsRunning) DoGameStep();
        }

        print(gameStepDelay);

        textureGenerator.UpdateTexture(cellArray);
    }

    private void KillAllCells()
    {
        foreach (Cell cell in cellArray)
        {
            cell.SetState(false);
        }
    }

    void DoGameStep()
    {
        ratioOfLiveCells = RatioOfLiveCells();
        audioSource.pitch = ratioOfLiveCells * 10 + 0.1f;
        if (audioSource.pitch > 3f) audioSource.pitch = 3f;
        if (ratioOfLiveCells > 0.0001)
        {
            audioSource.PlayOneShot(tick);
        }
        foreach (Cell cell in cellArray)
        {
            cell.CheckNextState();
        }
        foreach (Cell cell in cellArray)
        {
            cell.SetNextState();
        }
    }

    float RatioOfLiveCells()
    {
        float totalCells = cellArray.Length;
        float liveCells = 0;

        foreach (Cell cell in cellArray)
        {
            if (cell.GetCurrentState()) liveCells++;
        }
        return liveCells / totalCells;
    }

    void RandomizeCells()
    {
        foreach (Cell cell in cellArray)
        {
            bool randomBool;
            if (Random.value > 0.5f) randomBool = true;
            else randomBool = false;
            cell.SetState(randomBool);
        }
    }

    void InitiateCells()
    {
        for (int row = 0; row < sizeY; row++)
        {
            for (int column = 0; column < sizeX; column++)
            {
                Cell cell = new Cell();
                // Instantiate(cellPrefab, new Vector3(column, row, 0f), Quaternion.identity);
                cellArray[column, row] = cell;
            }
        }
        CreateCellLinks();
    }

    void CreateCellLinks()
    {
        for (int row = 0; row < sizeY; row++)
        {
            for (int column = 0; column < sizeX; column++)
            {
                Cell cellClass = cellArray[column, row];

                bool topRow = false;
                bool bottomRow = false;
                bool leftColumn = false;
                bool rightColumn = false;

                if (row < 1) topRow = true;
                if (row == sizeY - 1) bottomRow = true;
                if (column < 1) leftColumn = true;
                if (column == sizeX - 1) rightColumn = true;

                //Neighboring cells notation (origin cell notated as X)
                //NW    N   NE
                //W     X   E
                //SW    S   SE

                // NW
                if (topRow == false && leftColumn == false)
                {
                    cellClass.AddCellNeighbor(cellArray[column - 1, row - 1]);
                }
                // N
                if (topRow == false)
                {
                    cellClass.AddCellNeighbor(cellArray[column, row - 1]);
                }
                // NE
                if (topRow == false && rightColumn == false)
                {
                    cellClass.AddCellNeighbor(cellArray[column + 1, row - 1]);
                }

                // W
                if (leftColumn == false)
                {
                    cellClass.AddCellNeighbor(cellArray[column - 1, row]);
                }
                // E
                if (rightColumn == false)
                {
                    cellClass.AddCellNeighbor(cellArray[column + 1, row]);
                }

                // SW
                if (bottomRow == false && leftColumn == false)
                {
                    cellClass.AddCellNeighbor(cellArray[column - 1, row + 1]);
                }
                // S
                if (bottomRow == false)
                {
                    cellClass.AddCellNeighbor(cellArray[column, row + 1]);
                }
                // SE
                if (bottomRow == false && rightColumn == false)
                {
                    cellClass.AddCellNeighbor(cellArray[column + 1, row + 1]);
                }
            }
        }
    }
}