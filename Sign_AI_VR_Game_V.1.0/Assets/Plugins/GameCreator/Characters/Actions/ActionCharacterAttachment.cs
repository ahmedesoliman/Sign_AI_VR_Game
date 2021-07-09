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
	public class ActionCharacterAttachment : IAction
	{
        public enum Action
        {
            Attach,
            Detach,
            Remove
        }

        public TargetCharacter character = new TargetCharacter();
        public Action action = Action.Attach;

        public HumanBodyBones bone = HumanBodyBones.RightHand;
        public TargetGameObject instance = new TargetGameObject();
        public Space space = Space.Self;
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character characterTarget = this.character.GetCharacter(target);
            if (characterTarget == null) return true;

            CharacterAnimator animator = characterTarget.GetCharacterAnimator();
            if (animator == null) return true;

            CharacterAttachments attachments = animator.GetCharacterAttachments();
            if (attachments == null) return true;

            switch (this.action)
            {
                case Action.Attach:
                    attachments.Attach(
                        this.bone,
                        this.instance.GetGameObject(target),
                        this.position,
                        Quaternion.Euler(this.rotation),
                        this.space
                    ); break;

                case Action.Detach:
                    attachments.Detach(this.bone);
                    break;

                case Action.Remove:
                    attachments.Remove(this.bone);
                    break;
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    public static new string NAME = "Character/Character Attachment";
        private const string NODE_TITLE = "{0} from {1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spCharacter;
        private SerializedProperty spAction;

        private SerializedProperty spBone;
        private SerializedProperty spInstance;
        private SerializedProperty spSpace;
        private SerializedProperty spPosition;
        private SerializedProperty spRotation;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
                NODE_TITLE, 
                this.action.ToString(),
                this.bone.ToString()
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCharacter = this.serializedObject.FindProperty("character");
            this.spAction = this.serializedObject.FindProperty("action");

            this.spBone = this.serializedObject.FindProperty("bone");
            this.spInstance = this.serializedObject.FindProperty("instance");
            this.spSpace = serializedObject.FindProperty("space");
            this.spPosition = this.serializedObject.FindProperty("position");
            this.spRotation = this.serializedObject.FindProperty("rotation");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spCharacter = null;
            this.spAction = null;

            this.spBone = null;
            this.spInstance = null;
            this.spSpace = null;
            this.spPosition = null;
            this.spRotation = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCharacter);
            EditorGUILayout.PropertyField(this.spAction);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spBone);
            if (this.spAction.intValue == (int)Action.Attach)
            {
                EditorGUILayout.PropertyField(this.spInstance);
                EditorGUILayout.PropertyField(this.spSpace);
                EditorGUILayout.PropertyField(this.spPosition);
                EditorGUILayout.PropertyField(this.spRotation);
            }

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
