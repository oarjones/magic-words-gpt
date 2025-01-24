using UnityEngine;

namespace MagicWords.Core.Services
{
    /// <summary>
    /// Servicio responsable de cargar/guardar datos (ej: JSON, ScriptableObjects, PlayerPrefs...).
    /// </summary>
    public class DataService
    {
        // Ejemplo: Cargar una lista de palabras desde un ScriptableObject
        public string[] LoadWordsFromScriptableObject()
        {
            // Pseudocódigo
            //var wordData = Resources.Load<WordListScriptable>("WordList");
            //return wordData.words;
            return new string[] { "MAGIC", "WORDS" };
        }

        // O persistir datos con PlayerPrefs:
        //public void SaveHighScore(int score) => PlayerPrefs.SetInt("HighScore", score);
        //public int LoadHighScore() => PlayerPrefs.GetInt("HighScore", 0);
    }
}
