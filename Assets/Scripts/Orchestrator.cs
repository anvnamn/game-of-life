using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orchestrator : MonoBehaviour
{
    [SerializeField] GameObject cellPrefab;
    [SerializeField] int sizeX = 40;
    [SerializeField] int sizeY = 30;

    // State vars
    int listLength;
    bool gameIsRunning = false;
    float cellSizeX;
    float cellSizeY;
    float smallestCellSize;

    List<GameObject> cellList;

    void Start()
    {
        InitiateCells();
        UpdateCellLinks();
    }

    private void Awake()
    {
        cellList = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameIsRunning == false)
            {
                gameIsRunning = true;
                StartCoroutine(RunGame());
            }
            else
            {
                gameIsRunning = false;
            }
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
    }

    private void KillAllCells()
    {
        foreach (GameObject cellObject in cellList)
        {
            cellObject.GetComponent<Cell>().SetState(false);
        }
    }

    IEnumerator RunGame()
    {
        while (true)
        {
            foreach (GameObject cellObject in cellList)
            {
                cellObject.GetComponent<Cell>().CheckNextState();
            }
            foreach (GameObject cellObject in cellList)
            {
                cellObject.GetComponent<Cell>().UpdateState();
            }
            if (gameIsRunning == false) break;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void UpdateCellSize()
    {
        cellSizeX = Camera.main.orthographicSize * 2 * 4 / (3 *sizeX);
        cellSizeY = Camera.main.orthographicSize * 2 / sizeY;
        Debug.Log("New size X: " + cellSizeX);
        Debug.Log("New size Y: " + cellSizeY);

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
                GameObject cellInstance = Instantiate(cellPrefab,
                    new Vector3(column + 0.5f,
                    -row - 0.5f, 0f),
                    Quaternion.identity);
                cellInstance.name = "Cell " + column + "-" + row; 
                cellList.Add(cellInstance);
            }
        }
        listLength = cellList.Count;
    }

    void RepositionRescaleCells()
    {
        int listIndex = 0;

        for (int row = 0; row < sizeY; row++)
        {
            for (int column = 0; column < sizeX; column++)
            {
                GameObject cell = cellList[listIndex].gameObject;
                cell.transform.position =
                    new Vector3(column * smallestCellSize + smallestCellSize / 2,
                    -row * smallestCellSize - smallestCellSize / 2, 0f);
                cell.name = "Cell " + column + "-" + row;
                cell.transform.localScale = new Vector3(smallestCellSize, smallestCellSize, 1);
                listIndex++;
            }
        }
    }

    void AddRow()
    {
        for (int column = 0; column < sizeX; column++)
        {
            GameObject cellInstance = Instantiate(cellPrefab,
                new Vector3(column * smallestCellSize + 0.5f,
                -sizeY - 0.5f, 0f),
                Quaternion.identity);
            cellInstance.name = "WWW " + column + "-" + sizeY;
            cellInstance.transform.localScale = cellInstance.transform.localScale * smallestCellSize;
            cellList.Add(cellInstance);
        }
        sizeY += 1;
        listLength = cellList.Count;
        UpdateCellLinks();
        UpdateCellSize();
        RepositionRescaleCells();
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
