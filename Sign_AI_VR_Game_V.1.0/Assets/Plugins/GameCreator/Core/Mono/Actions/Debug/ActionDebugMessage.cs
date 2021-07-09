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
	public class ActionDebugMessage : IAction 
	{
		public enum LogType
		{
			NORMAL,
			WARNING,
			ERROR
		}

		public LogType logType = LogType.NORMAL;
		public string message = "debug message";

		// EXECUTABLE: ----------------------------------------------------------------------------
		
        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            switch (this.logType)
            {
                case LogType.NORMAL: Debug.Log(this.message, gameObject); break;
                case LogType.WARNING: Debug.LogWarning(this.message, gameObject); break;
                case LogType.ERROR: Debug.LogError(this.message, gameObject); break;
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Debug/Debug Message";
		private const string NODE_TITLE = "Debug.{0}: {1}";

		private static readonly GUIContent GUICONTENT_LOGTYPE = new GUIContent("Message Type");
		private static readonly GUIContent GUICONTENT_MESSAGE = new GUIContent("Message");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spLogType;
		private SerializedProperty spMessage;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			string type = "Log";
			if (this.logType == LogType.WARNING) type = "Warning";
			if (this.logType == LogType.ERROR) type = "Error";

			return string.Format(NODE_TITLE, type, this.message);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spLogType = this.serializedObject.FindProperty("logType");
			this.spMessage = this.serializedObject.FindProperty("message");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spLogType = null;
			this.spMessage = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spLogType, GUICONTENT_LOGTYPE);
			EditorGUILayout.PropertyField(this.spMessage, GUICONTENT_MESSAGE);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}