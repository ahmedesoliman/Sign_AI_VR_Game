namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionCharacterProperies : IAction 
	{
		public enum CHANGE_PROPERTY
		{
			IsControllable,
			CanRun,
			SetRunSpeed,
            Height,
            JumpForce,
            JumpTimes,
            Gravity,
            MaxFallSpeed,
            CanJump
		}

        public TargetCharacter target = new TargetCharacter(TargetCharacter.Target.Player);

		public CHANGE_PROPERTY changeProperty = CHANGE_PROPERTY.IsControllable;

        public BoolProperty valueBool = new BoolProperty(true);
        public NumberProperty valueNumber = new NumberProperty(5.0f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = this.target.GetCharacter(target);
            if (charTarget != null)
            {
                switch (this.changeProperty)
                {
                    case CHANGE_PROPERTY.IsControllable:
                        bool isControllable = this.valueBool.GetValue(target);
                        charTarget.characterLocomotion.SetIsControllable(isControllable);
                        break;

                    case CHANGE_PROPERTY.CanRun:
                        charTarget.characterLocomotion.canRun = this.valueBool.GetValue(target);
                        break;

                    case CHANGE_PROPERTY.SetRunSpeed:
                        charTarget.characterLocomotion.runSpeed = this.valueNumber.GetValue(target);
                        break;

                    case CHANGE_PROPERTY.Height:
                        charTarget.characterLocomotion.ChangeHeight(this.valueNumber.GetValue(target));
                        break;

                    case CHANGE_PROPERTY.JumpForce:
                        charTarget.characterLocomotion.jumpForce = this.valueNumber.GetValue(target);
                        break;

                    case CHANGE_PROPERTY.JumpTimes:
                        charTarget.characterLocomotion.jumpTimes = this.valueNumber.GetInt(target);
                        break;

                    case CHANGE_PROPERTY.Gravity:
                        charTarget.characterLocomotion.gravity = this.valueNumber.GetValue(target);
                        break;

                    case CHANGE_PROPERTY.MaxFallSpeed:
                        charTarget.characterLocomotion.maxFallSpeed = this.valueNumber.GetValue(target);
                        break;

                    case CHANGE_PROPERTY.CanJump:
                        charTarget.characterLocomotion.canJump = this.valueBool.GetValue(target);
                        break;
                }
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Character/Change Property";
		private const string NODE_TITLE = "Change {0} {1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;
		private SerializedProperty spChangeProperty;
		private SerializedProperty spValueBool;
		private SerializedProperty spValueNumber;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE,
                this.target,
				this.changeProperty
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = this.serializedObject.FindProperty("target");
			this.spChangeProperty = this.serializedObject.FindProperty("changeProperty");
            this.spValueBool = this.serializedObject.FindProperty("valueBool");
            this.spValueNumber = this.serializedObject.FindProperty("valueNumber");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spTarget = null;
            this.spChangeProperty = null;
            this.spValueBool = null;
            this.spValueNumber = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spTarget);
			EditorGUILayout.PropertyField(this.spChangeProperty);

			EditorGUILayout.Space();
            switch ((CHANGE_PROPERTY)this.spChangeProperty.intValue)
            {
                case CHANGE_PROPERTY.IsControllable: EditorGUILayout.PropertyField(this.spValueBool); break;
                case CHANGE_PROPERTY.CanRun: EditorGUILayout.PropertyField(this.spValueBool); break;
                case CHANGE_PROPERTY.SetRunSpeed: EditorGUILayout.PropertyField(this.spValueNumber); break;
                case CHANGE_PROPERTY.Height: EditorGUILayout.PropertyField(this.spValueNumber); break;
                case CHANGE_PROPERTY.JumpForce: EditorGUILayout.PropertyField(this.spValueNumber); break;
                case CHANGE_PROPERTY.JumpTimes: EditorGUILayout.PropertyField(this.spValueNumber); break;
                case CHANGE_PROPERTY.Gravity: EditorGUILayout.PropertyField(this.spValueNumber); break;
                case CHANGE_PROPERTY.MaxFallSpeed: EditorGUILayout.PropertyField(this.spValueNumber); break;
                case CHANGE_PROPERTY.CanJump: EditorGUILayout.PropertyField(this.spValueBool); break;
            }

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}