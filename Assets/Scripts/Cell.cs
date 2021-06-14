using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    // State vars
    bool currentState = false;
    bool nextState = false;

    GameObject[] neighboringCellObjects = new GameObject[8];

    // Cached reference
    SpriteRenderer squareSprite;

    // Start is called before the first frame update
    void Awake()
    {
        squareSprite = GetComponent<SpriteRenderer>();
        UpdateCellColor();
    }

    public void AddCellNeighbor(GameObject cellObject)
    {
        for (int index = 0; index < neighboringCellObjects.Length; index++)
        {
            if (neighboringCellObjects[index] == null)
            {
                neighboringCellObjects[index] = cellObject;
                break;
            }
        }
    }

    public void ClearCellNeighbors()
    {
        for (int index = 0; index < neighboringCellObjects.Length; index++)
        {
            neighboringCellObjects[index] = null;
        }
    }

    public void CheckNextState()
    {
        int totalNeighbors = 0;
        int neighborsAlive = 0;

        for (int index = 0; index < neighboringCellObjects.Length; index++)
        {
            if (neighboringCellObjects[index] == null) break;
            else totalNeighbors += 1;

            if (neighboringCellObjects[index].GetComponent<Cell>().GetCurrentState() == true) neighborsAlive += 1;
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
        UpdateCellColor();
    }

    public void SetState(bool state)
    {
        currentState = state;
        UpdateCellColor();
    }

    public void UpdateState()
    {
        currentState = nextState;
        nextState = false;
        UpdateCellColor();
    }

    void UpdateCellColor()
    {
        if (currentState) squareSprite.color = new Color (0.5f, 0.5f, 0.5f);
        else squareSprite.color = new Color(1f, 1f, 1f);
    }   
}
