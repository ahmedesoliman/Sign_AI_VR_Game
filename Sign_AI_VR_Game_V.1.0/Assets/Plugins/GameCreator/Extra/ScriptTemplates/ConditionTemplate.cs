/* ##HEADER##
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
	public class __CLASS_NAME__ : ICondition
	{
		public bool satisfied = true;

		// EXECUTABLE: ----------------------------------------------------------------------------

		public override bool Check(GameObject target)
		{
			return this.satisfied;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    public static new string NAME = "Custom/__CLASS_NAME__";
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

		protected override void OnDisableEditorChild ()
		{
			this.spSatisfied = null;
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
##FOOTER## */
