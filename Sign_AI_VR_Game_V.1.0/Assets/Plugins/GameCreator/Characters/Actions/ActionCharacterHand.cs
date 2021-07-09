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
	public class ActionCharacterHand : IAction
	{
        public enum Action
        {
            Reach,
            LetGo
        }

        public TargetCharacter character = new TargetCharacter();
        public Action action = Action.Reach;

        public CharacterHandIK.Limb hand = CharacterHandIK.Limb.LeftHand;
        public TargetGameObject reachTarget = new TargetGameObject();

        [Range(0.01f, 5.0f)]
        public float duration = 0.5f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character characterTarget = this.character.GetCharacter(target);
            if (characterTarget == null) return true;

            CharacterAnimator animator = characterTarget.GetCharacterAnimator();
            if (animator == null) return true;

            CharacterHandIK handIK = animator.GetCharacterHandIK();
            if (handIK == null) return true;

            switch (this.action)
            {
                case Action.Reach:
                    handIK.Reach(
                        this.hand, 
                        this.reachTarget.GetTransform(target), 
                        this.duration
                    );
                    break;

                case Action.LetGo:
                    handIK.LetGo(this.hand, this.duration);
                    break;
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    public static new string NAME = "Character/Character Hand";
        private const string NODE_TITLE = "{0} with {1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spCharacter;
        private SerializedProperty spAction;

        private SerializedProperty spHand;
        private SerializedProperty spReachTarget;
        private SerializedProperty spDuration;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
                NODE_TITLE, 
                this.action.ToString(),
                this.hand.ToString()
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCharacter = this.serializedObject.FindProperty("character");
            this.spAction = this.serializedObject.FindProperty("action");

            this.spHand = this.serializedObject.FindProperty("hand");
            this.spReachTarget = this.serializedObject.FindProperty("reachTarget");
            this.spDuration = serializedObject.FindProperty("duration");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spCharacter = null;
            this.spAction = null;

            this.spHand = null;
            this.spReachTarget = null;
            this.spDuration = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCharacter);
            EditorGUILayout.PropertyField(this.spAction);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spHand);
            if (this.spAction.intValue == (int)Action.Reach)
            {
                EditorGUILayout.PropertyField(this.spReachTarget);
            }
            EditorGUILayout.PropertyField(this.spDuration);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
