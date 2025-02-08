using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

namespace Assets.Scripts.Managers
{

    public class MGLocalizationManager:MonoBehaviour
    {

        private static readonly Lazy<MGLocalizationManager> localizationManager =
            new Lazy<MGLocalizationManager>(() => new MGLocalizationManager());

        public static MGLocalizationManager Instance { get { return localizationManager.Value; } }


        private Dictionary<string, string> _localizedText;
        private string _defaultLanguage = "es-ES"; // Idioma por defecto
        



        public void LoadLanguage(string languageCode)
        {
            _localizedText = new Dictionary<string, string>();
            TextAsset textAsset = Resources.Load<TextAsset>($"Localization/{languageCode}");

            if (textAsset != null)
            {
                // ... (resto del código de LoadLanguage, sin cambios) ...
                string[] lines = textAsset.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None); //Separamos por saltos de línea
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line) && !line.StartsWith("//")) // Ignora comentarios y líneas vacías
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();
                            _localizedText[key] = value; // Añade al diccionario
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("No se encontró el archivo de localización para: " + languageCode);
            }
        }
        public string GetLocalizedText(string key)
        {
            if (_localizedText == null || !_localizedText.ContainsKey(key)) //Comprueba si está inicializado
            {
#if UNITY_EDITOR  // Solo en el editor
                LoadLanguageInEditor(_defaultLanguage); // Carga el idioma por defecto
#endif
            }
            if (_localizedText.ContainsKey(key))
            {
                return _localizedText[key];
            }
            else
            {
                Debug.LogWarning("No se encontró la clave de localización: " + key);
                return "!!!" + key + "!!!"; // Devuelve un valor por defecto (para que se note que falta)
            }
        }

#if UNITY_EDITOR //Solo compila este método en el editor
        private void LoadLanguageInEditor(string languageCode)
        {
            _localizedText = new Dictionary<string, string>();
            string path = $"Assets/Resources/Localization/{languageCode}.txt";  //Ruta completa, para el editor
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);  //Lee todas las líneas del archivo
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line) && !line.StartsWith("//"))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();
                            _localizedText[key] = value;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("No se encontró el archivo de localización para: " + languageCode);
            }
        }
#endif
    }
}
