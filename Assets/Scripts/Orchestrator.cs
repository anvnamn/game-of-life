using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orchestrator : MonoBehaviour
{   
    // Config Params
    [SerializeField] GameObject cellPrefab;
    [SerializeField] int sizeX = 40;
    [SerializeField] int sizeY = 30;
    [SerializeField] AudioClip tick;
    [SerializeField] float period = 0.1f;

    // State vars
    int listLength;
    bool gameIsRunning = false;
    bool doOneStep = false;
    float cellSizeX;
    float cellSizeY;
    float smallestCellSize;
    float nextIteration = 0f;

    // Cached reference
    AudioSource audioSource;

    List<GameObject> cellList;

    void Start()
    {
        InitiateCells();
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        cellList = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameIsRunning ^= true;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            doOneStep = true;
            gameIsRunning = false;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RandomizeCells();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            KillAllCells();
            gameIsRunning = false;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && sizeY < 100)
        {
            AddRow();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && sizeY > 5)
        {
            RemoveRow();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && sizeX < 100)
        {
            AddColumn();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && sizeX > 5)
        {
            RemoveColumn();
        }

        if ((Time.time > nextIteration && gameIsRunning == true) || doOneStep == true)
        {
            nextIteration = Time.time + period;
            DoGameStep();
        }
    }

    private void KillAllCells()
    {
        foreach (GameObject cellObject in cellList)
        {
            cellObject.GetComponent<Cell>().SetState(false);
        }
    }

    void DoGameStep()
    {
        doOneStep = false;
        foreach (GameObject cellObject in cellList)
        {
            cellObject.GetComponent<Cell>().CheckNextState();
        }
        foreach (GameObject cellObject in cellList)
        {
            cellObject.GetComponent<Cell>().UpdateState();
        }
        float ratioOfLiveCells = RatioOfLiveCells();
        audioSource.pitch = ratioOfLiveCells * 10 + 0.1f;
        if (audioSource.pitch > 3f) audioSource.pitch = 3f;
        if (ratioOfLiveCells > 0.0001)
        {
            audioSource.PlayOneShot(tick);
        }
    }

    float RatioOfLiveCells()
    {
        float totalCells = listLength;
        float liveCells = 0;

        foreach (GameObject cellObject in cellList)
        {
            if (cellObject.GetComponent<Cell>().GetCurrentState()) liveCells++;
        }
        return liveCells / totalCells;
    }

    void UpdateCellSize()
    {
        cellSizeX = Camera.main.orthographicSize * 2 * 4 / (3 *sizeX);
        cellSizeY = Camera.main.orthographicSize * 2 / sizeY;

        if (cellSizeX < cellSizeY)
        {
            smallestCellSize = cellSizeX;
        }
        else
        {
            smallestCellSize = cellSizeY;
        }
    }

    void RandomizeCells()
    {
        foreach (GameObject cellObject in cellList)
        {
            bool randomBool;
            if (Random.value > 0.5f) randomBool = true;
            else randomBool = false;
            cellObject.GetComponent<Cell>().SetState(randomBool);
        }
    }

    void InitiateCells()
    {
        for (int row = 0; row < sizeY; row++)
        {
            for (int column = 0; column < sizeX; column++)
            {
                cellList.Add(InstantiateCell());
            }
        }
        UpdateCellList();
    }

    void RepositionRescaleCells()
    {
        int listIndex = 0;

        // Determine start of rows based on cell size
        float totalRowLength = sizeX * smallestCellSize;
        float rowOffset = (40 - totalRowLength) / 2;

        // Determine start of colums based on cell size
        float totalColumnHeight = sizeY * smallestCellSize;
        float columnOffset = (30 - totalColumnHeight) / 2;

        for (int row = 0; row < sizeY; row++)
        {
            for (int column = 0; column < sizeX; column++)
            {
                GameObject cell = cellList[listIndex].gameObject;
                cell.transform.position =
                    new Vector3(
                        column * smallestCellSize + smallestCellSize / 2 + rowOffset,
                        -row * smallestCellSize - smallestCellSize / 2 - columnOffset,
                        0f);
                cell.name = "Cell " + column + "-" + row;
                cell.transform.localScale = new Vector3(smallestCellSize, smallestCellSize, 1);
                listIndex++;
            }
        }
    }

    GameObject InstantiateCell()
    {
        GameObject cellInstance = Instantiate(cellPrefab);
        return cellInstance;
    }

    void AddRow()
    {
        for (int column = 0; column < sizeX; column++)
        {
            cellList.Add(InstantiateCell());
        }
        sizeY += 1;
        UpdateCellList();
    }

    void RemoveRow()
    {
        for (int column = 0; column < sizeX; column++)
        {
            int lastItemIndex = cellList.Count - 1;
            Destroy(cellList[lastItemIndex]);
            cellList.RemoveAt(lastItemIndex);
        }
        sizeY -= 1;
        UpdateCellList();
    }

    void AddColumn()
    {
        for (int index = sizeX ; index < (sizeX + 1) * sizeY; index += sizeX + 1)
        {
            cellList.Insert(index,InstantiateCell());
        }
        sizeX += 1;
        UpdateCellList();
    }

    void RemoveColumn()
    {
        for (int index = listLength - 1; index > 0; index -= sizeX)
        {
            Destroy(cellList[index]);
            cellList.RemoveAt(index);
        }
        sizeX -= 1;
        UpdateCellList();
    }

    private void UpdateCellList()
    {
        listLength = cellList.Count;
        UpdateCellLinks();
        UpdateCellSize();
        RepositionRescaleCells();
    }

    void UpdateCellLinks()
    {
        bool topRow, bottomRow, leftColumn, rightColumn;
        int index = 0;
        Cell cellClass;

        foreach (GameObject cellObject in cellList)
        {
            cellClass = cellObject.GetComponent<Cell>();
            cellClass.ClearCellNeighbors();

            topRow = false;
            bottomRow = false;
            leftColumn = false;
            rightColumn = false;

            if (index < sizeX) topRow = true;
            if (index >= listLength - sizeX) bottomRow = true;

            if (index % sizeX == 0) leftColumn = true;
            if (index % sizeX == sizeX - 1) rightColumn = true;

            //Neighboring cells notation (origin cell notated as X)
            //NW    N   NE
            //W     X   E
            //SW    S   SE

            // NW
            if (topRow == false && leftColumn == false)
            {
                cellClass.AddCellNeighbor(cellList[index - sizeX - 1]);
            }
            // N
            if (topRow == false)
            {
                cellClass.AddCellNeighbor(cellList[index - sizeX]);
            }
            // NE
            if (topRow == false && rightColumn == false)
            {
                cellClass.AddCellNeighbor(cellList[index - sizeX + 1]);
            }

            // W
            if (leftColumn == false)
            {
                cellClass.AddCellNeighbor(cellList[index - 1]);
            }
            // E
            if (rightColumn == false)
            {
                cellClass.AddCellNeighbor(cellList[index + 1]);
            }

            // SW
            if (bottomRow == false && leftColumn == false)
            {
                cellClass.AddCellNeighbor(cellList[index + sizeX - 1]);
            }
            // S
            if (bottomRow == false)
            {
                cellClass.AddCellNeighbor(cellList[index + sizeX]);
            }
            // SE
            if (bottomRow == false && rightColumn == false)
            {
                cellClass.AddCellNeighbor(cellList[index + sizeX + 1]);
            }
            index++;
        }
    }

}
