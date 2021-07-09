namespace GameCreator.Characters
{
    using System;
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
	public class ActionCharacterStopGesture : IAction
	{
        public TargetCharacter character = new TargetCharacter();
        public float transition = 0.2f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = this.character.GetCharacter(target);
            if (charTarget != null && charTarget.GetCharacterAnimator() != null)
            {
                CharacterAnimator characterAnimator = charTarget.GetCharacterAnimator();
                characterAnimator.StopGesture(this.transition);
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Character/Character Stop Gesture";
        private const string NODE_TITLE = "Character {0} stop gesture";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spCharacter;
        private SerializedProperty spTransition;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, this.character);
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCharacter = this.serializedObject.FindProperty("character");
            this.spTransition = this.serializedObject.FindProperty("transition");
        }

		protected override void OnDisableEditorChild ()
		{
            this.spCharacter = null;
            this.spTransition = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCharacter);
            EditorGUILayout.PropertyField(this.spTransition);

            this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
