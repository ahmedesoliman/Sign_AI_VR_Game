namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEditor;
    using UnityEditor.UI;

    [CustomEditor(typeof(SliderVectorVariable))]
    public class SliderVectorVariableEditor : SliderEditor
    {
        private SerializedProperty spVariable;
        private SerializedProperty spComponent;

        private new void OnEnable()
        {
            base.OnEnable();
            this.spVariable = serializedObject.FindProperty("variable");
            this.spComponent = serializedObject.FindProperty("component");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(this.spVariable);
            EditorGUILayout.PropertyField(this.spComponent);

            serializedObject.ApplyModifiedProperties();
        }

        // CREATE: --------------------------------------------------------------------------------

        [MenuItem("GameObject/Game Creator/UI/Slider Vector", false, 10)]
        public static void CreateSliderVariable()
        {
            GameObject canvas = CreateSceneObject.GetCanvasGameObject();
            GameObject sliderGO = DefaultControls.CreateSlider(CreateSceneObject.GetStandardResources());
            sliderGO.transform.SetParent(canvas.transform, false);

            Slider slider = sliderGO.GetComponent<Slider>();
            Graphic targetGraphic = slider.targetGraphic;
            RectTransform rectFill = slider.fillRect;
            RectTransform rectHandle = slider.handleRect;

            DestroyImmediate(slider);
            slider = sliderGO.AddComponent<SliderVectorVariable>();
            slider.targetGraphic = targetGraphic;
            slider.fillRect = rectFill;
            slider.handleRect = rectHandle;
            Selection.activeGameObject = sliderGO;
        }
    }
}