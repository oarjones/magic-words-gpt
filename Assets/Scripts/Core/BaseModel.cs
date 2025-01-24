namespace MagicWords.Core
{
    /// <summary>
    /// Clase base para los Modelos. Contiene lógica de negocio y datos.
    /// </summary>
    public abstract class BaseModel : IModel
    {
        public virtual void Initialize()
        {
            // Lógica de inicialización común para todos los modelos
        }
    }
}
