namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
	using GameCreator.Core.Hooks;
	using GameCreator.Characters;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionCharacterJump : IAction
	{
        public TargetCharacter target = new TargetCharacter();

		public bool overrideJumpForce = false;
        public NumberProperty jumpForce = new NumberProperty(10f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = this.target.GetCharacter(target);
            if (charTarget != null)
            {
                if (this.overrideJumpForce) charTarget.Jump(this.jumpForce.GetValue(target));
                else charTarget.Jump();
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Character/Character Jump";
		private const string NODE_TITLE = "Jump character {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;
		private SerializedProperty spOverrideJumpForce;
		private SerializedProperty spJumpForce;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, this.target);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = this.serializedObject.FindProperty("target");
			this.spOverrideJumpForce = this.serializedObject.FindProperty("overrideJumpForce");
			this.spJumpForce = this.serializedObject.FindProperty("jumpForce");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spTarget = null;
			this.spOverrideJumpForce = null;
			this.spJumpForce = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spTarget);
			EditorGUILayout.PropertyField(this.spOverrideJumpForce);
			
            EditorGUI.BeginDisabledGroup(!this.spOverrideJumpForce.boolValue);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spJumpForce);
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
