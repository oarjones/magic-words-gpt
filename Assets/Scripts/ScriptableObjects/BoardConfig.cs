using UnityEngine;

[CreateAssetMenu(fileName = "BoardConfig", menuName = "ScriptableObjects/BoardConfig")]
public class BoardConfig : ScriptableObject
{
    public int mapSize = 7;
    public GameObject cellPrefab;
    public float cellSpacing = 1.0f;
    public Transform boardParent = null;
    
    public float xOffset = 0.400f;
    public float yOffset = 0.600f;
    public float initialXpos = 0f;
    public float initialYpos = 0f;
}