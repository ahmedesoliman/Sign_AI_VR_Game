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
	public class ActionDebugPause : IAction 
	{
		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Debug.Break();
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Debug/Pause Editor";
		private const string NODE_TITLE = "Pauses the Editor";

		private const string MSG_DESCRIPTION = "Pause the Editor. To resume, press the Pause icon next to the Play";

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return NODE_TITLE;
		}

		protected override void OnEnableEditorChild ()
		{
			
		}

		protected override void OnDisableEditorChild ()
		{
			
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.HelpBox(MSG_DESCRIPTION, MessageType.Info);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}