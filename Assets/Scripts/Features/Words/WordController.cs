using MagicWords.Core;

namespace MagicWords.Features.Words
{
    /// <summary>
    /// Controlador para el sistema de palabras. Coordina la interacción entre WordModel y WordView.
    /// </summary>
    public class WordController : BaseController
    {
        // Hacemos cast para no repetirlo a cada rato
        private WordModel wordModel => (WordModel)model;
        private WordView wordView => (WordView)view;

        public WordController(WordModel model, WordView view)
        {
            SetModel(model);
            SetView(view);

            // Si necesitamos llamar a Initialize() manualmente
            Initialize();

            // Asignar este controlador a la vista
            wordView.SetController(this);
            wordView.UpdateWordLabel(wordModel.CurrentWord);
        }

        public void OnValidateButtonPressed(string input)
        {
            bool isValid = wordModel.ValidateWord(input);
            // Aquí decides qué hacer: mostrar feedback en la vista, disparar un evento, etc.

            // Ejemplo: actualizar el texto de la vista
            if (isValid)
            {
                wordView.UpdateWordLabel("¡Correcto!");
            }
            else
            {
                wordView.UpdateWordLabel("Incorrecto");
            }
        }
    }
}
