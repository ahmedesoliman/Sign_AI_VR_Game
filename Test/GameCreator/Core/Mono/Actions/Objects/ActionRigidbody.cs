namespace GameCreator.Core
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
	public class ActionRigidbody : IAction
	{
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.GameObject);

        public bool changeMass = false;
        public bool changeUseGravity = false;
        public bool changeIsKinematic = false;

        public NumberProperty mass = new NumberProperty(1f);
        public BoolProperty useGravity = new BoolProperty(true);
        public BoolProperty isKinematic = new BoolProperty(true);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject targetGO = this.target.GetGameObject(target);
            if (targetGO == null) return true;

            Rigidbody targetRB = targetGO.GetComponent<Rigidbody>();
            if (targetRB != null)
            {
                targetRB.mass = this.mass.GetValue(target);
                targetRB.useGravity = this.useGravity.GetValue(target);
                targetRB.isKinematic = this.isKinematic.GetValue(target);
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Rigidbody";
        private const string NODE_TITLE = "Change {0} properties";

        private static readonly GUIContent GUICONTENT_EMPTY = new GUIContent(" ");

        // PROPERTIES: ----------------------------------------------------------------------------

        public SerializedProperty spTarget;
        public SerializedProperty spChangeMass;
        public SerializedProperty spChangeUseGravity;
        public SerializedProperty spChangeIsKinematic;
        public SerializedProperty spMass;
        public SerializedProperty spUseGravity;
        public SerializedProperty spIsKinematic;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, this.target);
		}

		protected override void OnEnableEditorChild ()
		{
            this.spTarget = this.serializedObject.FindProperty("target");
            this.spChangeMass = this.serializedObject.FindProperty("changeMass");
            this.spChangeUseGravity = this.serializedObject.FindProperty("changeUseGravity");
            this.spChangeIsKinematic = this.serializedObject.FindProperty("changeIsKinematic");

            this.spMass = this.serializedObject.FindProperty("mass");
            this.spUseGravity = this.serializedObject.FindProperty("useGravity");
            this.spIsKinematic = this.serializedObject.FindProperty("isKinematic");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spTarget = null;
            this.spChangeMass = null;
            this.spChangeUseGravity = null;
            this.spChangeIsKinematic = null;
            this.spMass = null;
            this.spUseGravity = null;
            this.spIsKinematic = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spTarget);
            EditorGUILayout.Space();

            this.PaintSection(this.spChangeMass, this.spMass);
            EditorGUILayout.Space();
            this.PaintSection(this.spChangeUseGravity, this.spUseGravity);
            EditorGUILayout.Space();
            this.PaintSection(this.spChangeIsKinematic, this.spIsKinematic);

			this.serializedObject.ApplyModifiedProperties();
		}

        private void PaintSection(SerializedProperty spChange, SerializedProperty spProperty)
        {
            EditorGUILayout.PropertyField(spChange, new GUIContent(spProperty.displayName));
            EditorGUI.BeginDisabledGroup(!spChange.boolValue);
            EditorGUILayout.PropertyField(spProperty, GUICONTENT_EMPTY);
            EditorGUI.EndDisabledGroup();
        }

		#endif
	}
}
