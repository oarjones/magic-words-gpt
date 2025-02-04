// GameView.cs
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    public Text scoreTextPlayer1;
    public Text scoreTextPlayer2;
    public Text wordTextPlayer1;
    public Text wordTextPlayer2;
    public Text timerText;
    public Button validateWordButton;
    public Transform boardContainer;
    // ... references to other UI elements
    public BoardConfig boardConfig; // Referencia a BoardConfig

    public void UpdateScore(string playerId, int score)
    {
        // ... update the score text for the corresponding player
    }

    public void UpdateWord(string playerId, string word)
    {
        // ... update the word text for the corresponding player
    }

    public void UpdateTimer(float remainingTime)
    {
        //TODO: implement timer
        //timerText.text = Mathf.RoundToInt(remainingTime).ToString();
    }

    public void ShowGameEndScreen(string winnerId)
    {
        // ... show a panel with the winner or a message indicating a tie
    }

    public void InitializeBoard(Board board)
    {
        // Assuming you have a CellView component attached to your Cell prefab
        foreach (Cell cell in board.cells)
        {
            GameObject cellGO = Instantiate(boardConfig.cellPrefab, boardContainer);
            CellView cellView = cellGO.GetComponent<CellView>();
            cellView.Initialize(cell);
            // ... position the cell based on cell.x and cell.y
        }
    }
}