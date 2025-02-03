using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig")]
public class GameConfig : ScriptableObject
{
    public GameMode selectedGameMode;
    public int maxScore = 10;
    public float gameDuration = 300f; // 5 minutes
    // Add power-up related configurations if needed
}