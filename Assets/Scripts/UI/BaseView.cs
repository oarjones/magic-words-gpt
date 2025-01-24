using UnityEngine;
using MagicWords.Core;

namespace MagicWords.UI
{
    /// <summary>
    /// Clase base para Vistas. Hereda de MonoBehaviour para manipular la UI en escena.
    /// </summary>
    public abstract class BaseView : MonoBehaviour, IView
    {
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
