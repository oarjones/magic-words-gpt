using UnityEngine;

public class BoardController : MonoBehaviour
{
    private GameModel gameModel;
    private InputManager inputManager;

    public void Initialize(GameModel gameModel, InputManager inputManager)
    {
        this.gameModel = gameModel;
        this.inputManager = inputManager;

        // Subscribe to the OnCellClicked event from InputManager
        inputManager.OnCellClicked += HandleCellClicked;
    }

    private void HandleCellClicked(Cell cell)
    {
        // Check if the clicked cell is a valid move for the current player
        // Here you would need to implement the logic to determine which player clicked
        // and if they are allowed to select the cell based on game rules

        // For simplicity, let's assume any player can select any unoccupied cell
        if (!cell.isOccupied)
        {
            // Notify the GameController about the selected cell
            // This is where you would pass the selected cell to the GameController
            // For example, using an event or a direct call
            // GameController.Instance.HandleCellSelection(cell);
        }
    }
    // ...
    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (inputManager != null)
        {
            inputManager.OnCellClicked -= HandleCellClicked;
        }
    }
}