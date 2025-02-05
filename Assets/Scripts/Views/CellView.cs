using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    public TextMeshProUGUI letterText;

    public Cell cellData;

    public void Initialize(Cell cell)
    {
        cellData = cell;
        letterText.text = cell.CurrentLetter;
        // ... other initializations
    }

    // Method to handle cell click
    public void OnCellClicked()
    {
        // Send an event or call a method in the BoardController
        // For example, using a UnityEvent:
        // cellClickedEvent.Invoke(cellData);
    }
}