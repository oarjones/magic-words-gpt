using UnityEngine;

[CreateAssetMenu(fileName = "BoardConfig", menuName = "ScriptableObjects/BoardConfig")]
public class BoardConfig : ScriptableObject
{
    public int boardWidth = 7;
    public int boardHeight = 7;
    public GameObject cellPrefab;
    public float cellSpacing = 1.0f;
}