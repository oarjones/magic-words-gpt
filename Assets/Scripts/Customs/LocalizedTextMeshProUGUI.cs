using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Localization.Editor;
using UnityEngine;

namespace Assets.Scripts.Customs
{

    public class LocalizedTextMeshProUGUI : TextMeshProUGUI
    {
        [SerializeField]
        private string localizationKey;

        protected override void Awake()
        {
            base.Awake();
            // Asigna el texto localizado al iniciar la escena
            base.m_text = MGLocalizationManager.Instance.GetLocalizedText(localizationKey);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            base.m_text = MGLocalizationManager.Instance.GetLocalizedText(localizationKey); //Localization class not shown
        }
        
    }
}
