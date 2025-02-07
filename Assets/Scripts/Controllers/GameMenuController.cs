// En GameMenuController.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    public void StartPvAGame()
    {
        PlayerPrefs.SetString("GameMode", "PvA"); // Guarda la selección
        PlayerPrefs.Save(); // Asegura que se guarde
        SceneManager.LoadScene("GameScene"); // Carga la escena del juego
    }

    public void StartPvPGame()
    {
        PlayerPrefs.SetString("GameMode", "PvP"); // Guarda la selección
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene"); // Carga la escena del juego
    }
}