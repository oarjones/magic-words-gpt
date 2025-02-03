using UnityEngine;

[CreateAssetMenu(fileName = "DictionaryConfig", menuName = "ScriptableObjects/DictionaryConfig")]
public class DictionaryConfig : ScriptableObject
{
    public string defaultLanguage = "es-ES";
    public string[] availableLanguages = { "es-ES", "en-EN" };
    // ... other dictionary related configurations
}