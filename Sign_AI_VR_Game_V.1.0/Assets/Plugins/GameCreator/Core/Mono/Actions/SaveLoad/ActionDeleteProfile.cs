namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionDeleteProfile : IAction 
	{
		public int profile = 0;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            SaveLoadManager.Instance.DeleteProfile(profile);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Save & Load/Delete Profile";
		private const string NODE_TITLE = "Delete profile {0}";

		private static readonly GUIContent GUICONTENT_PROFILE = new GUIContent("Profile");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spProfile;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.profile.ToString());
		}

		protected override void OnEnableEditorChild ()
		{
			this.spProfile = this.serializedObject.FindProperty("profile");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spProfile = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spProfile, GUICONTENT_PROFILE);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}