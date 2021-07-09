namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
	using GameCreator.Core.Hooks;
    using GameCreator.Variables;

#if UNITY_EDITOR
    using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionCameraChange : IAction 
	{
		private const string TOOLTIP_TRANS_TIME = "0: No transition. Values between 0.5 and 1.5 are recommended";

        public enum CameraMotorFrom
        {
            CameraMotor,
            Variable
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool mainCameraMotor = false;

        public CameraMotorFrom from = CameraMotorFrom.CameraMotor;
        public CameraMotor cameraMotor;

        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty variable = new VariableProperty(Variable.VarType.GlobalVariable);

		[Tooltip(TOOLTIP_TRANS_TIME)] 
		[Range(0.0f, 60.0f)] 
		public float transitionTime = 0.0f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (HookCamera.Instance != null)
            {
                CameraController cameraController = HookCamera.Instance.Get<CameraController>();
                if (cameraController != null)
                {
                    CameraMotor motor = null;
                    if (this.mainCameraMotor) motor = CameraMotor.MAIN_MOTOR;
                    else
                    {
                        switch (this.from)
                        {
                            case CameraMotorFrom.CameraMotor: 
                                motor = this.cameraMotor; 
                                break;

                            case CameraMotorFrom.Variable:
                                GameObject value = this.variable.Get(target) as GameObject;
                                if (value != null) motor = value.GetComponent<CameraMotor>();
                                break;
                        }
                    }

                    if (motor != null)
                    {
                        cameraController.ChangeCameraMotor(
                            motor, 
                            this.transitionTime
                        );
                    }
                }
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Camera/Change Camera";
		private const string NODE_TITLE = "Change to camera {0} ({1})";

		private static readonly GUIContent GUICONTENT_TRANSITIONTIME = new GUIContent("Transition Time [?]");

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spMainCameraMotor;
        private SerializedProperty spMotorFrom;
        private SerializedProperty spCameraMotor;
        private SerializedProperty spVariable;
        private SerializedProperty spTransitionTime;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            string cameraName = "";
            if (this.mainCameraMotor)
            {
                cameraName = "Main Camera Motor";
            }
            else
            {
                switch (this.from)
                {
                    case CameraMotorFrom.CameraMotor:
                        cameraName = (this.cameraMotor == null ? "none" : this.cameraMotor.gameObject.name);
                        break;

                    case CameraMotorFrom.Variable:
                        cameraName = "variable";
                        break;
                }
            }

			return string.Format(
				NODE_TITLE, 
				cameraName,
				(Mathf.Approximately(this.transitionTime, 0f) 
					? "instant" 
					: string.Format("{0:0.00}s", this.transitionTime)
				)
			);
		}

		protected override void OnEnableEditorChild ()
		{
            this.spMainCameraMotor = this.serializedObject.FindProperty("mainCameraMotor");
            this.spMotorFrom = this.serializedObject.FindProperty("from");
            this.spCameraMotor = this.serializedObject.FindProperty("cameraMotor");
            this.spVariable = this.serializedObject.FindProperty("variable");
            this.spTransitionTime = this.serializedObject.FindProperty("transitionTime");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spMainCameraMotor = null;
            this.spMotorFrom = null;
            this.spCameraMotor = null;
			this.variable = null;
            this.spTransitionTime = null;
        }

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spMainCameraMotor);

            EditorGUI.BeginDisabledGroup(this.spMainCameraMotor.boolValue);
            EditorGUILayout.PropertyField(this.spMotorFrom);
            switch (this.spMotorFrom.enumValueIndex)
            {
                case (int)CameraMotorFrom.CameraMotor:
                    EditorGUILayout.PropertyField(this.spCameraMotor);
                    break;

                case (int)CameraMotorFrom.Variable:
                    EditorGUILayout.PropertyField(this.spVariable);
                    break;
            }

            EditorGUI.EndDisabledGroup();

			EditorGUILayout.PropertyField(this.spTransitionTime, GUICONTENT_TRANSITIONTIME);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}