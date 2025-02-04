using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }
    public GameMode SelectedGameMode { get; private set; }

    private void Awake()
    {
        // Implement a simple singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object even when loading new scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetGameMode(GameMode mode)
    {
        SelectedGameMode = mode;
    }
}