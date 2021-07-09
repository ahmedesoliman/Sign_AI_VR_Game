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
	public class ActionCharacterDirection : IAction
	{
        public TargetCharacter character = new TargetCharacter();

        public CharacterLocomotion.FACE_DIRECTION direction;
        public TargetPosition directionTarget = new TargetPosition(TargetPosition.Target.Transform);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character c = this.character.GetCharacter(target);

            if (c != null)
            {
                c.characterLocomotion.faceDirection = this.direction;
                if (this.direction == CharacterLocomotion.FACE_DIRECTION.Target)
                {
                    TargetPosition dirTarget = this.directionTarget;
                    if (dirTarget.target == TargetPosition.Target.Invoker)
                    {
                        dirTarget.target = TargetPosition.Target.Transform;
                        dirTarget.targetTransform = target.transform;
                    }

                    c.characterLocomotion.faceDirectionTarget = dirTarget;
                }
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Character/Character Direction";
        private const string NODE_TITLE = "Change {0} direction to {1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spCharacter;
        private SerializedProperty spDirection;
        private SerializedProperty spDirectionTarget;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE, 
                this.character.ToString(), 
                this.direction.ToString()
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCharacter = this.serializedObject.FindProperty("character");
            this.spDirection = this.serializedObject.FindProperty("direction");
            this.spDirectionTarget = this.serializedObject.FindProperty("directionTarget");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spCharacter = null;
            this.spDirection = null;
            this.spDirectionTarget = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCharacter);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spDirection);
            if (this.spDirection.intValue == (int)CharacterLocomotion.FACE_DIRECTION.Target)
            {
                EditorGUILayout.PropertyField(this.spDirectionTarget);
            }

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
