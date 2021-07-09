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
	public class ActionDebugBeep : IAction
	{
		public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            #if UNITY_EDITOR
            EditorApplication.Beep();
            #endif

            return true;
        }

        #if UNITY_EDITOR

        public static new string NAME = "Debug/Debug Beep";
        private const string NODE_TITLE = "Beep";
        private const string INFO = "Play the default Beep sound by the operative system";

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(INFO, MessageType.Info);
        }

        #endif
    }
}
