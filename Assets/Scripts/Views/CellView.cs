using Assets.Scripts.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    public TextMeshProUGUI letterText;
    public CellModel cellModel;
    public Sprite sprite;

    private GameObject view;

    public CellView(CellModel cellModel, GameObject cellPrefab, Transform parent)
    {
        this.cellModel = cellModel;
        letterText.text = cellModel.CurrentLetter;

        //Instanciar celda en tablero
        var ubicatePos = new Vector3(cellModel.Position.x, cellModel.Position.y, cellModel.Position.z);
        // Crear la vista
        view = GameObject.Instantiate(cellPrefab, ubicatePos, Quaternion.identity, parent);

    }

    // Método para actualizar la UI en base a los datos del modelo
    public void UpdateView()
    {
        if (cellModel != null)
        {
            letterText.text = cellModel.CurrentLetter;
        }
    }
}