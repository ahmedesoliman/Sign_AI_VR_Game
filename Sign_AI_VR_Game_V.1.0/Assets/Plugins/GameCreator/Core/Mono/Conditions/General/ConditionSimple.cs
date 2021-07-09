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
	public class ConditionSimple : ICondition
	{
		public bool satisfied = true;

		// EXECUTABLE: ----------------------------------------------------------------------------
		
		public override bool Check()
		{
			return this.satisfied;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "General/Simple Condition";
		private const string NODE_TITLE = "Condition is always {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spSatisfied;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.satisfied);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spSatisfied = this.serializedObject.FindProperty("satisfied");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spSatisfied, new GUIContent("Is Condition Satisfied?"));

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}