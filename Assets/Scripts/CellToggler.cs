using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellToggler : MonoBehaviour
{
    Ray ray;
    RaycastHit2D hit;

    void Start()
    {
        StartCoroutine(DoRayCastsOnCells());
    }

    IEnumerator DoRayCastsOnCells()
    {
        while (true)
        {
            if (Input.GetMouseButton(0)) SetCellState(true);
            else if (Input.GetMouseButton(1)) SetCellState(false);
            yield return new WaitForSeconds(0.025f); // Run this check 50 times per second
        }
    }

    void SetCellState(bool state)
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
            {
            if (hit.collider.gameObject != null)
            {
                hit.collider.gameObject.GetComponent<Cell>().SetState(state);
            }
        }
    }
}
