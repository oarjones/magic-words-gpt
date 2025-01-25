using UnityEngine;
using MagicWords.Features.Words; // donde est�n tus clases WordModel, WordController, WordView

public class MainSceneInitializer : MonoBehaviour
{
    [SerializeField] private WordView wordView;  // Referencia a la vista en escena
    private WordController wordController;

    void Start()
    {
        // 1. Crear una instancia del modelo
        var model = new WordModel();

        // 2. Crear el controlador pas�ndole el modelo y la vista
        wordController = new WordController(model, wordView);

        // 3. Opcional: Llamar a m�todos extra de configuraci�n
        // wordController.InicializarDatos(); ...
    }
}
