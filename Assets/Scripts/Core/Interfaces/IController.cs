namespace MagicWords.Core
{
    /// <summary>
    /// Define la interfaz para cualquier "Controlador" en la arquitectura MVC.
    /// </summary>
    public interface IController
    {
        // Métodos o propiedades que deben implementar todos los controladores
        void SetModel(IModel model);
        void SetView(IView view);
        void Initialize();
    }
}
