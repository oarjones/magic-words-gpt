using Assets.Scripts.Data;
using Assets.Scripts.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Rendering;

public class BoardGenerator : MonoBehaviour
{
    public BoardConfig boardConfig; // Asigna el ScriptableObject BoardConfig en el Inspector
    public Transform boardParent = null;



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
    public Board GenerateBoard(float mapSize, GameMode selectedGameMode)
    {

        List<CellModel> AllCells = new List<CellModel>();

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float screenratio = (screenWidth / screenHeight);
        var _cellScaleFactorY = CalculateScaleFactorY(screenWidth, screenHeight);

        float yOffset1 = 0f;
        yOffset1 = boardConfig.yOffset * _cellScaleFactorY;

        float xPos = boardConfig.initialXpos;
        float yPos = boardConfig.initialYpos;

        var scaleX = boardConfig.cellPrefab.GetComponent<CellView>().transform.localScale.x;
        var scaleY = boardConfig.cellPrefab.GetComponent<CellView>().transform.localScale.y;

        float currentXOffset = 0.0f;
        float currentYOffset = 0.0f;

        //Modificamos escala
        if (scaleX != 0)
            currentXOffset = boardConfig.xOffset - (boardConfig.xOffset * (1f - scaleX));

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
                var cell = new Cell(0, 0, "Hex_0_0", posVector);
                // Lo agregamos a la lista
                AllCells.Add(new CellModel(cell));
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
                    var cell = new Cell(level, tileNum, $"Hex_{level}_{tileNum}", posVector);

                    // Lo agregamos a la lista
                    AllCells.Add(new CellModel(cell));

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

        if (AllCells.Count > 0)
        {
            AssignTileNeighbors(AllCells);
        }

        List<CellView> cellViews = new List<CellView>(); // To associate Cell data with CellView instances
        foreach (var cell in AllCells)
        {
            CellView cellView = boardConfig.cellPrefab.GetComponent<CellView>();
            cellView.Initialize(cell, boardConfig.cellPrefab, boardParent);
            cellViews.Add(cellView);

        }

        return new Board(cellViews);

    }

    


    /// <summary>
    /// Método para asiganar las celdas vecinas de cada celda del tablero
    /// </summary>
    public void AssignTileNeighbors(List<CellModel> AllCells)
    {

        foreach (var tile in AllCells)
        {
            tile.AdjacentCells.neighbor_TOP = AllCells.Where(c => c.X == tile.X && c.Y == (tile.Y + 1f)).Any() ? AllCells.Where(c => c.X == tile.X && c.Y == (tile.Y + 1f)).First() : null;
            tile.AdjacentCells.neighbor_BOTTOM = AllCells.Where(c => c.X == tile.X && c.Y == (tile.Y - 1f)).Any() ? AllCells.Where(c => c.X == tile.X && c.Y == (tile.Y - 1f)).First() : null;

            tile.AdjacentCells.neighbor_LEFTUP = AllCells.Where(c => c.X == (tile.X - 1f) && c.Y == (tile.Y + 0.5f)).Any() ? AllCells.Where(c => c.X == (tile.X - 1f) && c.Y == (tile.Y + 0.5f)).First() : null;
            tile.AdjacentCells.neighbor_LEFTDOWN = AllCells.Where(c => c.X == (tile.X - 1f) && c.Y == (tile.Y - 0.5f)).Any() ? AllCells.Where(c => c.X == (tile.X - 1f) && c.Y == (tile.Y - 0.5f)).First() : null;

            tile.AdjacentCells.neighbor_RIGHTUP = AllCells.Where(c => c.X == (tile.X + 1f) && c.Y == (tile.Y + 0.5f)).Any() ? AllCells.Where(c => c.X == (tile.X + 1f) && c.Y == (tile.Y + 0.5f)).First() : null;
            tile.AdjacentCells.neighbor_RIGHTDOWN = AllCells.Where(c => c.X == (tile.X + 1f) && c.Y == (tile.Y - 0.5f)).Any() ? AllCells.Where(c => c.X == (tile.X + 1f) && c.Y == (tile.Y - 0.5f)).First() : null;
        }

    }

}