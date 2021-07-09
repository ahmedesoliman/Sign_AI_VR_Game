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
    public class ActionCharacterState : IAction
    {
        public enum StateAction
        {
            Change,
            Reset
        }

        public enum StateInput
        {
            StateAsset,
            AnimationClip
        }

        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);
        public StateAction action = StateAction.Change;
        public CharacterAnimation.Layer layer = CharacterAnimation.Layer.Layer1;
        public AvatarMask avatarMask = null;

        public StateInput state = StateInput.StateAsset;
        public CharacterState stateAsset;
        public AnimationClip stateClip;

        [Range(0f, 1f)] public float weight = 1.0f;
        public float transitionTime = 0.25f;
        public NumberProperty speed = new NumberProperty(1.0f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = this.character.GetCharacter(target);
            if (charTarget != null && charTarget.GetCharacterAnimator() != null)
            {
                if (this.action == StateAction.Reset)
                {
                    charTarget.GetCharacterAnimator().ResetState(this.transitionTime, this.layer);
                }
                else if (this.state == StateInput.StateAsset && this.stateAsset != null)
                {
                    charTarget.GetCharacterAnimator().SetState(
                        this.stateAsset,
                        this.avatarMask,
                        this.weight,
                        this.transitionTime,
                        this.speed.GetValue(target),
                        this.layer
                    );
                }
                else if (this.state == StateInput.AnimationClip && this.stateClip != null)
                {
                    charTarget.GetCharacterAnimator().SetState(
                        this.stateClip,
                        this.avatarMask,
                        this.weight,
                        this.transitionTime,
                        this.speed.GetValue(target),
                        this.layer
                    );
                }
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Character/Character State";
        private const string NODE_TITLE = "{0} {1} state in {2}";

        private static readonly GUIContent GC_MASK = new GUIContent("Mask (optional)");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spCharacter;
        private SerializedProperty spAvatarMask;
        private SerializedProperty spAction;
        private SerializedProperty spLayer;

        private SerializedProperty spState;
        private SerializedProperty spStateAsset;
        private SerializedProperty spStateClip;

        private SerializedProperty spWeight;
        private SerializedProperty spTransitionTime;
        private SerializedProperty spSpeed;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE,
                (this.action == StateAction.Reset ? "Reset" : "Change"),
                this.character.ToString(),
                this.layer.ToString()
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCharacter = this.serializedObject.FindProperty("character");
            this.spAvatarMask = this.serializedObject.FindProperty("avatarMask");
            this.spAction = this.serializedObject.FindProperty("action");
            this.spLayer = this.serializedObject.FindProperty("layer");

            this.spState = this.serializedObject.FindProperty("state");
            this.spStateAsset = this.serializedObject.FindProperty("stateAsset");
            this.spStateClip = this.serializedObject.FindProperty("stateClip");

            this.spWeight = this.serializedObject.FindProperty("weight");
            this.spTransitionTime = this.serializedObject.FindProperty("transitionTime");
            this.spSpeed = this.serializedObject.FindProperty("speed");

        }

		protected override void OnDisableEditorChild ()
		{
            this.spCharacter = null;
            this.spAvatarMask = null;
            this.spAction = null;
            this.spLayer = null;

            this.spState = null;
            this.spStateAsset = null;
            this.spStateClip = null;

            this.spWeight = null;
            this.spTransitionTime = null;
            this.spSpeed = null;
        }

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCharacter);
            EditorGUILayout.PropertyField(this.spAction);
            if (this.spAction.intValue == (int)StateAction.Change)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.spState);
                switch (this.spState.intValue)
                {
                    case (int)StateInput.StateAsset:
                        EditorGUILayout.PropertyField(this.spStateAsset);
                        break;

                    case (int)StateInput.AnimationClip:
                        EditorGUILayout.PropertyField(this.spStateClip);
                        break;
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(this.spAvatarMask, GC_MASK);
                EditorGUILayout.PropertyField(this.spWeight);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spLayer);
            EditorGUILayout.PropertyField(this.spTransitionTime);
            EditorGUILayout.PropertyField(this.spSpeed);

            this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
