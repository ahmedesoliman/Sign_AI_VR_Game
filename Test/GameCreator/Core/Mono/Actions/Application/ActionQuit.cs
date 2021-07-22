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
	public class ActionQuit : IAction 
	{
		// EXECUTABLE: ----------------------------------------------------------------------------
		
        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
		{
			Application.Quit ();
			return true;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Application/Quit Game";
		private const string NODE_TITLE = "Exit the game";
		private const string MSG = "Exits the game";

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return NODE_TITLE;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();
			EditorGUILayout.HelpBox (MSG, MessageType.None);
			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}