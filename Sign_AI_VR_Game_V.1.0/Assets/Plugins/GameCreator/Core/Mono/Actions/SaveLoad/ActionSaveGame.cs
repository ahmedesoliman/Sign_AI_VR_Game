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
	public class ActionSaveGame : IAction 
	{
		public bool useCurrentProfile = true;
		public int selectProfile = 1;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            int profile = (this.useCurrentProfile 
                ? SaveLoadManager.Instance.GetCurrentProfile() 
                : this.selectProfile
            );

            SaveLoadManager.Instance.Save(profile);

            yield return null;
            yield return 0;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Save & Load/Save Game";
		private const string NODE_TITLE = "Save Game (profile {0})";

		private static readonly GUIContent GUICONTENT_USECURR_PROFILE = new GUIContent("Use Current Profile?");
		private static readonly GUIContent GUICONTENT_SELECT_PROFILE = new GUIContent("Select Profile");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spUseCurrentProfile;
		private SerializedProperty spSelectProfile;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE,
				(this.useCurrentProfile ? "current" : this.selectProfile.ToString())
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spUseCurrentProfile = serializedObject.FindProperty("useCurrentProfile");
			this.spSelectProfile = serializedObject.FindProperty("selectProfile");
		}
		protected override void OnDisableEditorChild ()
		{
			this.spUseCurrentProfile = null;
			this.spSelectProfile = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spUseCurrentProfile, GUICONTENT_USECURR_PROFILE);
			if (!this.spUseCurrentProfile.boolValue)
			{
				EditorGUILayout.PropertyField(this.spSelectProfile, GUICONTENT_SELECT_PROFILE);
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}