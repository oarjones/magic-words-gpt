using UnityEngine;
using MagicWords.Features.Board;

public class BoardInitializer : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private CellView cellPrefab;
    [SerializeField] private Transform boardParent; // Un objeto con RectTransform dentro del Canvas

    [Header("Board Setup")]
    [Tooltip("El valor legacy que se usaba para 'mapSize'.")]
    [SerializeField] private float mapSize = 5f;

    private BoardModel boardModel;
    private BoardController boardController;
    private string localPlayerId = "PLAYER_LOCAL";

    void Start()
    {
        // 1) Crear el modelo
        boardModel = new BoardModel();
        boardModel.Initialize();

        // 2) Crear el controlador
        boardController = new BoardController(
            boardModel, boardParent, cellPrefab, localPlayerId
        );

        // 3) Generar usando la lógica legacy
        boardController.GenerateBoardInSceneLegacy(mapSize);
    }
}
