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
    public class ActionCharacterRagdoll : IAction
	{
        public enum Operation
        {
            Ragdoll,
            Recover
        }

        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);
        public Operation turnTo = Operation.Ragdoll;
        public bool autoRecover = false;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character c = this.character.GetCharacter(target);
            if (c != null)
            {
                switch (this.turnTo)
                {
                    case Operation.Ragdoll : c.SetRagdoll(true, this.autoRecover); break;
                    case Operation.Recover : c.SetRagdoll(false); break;
                }
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Character/Character Ragdoll";
        private const string NODE_TITLE = "{0} character {1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spCharacter;
        private SerializedProperty spTurnTo;
        private SerializedProperty spAutoRecover;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, this.turnTo, this.character.ToString());
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCharacter = this.serializedObject.FindProperty("character");
            this.spTurnTo = this.serializedObject.FindProperty("turnTo");
            this.spAutoRecover = this.serializedObject.FindProperty("autoRecover");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spCharacter = null;
            this.spTurnTo = null;
            this.spAutoRecover = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCharacter);
            EditorGUILayout.PropertyField(this.spTurnTo);

            if (this.spTurnTo.intValue == (int)Operation.Ragdoll)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.spAutoRecover);
                EditorGUI.indentLevel--;
            }

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
