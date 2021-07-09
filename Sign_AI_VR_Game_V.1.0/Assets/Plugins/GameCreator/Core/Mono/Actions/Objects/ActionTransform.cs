namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core.Hooks;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionTransform : IAction 
	{
		public enum RELATIVE
		{
			Local,
			Global
		}

		public enum PARENT
		{
			DontChange,
			ChangeParent,
			ClearParent
		}

        // PROPERTIES: ----------------------------------------------------------------------------

        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.Player);

		public PARENT changeParent = PARENT.DontChange;
        public TargetGameObject newParent = new TargetGameObject(TargetGameObject.Target.GameObject);

		public bool changePosition = false;
		public RELATIVE positionRelativity = RELATIVE.Global;
        public TargetPosition position = new TargetPosition();

		public bool changeRotation = false;
		public RELATIVE rotationRelativity = RELATIVE.Global;
		public Vector3 rotation = Vector3.zero;

		public bool changeScale = false;
		public Vector3 scale = Vector3.one;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Transform targetTrans = this.target.GetTransform(target);
            if (targetTrans != null)
            {
                switch (this.changeParent)
                {
                    case PARENT.ChangeParent:
                        Transform newParentTransform = this.newParent.GetTransform(target);
                        if (newParentTransform != null) targetTrans.SetParent(newParentTransform);
                        break;

                    case PARENT.ClearParent:
                        targetTrans.SetParent(null);
                        break;
                }

                if (this.changePosition)
                {
                    switch (this.positionRelativity)
                    {
                        case RELATIVE.Local: targetTrans.localPosition = this.position.GetPosition(target); break;
                        case RELATIVE.Global: targetTrans.position = this.position.GetPosition(target); break;
                    }
                }

                if (this.changeRotation)
                {
                    switch (this.rotationRelativity)
                    {
                        case RELATIVE.Local: targetTrans.localRotation = Quaternion.Euler(this.rotation); break;
                        case RELATIVE.Global: targetTrans.rotation = Quaternion.Euler(this.rotation); break;
                    }
                }

                if (this.changeScale)
                {
                    targetTrans.localScale = this.scale;
                }
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Transform";
		private const string NODE_TITLE = "Change {0} transform properties";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;

		private SerializedProperty spChangeParent;
		private SerializedProperty spNewParent;

		private SerializedProperty spChangePosition;
		private SerializedProperty spPositionRelativity;
		private SerializedProperty spPosition;

		private SerializedProperty spChangeRotation;
		private SerializedProperty spRotationRelativity;
		private SerializedProperty spRotation;

		private SerializedProperty spChangeScale;
		private SerializedProperty spScale;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.target.ToString());
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = serializedObject.FindProperty("target");

			this.spChangeParent = serializedObject.FindProperty("changeParent");
			this.spNewParent = serializedObject.FindProperty("newParent");

			this.spChangePosition = serializedObject.FindProperty("changePosition");
			this.spPositionRelativity = serializedObject.FindProperty("positionRelativity");
			this.spPosition = serializedObject.FindProperty("position");

			this.spChangeRotation = serializedObject.FindProperty("changeRotation");
			this.spRotationRelativity = serializedObject.FindProperty("rotationRelativity");
			this.spRotation = serializedObject.FindProperty("rotation");

			this.spChangeScale = serializedObject.FindProperty("changeScale");
			this.spScale = serializedObject.FindProperty("scale");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spTarget = null;
			this.spChangeParent = null;
			this.spNewParent = null;
			this.spChangePosition = null;
			this.spPositionRelativity = null;
			this.spPosition = null;
			this.spChangeRotation = null;
			this.spRotationRelativity = null;
			this.spRotation = null;
			this.spChangeScale = null;
			this.spScale = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spTarget);

			EditorGUILayout.PropertyField(this.spChangeParent);
			if (this.spChangeParent.intValue == (int)PARENT.ChangeParent)
			{
				EditorGUILayout.PropertyField(this.spNewParent);
                EditorGUILayout.Space();
			}

			EditorGUILayout.PropertyField(this.spChangePosition);
			if (this.spChangePosition.boolValue)
			{
				EditorGUILayout.PropertyField(this.spPositionRelativity);
				EditorGUILayout.PropertyField(this.spPosition);
                EditorGUILayout.Space();
			}

			EditorGUILayout.PropertyField(this.spChangeRotation);
			if (this.spChangeRotation.boolValue)
			{
				EditorGUILayout.PropertyField(this.spRotationRelativity);
				EditorGUILayout.PropertyField(this.spRotation);
                EditorGUILayout.Space();
			}

			EditorGUILayout.PropertyField(this.spChangeScale);
			if (this.spChangeScale.boolValue)
			{
				EditorGUILayout.PropertyField(this.spScale);
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}