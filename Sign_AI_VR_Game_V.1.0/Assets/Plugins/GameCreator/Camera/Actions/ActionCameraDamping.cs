namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
	using GameCreator.Camera;
	using GameCreator.Core.Hooks;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionCameraDamping : IAction
	{
		[Range(0f, 1f)] public float dampingTranslation = 0.1f;
		[Range(0f, 1f)] public float dampingRotation = 0.1f;

        // EXECUTABLE: -----------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (HookCamera.Instance != null)
            {
                CameraController cameraController = HookCamera.Instance.Get<CameraController>();
                if (cameraController != null)
                {
                    cameraController.cameraSmoothTime.positionDuration = this.dampingTranslation;
                    cameraController.cameraSmoothTime.rotationDuration = this.dampingRotation;
                }
            }

            return true;
        }

		// +---------------------------------------------------------------------------------------------------------------+
		// | EDITOR                                                                                                        |
		// +---------------------------------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    public static new string NAME = "Camera/Camera Damping";
		private const string NODE_TITLE = "Change camera damping to {0} {1}";

		// PROPERTIES: -----------------------------------------------------------------------------------------------------

		private SerializedProperty spDampingTranslation;
		private SerializedProperty spDampingRotation;

		// INSPECTOR METHODS: ----------------------------------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.dampingTranslation, this.dampingRotation);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spDampingTranslation = this.serializedObject.FindProperty("dampingTranslation");
			this.spDampingRotation = this.serializedObject.FindProperty("dampingRotation");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spDampingTranslation = null;
			this.spDampingRotation = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spDampingTranslation);
			EditorGUILayout.PropertyField(this.spDampingRotation);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}