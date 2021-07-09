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
	public class ConditionInputKey : ICondition
	{
		public enum STATE 
		{
			BeingPressed,
			JustPressed,
			JustReleased
		}

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		public KeyCode key = KeyCode.Space;
		public STATE state = STATE.JustReleased;

		// EXECUTABLE: -------------------------------------------------------------------------------------------------
		
		public override bool Check()
		{
			bool result = false;
			switch (this.state)
			{
			case STATE.BeingPressed : result = Input.GetKey(this.key); break;
			case STATE.JustPressed  : result = Input.GetKeyDown(this.key); break;
			case STATE.JustReleased : result = Input.GetKeyUp(this.key); break;
			}

			return result;
		}

		// +-----------------------------------------------------------------------------------------------------------+
		// | EDITOR                                                                                                    |
		// +-----------------------------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Input/Keyboard";
		private const string NODE_TITLE = "Is {0} {1}";

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private SerializedProperty spKey;
		private SerializedProperty spState;

		// INSPECTOR METHODS: ------------------------------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			string keyName = this.key.ToString();
			string keyState = "";
			switch (this.state)
			{
			case STATE.BeingPressed : keyState = "Being Pressed"; break;
			case STATE.JustPressed  : keyState = "Just Pressed";  break;
			case STATE.JustReleased : keyState = "Just Released"; break;
			}

			return string.Format(
				NODE_TITLE, 
				keyName,
				keyState
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spKey = this.serializedObject.FindProperty("key");
			this.spState = this.serializedObject.FindProperty("state");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spKey);
			EditorGUILayout.PropertyField(this.spState);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}