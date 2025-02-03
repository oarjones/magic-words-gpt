using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    public Text playerNameText;
    public Text playerScoreText;
    // Other UI elements related to the player

    public void Initialize(Player player)
    {
        playerNameText.text = player.name;
        playerScoreText.text = player.score.ToString();
        // Initialize other UI elements
    }

    public void UpdatePlayer(Player player)
    {
        playerNameText.text = player.name;
        playerScoreText.text = player.score.ToString();
        // Update other UI elements based on player data
    }
}