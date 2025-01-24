using MagicWords.Core;

namespace MagicWords.Core
{
    /// <summary>
    /// Clase base para Controladores. Orquesta la interacción entre Model y View.
    /// </summary>
    public abstract class BaseController : IController
    {
        protected IModel model;
        protected IView view;

        public virtual void SetModel(IModel model)
        {
            this.model = model;
        }

        public virtual void SetView(IView view)
        {
            this.view = view;
        }

        public virtual void Initialize()
        {
            model?.Initialize();
        }
    }
}
