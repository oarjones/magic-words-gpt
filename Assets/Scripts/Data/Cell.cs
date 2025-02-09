using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Cell
{
    // Posici�n de la celda (por si fuera �til)
    public int Row;
    public int Col ;

    [SerializeField]
    public int Level;

    [SerializeField]
    public int TileNumber;

    [SerializeField]
    public string Name;

    [SerializeField]
    // El estado combinable (bloqueado, oculto, congelado, seleccionado, etc.)
    public GameTileState State = GameTileState.Unselected;

    [SerializeField]
    public bool isObjectiveTile = false;


    /// <summary>
    /// Identifica qui�n es el "due�o" actual de la celda.
    /// - En modo offline, puede ser "AI_Easy", "AI_Hard", etc.
    /// - En modo online, ser� el UID de Firebase.
    /// Si est� vac�o (""), no hay due�o.
    /// </summary>
    [SerializeField]
    public string OwnerId = string.Empty;


    // Letra que muestra la celda
    [SerializeField]
    public string CurrentLetter = string.Empty;

    // Control de tiempo (si la celda usa contador)
    public bool UseTimeCounter { get; protected set; }
    public float TimeCounter { get; protected set; }


    /// <summary>
    /// Celdas adyacentes
    /// </summary>
    [HideInInspector]
    [SerializeField]
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

    [SerializeField]
    public float X;

    [SerializeField]
    public float Y;

    [SerializeField]
    public Vector3 Position;

    public void SetPosition(float x, float y)
    {
        Position = new Vector3(x, y, 0);
    }
    public void Initialize()
    {
        State = GameTileState.Unselected;
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
