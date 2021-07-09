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
	public class ActionMethods : IAction 
	{
		public UnityEvent events = new UnityEvent();

		// EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.events != null) this.events.Invoke();
            return true;
        }

		// +-----------------------------------------------------------------------------------------------------------+
		// | EDITOR                                                                                                    |
		// +-----------------------------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		public static new string NAME = "General/Call Methods";
		private const string NODE_TITLE = "Call {0} method{1}";

		private static readonly GUIContent GUICONTENT_EVENTS = new GUIContent("Call methods");
		private SerializedProperty spEvents;

		// INSPECTOR METHODS: ------------------------------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			int methodsCount = 0;
			string methodsSuffix = "";

			if (this.events != null)
			{
				methodsCount = this.events.GetPersistentEventCount();
				methodsSuffix = (methodsCount == 1 ? "" : "s");
			}

			return string.Format(ActionMethods.NODE_TITLE, methodsCount, methodsSuffix);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spEvents = this.serializedObject.FindProperty("events");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spEvents = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spEvents, GUICONTENT_EVENTS);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}