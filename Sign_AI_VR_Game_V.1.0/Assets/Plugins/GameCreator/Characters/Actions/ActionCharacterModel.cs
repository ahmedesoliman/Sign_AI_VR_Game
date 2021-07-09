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
	public class ActionCharacterModel : IAction
	{
        public TargetCharacter character = new TargetCharacter();
        public TargetGameObject prefabModel = new TargetGameObject(TargetGameObject.Target.GameObject);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = this.character.GetCharacter(target);
            GameObject prefab = this.prefabModel.GetGameObject(target);
            if (charTarget != null && prefab != null)
            {
                CharacterAnimator targetCharAnim = charTarget.GetComponent<CharacterAnimator>();
                if (targetCharAnim.animator != null)
                {
                    targetCharAnim.ChangeModel(prefab);
                }
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

        public static new string NAME = "Character/Character Model";
        private const string NODE_TITLE = "Change character {0} model";

		// PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spPrefabModel;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, this.character.ToString());
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCharacter = this.serializedObject.FindProperty("character");
            this.spPrefabModel = this.serializedObject.FindProperty("prefabModel");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spCharacter = null;
            this.spPrefabModel = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCharacter);
            EditorGUILayout.PropertyField(this.spPrefabModel);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
