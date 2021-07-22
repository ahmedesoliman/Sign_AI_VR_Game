namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionDebugBreak : IAction
	{
		public bool isEnabled = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (Application.isEditor && this.isEnabled) Debug.Break();
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Debug/Debug Break";
		private const string NODE_TITLE = "Debug.Break ({0})";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spIsEnabled;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.isEnabled ? "On" : "Off");
		}

		protected override void OnEnableEditorChild ()
		{
			this.spIsEnabled = this.serializedObject.FindProperty("isEnabled");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spIsEnabled = null;
		}

		public override void OnInspectorGUI()
		{
            if (this.serializedObject == null) return;

			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spIsEnabled);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
