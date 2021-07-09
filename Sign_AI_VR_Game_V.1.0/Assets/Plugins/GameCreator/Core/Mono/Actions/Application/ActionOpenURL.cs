namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionOpenURL : IAction 
	{
        public StringProperty link = new StringProperty("http://...");

		// EXECUTABLE: ----------------------------------------------------------------------------
		
        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
		{
            Application.OpenURL (this.link.GetValue(target));
			return true;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Application/Open URL";
		private const string NODE_TITLE = "Open URL: {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spLink;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.link);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spLink = this.serializedObject.FindProperty("link");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spLink = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spLink);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}