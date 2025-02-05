using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    // Posici�n de la celda (por si fuera �til)
    public int Row { get; protected set; }
    public int Col { get; protected set; }


    public int Level { get; protected set; }
    public int TileNumber { get; protected set; }
    public string Name { get; protected set; }

    // El estado combinable (bloqueado, oculto, congelado, seleccionado, etc.)
    public CellState State { get; protected set; } = CellState.None;

    /// <summary>
    /// Identifica qui�n es el "due�o" actual de la celda.
    /// - En modo offline, puede ser "AI_Easy", "AI_Hard", etc.
    /// - En modo online, ser� el UID de Firebase.
    /// Si est� vac�o (""), no hay due�o.
    /// </summary>
    public string OwnerId { get; protected set; } = string.Empty;

    // Letra que muestra la celda
    public string CurrentLetter { get; protected set; } = string.Empty;

    // Control de tiempo (si la celda usa contador)
    public bool UseTimeCounter { get; protected set; }
    public float TimeCounter { get; protected set; }


    /// <summary>
    /// Celdas adyacentes
    /// </summary>
    [HideInInspector]
    public AdjacentCell AdjacentCells;

    public Cell(int level, int tileNumber, string name, Vector3 position)
    {
        Level = level;
        TileNumber = tileNumber;
        Name = name;
        Position = position;
        X = position.x;
        Y = position.y;
    }

    public float X { get; protected set; }
    public float Y { get; protected set; }
    public Vector3 Position { get; protected set; }

    public void SetPosition(float x, float y)
    {
        Position = new Vector3(x, y, 0);
    }
    public void Initialize()
    {
        State = CellState.None;
        OwnerId = string.Empty;
        CurrentLetter = string.Empty;
        UseTimeCounter = false;
        TimeCounter = 0f;
        ClearAdjacentCells();
    }

    private void ClearAdjacentCells()
    {
        AdjacentCells.Clear();
    }

    


}
