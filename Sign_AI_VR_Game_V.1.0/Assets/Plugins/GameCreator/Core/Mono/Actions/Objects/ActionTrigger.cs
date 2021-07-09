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
    public class ActionTrigger : IAction 
	{
        public Trigger trigger;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.trigger != null) this.trigger.Execute(target);
            return true;
        }

		// +-----------------------------------------------------------------------------------------------------------+
		// | EDITOR                                                                                                    |
		// +-----------------------------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Trigger";
		private const string NODE_TITLE = "Trigger {0}";

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private SerializedProperty spTrigger;

		// INSPECTOR METHODS: ------------------------------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE, 
                (this.trigger == null ? "none" : this.trigger.gameObject.name)
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spTrigger = this.serializedObject.FindProperty("trigger");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spTrigger = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.spTrigger);
			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}