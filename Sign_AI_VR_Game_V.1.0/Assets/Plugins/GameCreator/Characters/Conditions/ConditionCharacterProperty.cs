namespace GameCreator.Characters
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
	public class ConditionCharacterProperty : ICondition
	{
        public enum CharacterProperty
        {
            IsControllable,
            IsIdle,
            IsWalking,
            IsRunning,
            IsGrounded,
            IsOnAir,
            CanRun,
            CanJump
        }

        public TargetCharacter target;
        public CharacterProperty property = CharacterProperty.IsIdle;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check(GameObject target)
		{
            Character character = this.target.GetCharacter(target);
            if (character == null) return true;
            bool result = true;

            switch (this.property)
            {
                case CharacterProperty.IsControllable:
                    result = character.IsControllable();
                    break;

                case CharacterProperty.IsIdle:
                    result = character.GetCharacterMotion() == 0;
                    break;

                case CharacterProperty.IsWalking:
                    result = character.GetCharacterMotion() == 1;
                    break;

                case CharacterProperty.IsRunning:
                    result = character.GetCharacterMotion() == 2;
                    break;

                case CharacterProperty.IsGrounded:
                    result = character.IsGrounded();
                    break;

                case CharacterProperty.IsOnAir:
                    result = !character.IsGrounded();
                    break;

                case CharacterProperty.CanRun:
                    if (character.characterLocomotion != null)
                        result = character.characterLocomotion.canRun;
                    break;

                case CharacterProperty.CanJump:
                    if (character.characterLocomotion != null)
                        result = character.characterLocomotion.canJump;
                    break;
            }

            return result;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    //public const string CUSTOM_ICON_PATH = "Assets/[Custom Path To Icon]";

		public static new string NAME = "Characters/Character Property";
        private const string NODE_TITLE = "Character {0} {1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;
        private SerializedProperty spProperty;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
                NODE_TITLE, 
                (this.target == null ? "(undefined)" : this.target.ToString()),
                this.property.ToString()
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spTarget = this.serializedObject.FindProperty("target");
            this.spProperty = this.serializedObject.FindProperty("property");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spTarget);
            EditorGUILayout.PropertyField(this.spProperty);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
