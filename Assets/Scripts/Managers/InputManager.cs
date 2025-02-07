using Assets.Scripts.Models;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public event System.Action<CellModel> OnCellClicked;
    //Ya no necesitamos el LayerMask en 2D.
    //public LayerMask cellLayerMask; // Ya no es necesario en 2D

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Convierte la posición del mouse a coordenadas del mundo (2D)
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Usa Physics2D.Raycast
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero); // Rayo desde la posición del mouse en dirección "cero"

            if (hit.collider != null)
            {
                CellView cellView = hit.collider.GetComponent<CellView>();
                if (cellView != null)
                {
                    OnCellClicked?.Invoke(cellView.cellModel);
                }
            }
        }
    }
}