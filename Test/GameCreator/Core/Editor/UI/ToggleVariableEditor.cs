namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEditor;
    using UnityEditor.UI;

    [CustomEditor(typeof(ToggleVariable))]
    public class ToggleVariableEditor : ToggleEditor
    {
        private SerializedProperty spVariable;

        private new void OnEnable()
        {
            base.OnEnable();
            this.spVariable = serializedObject.FindProperty("variable");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(this.spVariable);
            serializedObject.ApplyModifiedProperties();
        }

        // CREATE: --------------------------------------------------------------------------------

        [MenuItem("GameObject/Game Creator/UI/Toggle", false, 10)]
        public static void CreateToggleVariable()
        {
            GameObject canvas = CreateSceneObject.GetCanvasGameObject();
            GameObject toggleGO = DefaultControls.CreateToggle(CreateSceneObject.GetStandardResources());
            toggleGO.transform.SetParent(canvas.transform, false);

            Toggle toggle = toggleGO.GetComponent<Toggle>();
            Graphic targetGraphic = toggle.targetGraphic;
            Graphic graphic = toggle.graphic;

            DestroyImmediate(toggle);
            toggle = toggleGO.AddComponent<ToggleVariable>();
            toggle.targetGraphic = targetGraphic;
            toggle.graphic = graphic;
            Selection.activeGameObject = toggleGO;
        }
    }
}