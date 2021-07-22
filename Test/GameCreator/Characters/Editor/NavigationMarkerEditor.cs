namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
    using UnityEditorInternal;
    using GameCreator.Core;

	[CustomEditor(typeof(NavigationMarker))]
	public class NavigationMarkerEditor : Editor 
	{
        private const string BTN_NAME = "Marker Labels {0}";
        private const string PROP_MARKER_COLOR = "color";
        private const string PROP_MARKER_LABEL = "label";
        private const string PROP_MARKER_STOP = "stopThreshold";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spMarkerColor;
        private SerializedProperty spMarkerLabel;
        private SerializedProperty spStopThreshold;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            this.spMarkerColor = serializedObject.FindProperty(PROP_MARKER_COLOR);
            this.spMarkerLabel = serializedObject.FindProperty(PROP_MARKER_LABEL);
            this.spStopThreshold = serializedObject.FindProperty(PROP_MARKER_STOP);
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            this.PaintLabel();

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        private void PaintLabel()
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label);
            Rect rectLabel = new Rect(
                rect.x,
                rect.y,
                rect.width - 50f,
                rect.height
            );
            Rect rectButton = new Rect(
                rectLabel.x + rectLabel.width,
                rectLabel.y,
                50f,
                rectLabel.height
            );

            EditorGUI.PropertyField(rectLabel, this.spMarkerLabel);

            string buttonName = NavigationMarker.LABEL_SHOW ? "Hide" : "Show";
            if (GUI.Button(rectButton, buttonName, CoreGUIStyles.GetButtonRight()))
            {
                NavigationMarker.LABEL_SHOW = !NavigationMarker.LABEL_SHOW;
                EditorPrefs.SetBool(NavigationMarker.LABEL_KEY, NavigationMarker.LABEL_SHOW);

                this.Repaint();
                SceneView.RepaintAll();
            }

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(this.spMarkerColor);
            EditorGUILayout.PropertyField(this.spStopThreshold);

            if (EditorGUI.EndChangeCheck())
            {
                this.Repaint();
                SceneView.RepaintAll();
            }
        }

        // HIERARCHY CONTEXT MENU: ----------------------------------------------------------------

        [MenuItem("GameObject/Game Creator/Other/Marker", false, 0)]
        public static void CreateMarker()
		{
            GameObject marker = CreateSceneObject.Create("Marker");
			marker.AddComponent<NavigationMarker>();
			Selection.activeGameObject = marker;
		}
	}
}