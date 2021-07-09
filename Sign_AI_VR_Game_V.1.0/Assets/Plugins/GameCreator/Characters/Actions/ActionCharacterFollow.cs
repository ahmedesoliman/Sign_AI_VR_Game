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
	public class ActionCharacterFollow : IAction 
	{
        public enum ActionType
        {
            Follow,
            StopFollow
        }

        public TargetCharacter character = new TargetCharacter();
        public ActionType actionType = ActionType.Follow;

        public TargetGameObject followTarget = new TargetGameObject();
        public float followMinRadius = 2.0f;
        public float followMaxRadius = 4.0f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = this.character.GetCharacter(target);
            if (charTarget == null) return true;

            Transform follow = null;

            if (this.actionType == ActionType.Follow)
            {
                follow = this.followTarget.GetComponent<Transform>(target);
            }

            charTarget.characterLocomotion.FollowTarget(
                follow,
                this.followMinRadius,
                this.followMaxRadius
            );

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Character/Character Follow";
        private const string NODE_TITLE = "{0} {1} {2}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spCharacter;
		private SerializedProperty spActionType;
		
        private SerializedProperty spFollowTarget;
		private SerializedProperty spMinRadius;
		private SerializedProperty spMaxRadius;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE,
                this.character,
                ObjectNames.NicifyVariableName(this.actionType.ToString()),
                (this.actionType == ActionType.Follow ? this.followTarget.ToString() : "")
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCharacter = this.serializedObject.FindProperty("character");
            this.spActionType = this.serializedObject.FindProperty("actionType");
            this.spFollowTarget = this.serializedObject.FindProperty("followTarget");
            this.spMinRadius = this.serializedObject.FindProperty("followMinRadius");
            this.spMaxRadius = this.serializedObject.FindProperty("followMaxRadius");
		}

		protected override void OnDisableEditorChild ()
		{
			return;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCharacter);
            EditorGUILayout.PropertyField(this.spActionType);

            if ((ActionType)this.spActionType.intValue == ActionType.Follow)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(this.spFollowTarget);

                EditorGUILayout.PropertyField(this.spMinRadius);  
                EditorGUILayout.PropertyField(this.spMaxRadius);  
            }

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}