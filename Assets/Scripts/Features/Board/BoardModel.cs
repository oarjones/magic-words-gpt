using System.Collections.Generic;
using MagicWords.Core;
using TMPro;
using UnityEngine;

namespace MagicWords.Features.Board
{
    public class BoardModel : BaseModel
    {
        public List<CellModel> AllCells { get; private set; } = new List<CellModel>();

        // Por si quieres exponerlo en Inspector o desde el BoardInitializer
        private int rows;
        private int cols;

        // Constructor o seteo (opcional)
        public void SetDimensions(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        
        private float _cellScaleFactorY;
        public float xOffset = 0.400f;
        public float yOffset = 0.600f;
        public float initialXpos = 0f;
        public float initialYpos = 0f;

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
        public void GenerateBoard(float mapSize, CellView hexPrefab)
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float screenratio = (screenWidth / screenHeight);
            _cellScaleFactorY = CalculateScaleFactorY(screenWidth, screenHeight);

            float yOffset1 = 0f;
            yOffset1 = yOffset * _cellScaleFactorY;

            float xPos = initialXpos;
            float yPos = initialYpos;

            var scaleX = hexPrefab.GetComponent<CellView>().transform.localScale.x;
            var scaleY = hexPrefab.GetComponent<CellView>().transform.localScale.y;

            float currentXOffset = 0.0f;
            float currentYOffset = 0.0f;

            //Modificamos escala
            if (scaleX != 0)
                currentXOffset = xOffset - (xOffset * (1f - scaleX));

            if (scaleY != 0)
                currentYOffset = yOffset1 - (yOffset1 * (1f - scaleY));


            //Por cada nivel (núermo de niveles + número de niveles vacíos)
            for (int level = 0; level < mapSize; level++)
            {
                //Es un nivel vacío
                var isEmptyLevel = level >= mapSize;


                //El nivel 0 solo contendrá una celda
                if (level == 0)
                {
                    xPos = 0;
                    yPos = 0;

                    var posVector = new Vector3(xPos, yPos, 0);
                    var cell = new CellModel(0, 0, "Hex_0_0", posVector);
                    // Lo agregamos a la lista
                    AllCells.Add(cell);
                }
                else
                {
                    //El número de celdas es el nivel por 6. Esto trazará un diseñio en forma de panel
                    var levelTilesNumber = level * 6f;

                    float yIncrement = 0.5f;
                    float xIncrement = 1f;

                    float xMultipler = 0;
                    xMultipler = level * (xMultipler + xIncrement);

                    float yMultipler = 0;
                    yMultipler = level * (yMultipler + yIncrement);

                    var negativeXtiles = 0;
                    var positiveXtiles = 0;


                    //Se instanciará cada celda comenzando por la derecaha y siguiendo la dirección contraria de las agujas del reloj
                    for (int tileNum = 0; tileNum < levelTilesNumber; tileNum++)
                    {
                        xPos = (xMultipler * currentXOffset) * 1.030f;
                        yPos = (yMultipler * currentYOffset) * 1.030f;

                        var posVector = new Vector3(xPos, yPos, 0);

                        //Marcamos la celda como actual, siempre se posivionará en el último nivel arriba -90 grados (x = 0, y = level)
                        bool isCurrentUserTile = level == (mapSize - 1) && tileNum == 0;
                        bool isCurrentOpponentTile = level == (mapSize - 1) && tileNum == ((levelTilesNumber / 3));

                        //Instanciamos celda
                        var cell = new CellModel(level, tileNum, $"Hex_{level}_{tileNum}", posVector);
                        
                        // Lo agregamos a la lista
                        AllCells.Add(cell);

                        //Calculamos los valore de xMultipler y yMultipler para calcular la posición de la próxima celda
                        if (tileNum < level)
                        {
                            xMultipler = xMultipler - xIncrement;
                            yMultipler = yMultipler + yIncrement;
                        }
                        else if (tileNum == level)
                        {
                            xMultipler = xMultipler - xIncrement;
                            yMultipler = yMultipler - yIncrement;
                        }
                        else
                        {
                            if (xMultipler < 0)
                            {
                                if (xMultipler == -(level) && negativeXtiles == 0)
                                {
                                    negativeXtiles++;
                                }

                                if (negativeXtiles == 1)
                                {
                                    yIncrement = 1;
                                }

                                if (negativeXtiles == 0)
                                {
                                    xMultipler = xMultipler - xIncrement;
                                    yMultipler = yMultipler - yIncrement;
                                }
                                else if (negativeXtiles > 0 && negativeXtiles < (level + 1))
                                {
                                    negativeXtiles++;
                                    yMultipler = yMultipler - yIncrement;
                                }
                                else
                                {
                                    yIncrement = 0.5f;
                                    xMultipler = xMultipler + xIncrement;
                                    yMultipler = yMultipler - yIncrement;
                                }
                            }
                            else
                            {
                                if (xMultipler == (level) && positiveXtiles == 0)
                                {
                                    positiveXtiles++;
                                }

                                if (positiveXtiles == 1)
                                {
                                    yIncrement = 1;
                                }

                                if (positiveXtiles == 0)
                                {
                                    xMultipler = xMultipler + xIncrement;
                                    yMultipler = yMultipler + yIncrement;
                                }
                                else if (positiveXtiles > 0 && positiveXtiles < (level + 1))
                                {
                                    positiveXtiles++;
                                    yMultipler = yMultipler + yIncrement;
                                }
                                else
                                {
                                    yIncrement = 0.5f;
                                    xMultipler = xMultipler - xIncrement;
                                    yMultipler = yMultipler + yIncrement;
                                }

                            }
                        }

                    }
                }

            }

        }

        // Ejemplo de adyacencias, tal cual legacy
        private void AssignNeighborsLegacy(CellModel cell)
        {
            int r = cell.Row;
            int c = cell.Col;
            // Array de offsets "flat-top" odd-r (legacy style)
            int[][] offsetsEven = new int[][]
            {
                new int[]{ 0, -1}, new int[]{0, +1},
                new int[]{-1, 0}, new int[]{+1, 0},
                new int[]{-1, -1}, new int[]{+1, -1}
            };
            int[][] offsetsOdd = new int[][]
            {
                new int[]{ 0, -1}, new int[]{0, +1},
                new int[]{-1, 0}, new int[]{+1, 0},
                new int[]{-1, +1}, new int[]{+1, +1}
            };

            bool isOddRow = (r % 2 == 1);
            var chosenOffsets = isOddRow ? offsetsOdd : offsetsEven;

            foreach (var off in chosenOffsets)
            {
                int nr = r + off[0];
                int nc = c + off[1];

                if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                {
                    var neighbor = AllCells.Find(x => x.Row == nr && x.Col == nc);
                    if (neighbor != null)
                    {
                        cell.AddAdjacentCell(neighbor);
                    }
                }
            }
        }
    }
}
