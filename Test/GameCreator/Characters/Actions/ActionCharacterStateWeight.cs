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
    public class ActionCharacterStateWeight : IAction
	{
        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);
        public CharacterAnimation.Layer layer = CharacterAnimation.Layer.Layer1;
        public NumberProperty weight = new NumberProperty(1.0f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = this.character.GetCharacter(target);
            if (charTarget != null && charTarget.GetCharacterAnimator() != null)
            {
                charTarget.GetCharacterAnimator().ChangeStateWeight(
                    this.layer,
                    this.weight.GetValue(target)
                );
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Character/Character State Weight";
        private const string NODE_TITLE = "Change {0} {1} state weight";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spCharacter;
        private SerializedProperty spLayer;
        private SerializedProperty spWeight;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE,
                this.character.ToString(),
                this.layer.ToString()
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCharacter = this.serializedObject.FindProperty("character");
            this.spLayer = this.serializedObject.FindProperty("layer");
            this.spWeight = this.serializedObject.FindProperty("weight");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spCharacter = null;
            this.spLayer = null;
            this.spWeight = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCharacter);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spLayer);
            EditorGUILayout.PropertyField(this.spWeight);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
