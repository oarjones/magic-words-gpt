namespace MagicWords.Core
{
    /// <summary>
    /// Define la interfaz para cualquier "Vista" en la arquitectura MVC.
    /// </summary>
    public interface IView
    {
        // Métodos o propiedades que deben implementar todas las vistas (UI)
        void Show();
        void Hide();
    }
}
