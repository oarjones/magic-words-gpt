using UnityEngine;

public class InputManager : MonoBehaviour
{
    public event System.Action<Cell> OnCellClicked;

    private void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0)) // 0 is the left mouse button
        {
            // Cast a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits any collider
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object has a CellView component
                CellView cellView = hit.collider.GetComponent<CellView>();
                if (cellView != null)
                {
                    // Invoke the OnCellClicked event with the cell data
                    OnCellClicked?.Invoke(cellView.cellData);
                }
            }
        }
    }
}