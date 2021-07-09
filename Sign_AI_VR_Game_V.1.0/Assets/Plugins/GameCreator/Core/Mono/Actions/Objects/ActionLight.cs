namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;
    using UnityEngine.Serialization;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionLight : IAction
	{
        [FormerlySerializedAs("light")]
        public Light lightTarget;

        public bool changeRange = false;
        public NumberProperty range = new NumberProperty(10f);

        public bool changeIntensity = false;
        public NumberProperty intensity = new NumberProperty(1f);

        public bool changeColor = false;
        public ColorProperty color = new ColorProperty(Color.white);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.lightTarget != null)
            {
                if (this.changeRange) this.lightTarget.range = this.range.GetValue(target);
                if (this.changeIntensity) this.lightTarget.intensity = this.intensity.GetValue(target);
                if (this.changeColor) this.lightTarget.color = this.color.GetValue(target);
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Light";
        private const string NODE_TITLE = "Change light {0}";

        private static readonly GUIContent GUICONTENT_LIGHT = new GUIContent("Light");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spLight;
        private SerializedProperty spChangeRange;
        private SerializedProperty spRange;
        private SerializedProperty spChangeIntensity;
        private SerializedProperty spIntensity;
        private SerializedProperty spChangeColor;
        private SerializedProperty spColor;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE,
                (this.lightTarget == null ? "(none)" : this.lightTarget.gameObject.name)
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spLight = this.serializedObject.FindProperty("lightTarget");
            this.spChangeRange = this.serializedObject.FindProperty("changeRange");
            this.spRange = this.serializedObject.FindProperty("range");
            this.spChangeIntensity = this.serializedObject.FindProperty("changeIntensity");
            this.spIntensity = this.serializedObject.FindProperty("intensity");
            this.spChangeColor = this.serializedObject.FindProperty("changeColor");
            this.spColor = this.serializedObject.FindProperty("color");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spLight = null;
            this.spChangeRange = null;
            this.spRange = null;
            this.spChangeIntensity = null;
            this.spIntensity = null;
            this.spChangeColor = null;
            this.spColor = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spLight, GUICONTENT_LIGHT);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.spChangeRange);
            EditorGUI.BeginDisabledGroup(!this.spChangeRange.boolValue);
            EditorGUILayout.PropertyField(this.spRange);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spChangeIntensity);
            EditorGUI.BeginDisabledGroup(!this.spChangeIntensity.boolValue);
            EditorGUILayout.PropertyField(this.spIntensity);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spChangeColor);
            EditorGUI.BeginDisabledGroup(!this.spChangeColor.boolValue);
            EditorGUILayout.PropertyField(this.spColor);
            EditorGUI.EndDisabledGroup();

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
