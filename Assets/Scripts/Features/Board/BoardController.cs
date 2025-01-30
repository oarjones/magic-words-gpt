using UnityEngine;
using UnityEngine.UIElements;

namespace MagicWords.Features.Board
{
    public class BoardController
    {
        private BoardModel boardModel;
        private Transform boardParent;
        private CellView cellPrefab;
        private string localPlayerId;

        public BoardController(BoardModel model, Transform parent, CellView prefab, string localPlayerId)
        {
            boardModel = model;
            boardParent = parent;
            cellPrefab = prefab;
            this.localPlayerId = localPlayerId;
        }

        public float tileScale = 1.25f;

        float CalculateScaleFactorY(float screenWidth, float screenHeight)
        {
            float scaleFactorY = 1f; // Valor predeterminado si el alto no es superior al ancho

            // Verifica si el alto de la pantalla es superior al ancho
            if (screenHeight > screenWidth)
            {
                // Calcula el ratio dividiendo el ancho por el alto
                float ratio = screenWidth / screenHeight;

                // Pondera el factor final basado en el ratio
                // Asume que ratio = 0.45 corresponde a un factor ideal de 0.85
                // y ajusta linealmente según el ratio entre 0.1 y 0.9
                float minRatio = 0.1f;
                float maxRatio = 0.9f;
                float minFactor = 0.65f; // Factor ideal para el mínimo ratio observado
                float maxFactor = 1f; // Factor para el máximo ratio, asumiendo que queremos 1 como máximo

                // Interpolación lineal para calcular el factor basado en el ratio actual
                scaleFactorY = Mathf.Lerp(minFactor, maxFactor, (ratio - minRatio) / (maxRatio - minRatio));
            }

            return scaleFactorY;
        }

        public void GenerateBoardInSceneLegacy(float mapSize)
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float screenratio = (screenWidth / screenHeight);
            var _cellScaleFactorY = CalculateScaleFactorY(screenWidth, screenHeight);

            cellPrefab.transform.localScale = new Vector3(tileScale, tileScale * _cellScaleFactorY, 1.0f);

            // Genera los datos con la lógica original
            boardModel.GenerateBoard(mapSize, cellPrefab);

            // Instancia cada celda en la escena
            foreach (var cell in boardModel.AllCells)
            {
                var ubicatePos = new Vector3(cell.Position.x, cell.Position.y, cell.Position.z);

                // Crear la vista
                var view = GameObject.Instantiate(cellPrefab, ubicatePos, Quaternion.identity, boardParent);


                // Ajustamos su posición (UI => anchoredPosition)
                //var rect = view.GetComponent<RectTransform>();
                //if (rect != null)
                //{
                //    // cell.Position es en px, de la lógica legacy
                //    //rect.anchoredPosition = cell.Position;
                //    view.transform.localPosition = new Vector3(cell.Position.x, cell.Position.y, 0);
                //}

                // Creamos el controlador de celdas
                var controller = new CellController(cell, view, localPlayerId);
            }
        }
    }
}
