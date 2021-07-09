namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.UI;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionChangeFontSize : IAction
	{
        public Text text;
        public NumberProperty size = new NumberProperty(16f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.text != null)
            {
                this.text.fontSize = this.size.GetInt(target);
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "UI/Change Font Size";
        private const string NODE_TITLE = "Change text font size";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spText;
        private SerializedProperty spSize;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return NODE_TITLE;
		}

		protected override void OnEnableEditorChild()
		{
            this.spText = this.serializedObject.FindProperty("text");
            this.spSize = this.serializedObject.FindProperty("size");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spText = null;
            this.spSize = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spText);
            EditorGUILayout.PropertyField(this.spSize);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
