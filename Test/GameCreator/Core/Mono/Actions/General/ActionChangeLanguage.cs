namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Localization;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionChangeLanguage : IAction
	{
		public SystemLanguage language;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            LocalizationManager.Instance.ChangeLanguage(this.language); 
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "General/Change Language";
		private const string NODE_TITLE = "Set Language to {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spLanguage;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.language);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spLanguage = this.serializedObject.FindProperty("language");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spLanguage = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spLanguage);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
