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
	public class ActionLoadGame : IAction 
	{
		public bool useCurrentProfile = true;
		public int selectProfile = 1;

        private bool complete = false;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            int profile = (this.useCurrentProfile 
                ? SaveLoadManager.Instance.GetCurrentProfile() 
                : this.selectProfile
            );

            this.complete = false;
            SaveLoadManager.Instance.Load(profile, this.OnLoad);

            WaitUntil waitUntil = new WaitUntil(() => this.complete);
            yield return waitUntil;

            yield return 0;
        }

        private void OnLoad()
        {
            this.complete = true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Save & Load/Load Game";
		private const string NODE_TITLE = "Load Game (profile {0})";

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