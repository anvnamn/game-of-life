using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    // State vars
    bool currentState = false;
    bool nextState = false;

    Cell[] neighboringCells = new Cell[8];

    // Start is called before the first frame update
    void Awake()
    {
    }

    public void AddCellNeighbor(Cell cell)
    {
        for (int index = 0; index < neighboringCells.Length; index++)
        {
            if (neighboringCells[index] == null)
            {
                neighboringCells[index] = cell;
                break;
            }
        }
    }

    public void CheckNextState()
    {
        int neighborsAlive = 0;

        for (int index = 0; index < neighboringCells.Length; index++)
        {
            if (neighboringCells[index] == null) break;
            if (neighboringCells[index].GetCurrentState() == true) neighborsAlive += 1;
        }

        if (currentState && (neighborsAlive == 2 || neighborsAlive == 3))
        {
            nextState = true;
        }
        else if (!currentState && neighborsAlive == 3)
        {
            nextState = true;
        }
        else
        {
            nextState = false;
        }
    }

    public bool GetCurrentState()
    {
        return currentState;
    }

    public void ToggleState()
    {
        currentState = !currentState;
    }

    public void SetState(bool state)
    {
        if (currentState != state)
        {
            currentState = state;
        }
    }

    public void SetNextState()
    {
        currentState = nextState;
    }
}
