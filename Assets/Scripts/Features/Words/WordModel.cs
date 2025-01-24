using MagicWords.Core;

namespace MagicWords.Features.Words
{
    /// <summary>
    /// Modelo que contiene la lógica y datos para el sistema de palabras.
    /// </summary>
    public class WordModel : BaseModel
    {
        private string currentWord;

        public string CurrentWord => currentWord;

        public override void Initialize()
        {
            base.Initialize();
            // Lógica inicial, carga de datos, etc.
            currentWord = "MAGIC";
        }

        public bool ValidateWord(string input)
        {
            // Ejemplo sencillo: compara la palabra ingresada con la "currentWord"
            return input.Equals(currentWord);
        }
    }
}
