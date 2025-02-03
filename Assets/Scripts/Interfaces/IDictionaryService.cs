public interface IDictionaryService
{
    bool IsValidWord(string word, string languageCode);
    void LoadDictionaries(string languageCode);
}