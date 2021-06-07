using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellToggler : MonoBehaviour
{
    Ray ray;
    RaycastHit2D hit;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) ToggleCellState();
    }

    void ToggleCellState()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit.collider.gameObject != null)
        {
            hit.collider.gameObject.GetComponent<Cell>().ToggleState();
        }
    }
}
