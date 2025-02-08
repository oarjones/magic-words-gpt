// LocalizedTextEditor.cs (debe estar en una carpeta "Editor")
using UnityEngine;
using UnityEditor;
using TMPro;
using Assets.Scripts.Customs;
using TMPro.EditorUtilities;

[CustomEditor(typeof(LocalizedTextMeshProUGUI))]
public class LocalizedTextInspector : TMP_EditorPanelUI
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Dibuja el campo localizationKey editable
        SerializedProperty keyProperty = serializedObject.FindProperty("localizationKey");
        EditorGUILayout.PropertyField(keyProperty);

        // (Opcional) Mostrar el texto localizado en modo lectura
        SerializedProperty textProperty = serializedObject.FindProperty("m_text");
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(textProperty);
        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();

        // Si hubiera más elementos en el inspector de TMP, puedes llamarlos.
        base.OnInspectorGUI();
    }
}
