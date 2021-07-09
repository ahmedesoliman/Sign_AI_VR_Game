using System.Runtime.CompilerServices;
namespace GameCreator.Camera
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEngine;
	using UnityEditor;
	using GameCreator.Core;

	[CustomEditor(typeof(CameraMotor))]
	public class CameraMotorEditor : Editor
	{
        private const float PREVIEW_PADDING = 10f;

        private const string MSG_MAINCAM1 = "There's another Main Camera Motor in the current scene";
        private const string MSG_MAINCAM2 = "Do you want keep '{0}' as the Main Camera Motor or change it to this one?";
		private const string MSG_NO_MOTOR_TYPE = "No Motor Type is selected";
        private const string PROP_IS_MAIN_CAM_MOTOR = "isMainCameraMotor";
		private const string PROP_CAMERA_MOTOR_TYPE = "cameraMotorType";

		private class CameraMotorTypeData
		{
			public Type[] types;
			public string[] names;
			public int[] indexes;

			public CameraMotorTypeData(int length)
			{
				this.types = new Type[length];
				this.names = new string[length];
				this.indexes = new int[length];
			}
		}

		// PROPERTIES: ----------------------------------------------------------------------------

		private CameraMotorTypeData cameraMotorsClasses;
        private int selectMotorIndex = 0;

        private CameraMotor cameraMotor;
        private SerializedProperty spIsMainCameraMotor;
		private SerializedProperty spCameraMotorType;
		private ICameraMotorTypeEditor cameraMotorEditor;

        private Camera previewCamera;
        private RenderTexture previewTexture;
        private float previewAR = 1.0f;
        private Rect previewRect = Rect.zero;

		// INITIALIZERS: --------------------------------------------------------------------------

		private void OnEnable()
		{
			this.cameraMotor = (CameraMotor)target;

            this.spIsMainCameraMotor = serializedObject.FindProperty(PROP_IS_MAIN_CAM_MOTOR);
			this.spCameraMotorType = serializedObject.FindProperty(PROP_CAMERA_MOTOR_TYPE);

			this.GenerateMotorTypes();

			if (this.spCameraMotorType.objectReferenceValue == null)
			{
				this.ChangeMotorType(0);
			}
			else
			{
				this.UpdateMotorType((ICameraMotorType)this.spCameraMotorType.objectReferenceValue);
			}

            CameraController cameraController = FindObjectOfType<CameraController>();
            if (cameraController == null)
            {
                Camera mainCamera = Camera.main ?? FindObjectOfType<Camera>();
                if (mainCamera != null)
                {
                    cameraController = mainCamera.gameObject.AddComponent<CameraController>();
                    cameraController.currentCameraMotor = this.cameraMotor;
                }
            }

            this.InitPreviewCamera();
		}

		private void OnDisable()
		{
			this.cameraMotorEditor = null;
            if (this.previewCamera != null)
            {
                DestroyImmediate(this.previewCamera.gameObject, true);
            }
		}

        // INSPECTOR GUI: -------------------------------------------------------------------------

        public override void OnInspectorGUI ()
		{
			serializedObject.Update();

            EditorGUILayout.Space();
            bool prevIsMainCamMotor = this.spIsMainCameraMotor.boolValue;
            EditorGUILayout.PropertyField(this.spIsMainCameraMotor);
            if (!prevIsMainCamMotor && this.spIsMainCameraMotor.boolValue)
            {
                int curCamMotorID = this.cameraMotor.GetInstanceID();
                CameraMotor[] cameraMotors = GameObject.FindObjectsOfType<CameraMotor>();
                for (int i = 0; i < cameraMotors.Length; ++i)
                {
                    if (cameraMotors[i].GetInstanceID() != curCamMotorID && cameraMotors[i].isMainCameraMotor)
                    {
                        bool dialogResult = EditorUtility.DisplayDialog(
                            MSG_MAINCAM1,
                            string.Format(MSG_MAINCAM2, cameraMotors[i].name),
                            "Change",
                            "Keep previous"
                        );

                        if (dialogResult)
                        {
                            SerializedObject soOtherCam = new SerializedObject(cameraMotors[i]);
                            soOtherCam.FindProperty(PROP_IS_MAIN_CAM_MOTOR).boolValue = false;
                            soOtherCam.ApplyModifiedPropertiesWithoutUndo();
                            soOtherCam.Update();
                        }
                        else
                        {
                            this.spIsMainCameraMotor.boolValue = false;
                        }
                    }
                }
            }

			this.PaintSelectMotor();
            EditorGUILayout.BeginVertical(CoreGUIStyles.GetBox());

            this.PaintConfigMotor();

			EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}

		private void PaintSelectMotor()
		{
			EditorGUILayout.BeginHorizontal();

            GUILayout.Button(
                this.cameraMotorsClasses.names[this.selectMotorIndex],
                CoreGUIStyles.GetGridButtonLeftOn(),
                GUILayout.Height(22f)
            );

            bool clickSelect = GUILayout.Button(
                "Change...",
                CoreGUIStyles.GetGridButtonRightOff(),
                GUILayout.Height(22f),
                GUILayout.Width(80f)
            );

            if (clickSelect)
            {
                GenericMenu motorMenu = new GenericMenu();
                for (int i = 0; i < this.cameraMotorsClasses.indexes.Length; ++i)
                {
                    motorMenu.AddItem(
                        new GUIContent(this.cameraMotorsClasses.names[i]),
                        false,
                        this.ChangeMotorType,
                        i
                    );
                }

                motorMenu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();
        }

		private void PaintConfigMotor()
		{
			if (this.cameraMotorEditor != null) 
			{
                if (this.cameraMotorEditor.PaintInspectorMotor(this.cameraMotor.transform)) 
				{
					SceneView.RepaintAll();
				}
			}
			else
			{
				EditorGUILayout.HelpBox(MSG_NO_MOTOR_TYPE, MessageType.Warning);
			}
		}

		// SCENE GUI: -----------------------------------------------------------------------------

		public void OnSceneGUI()
		{
			if (this.cameraMotorEditor == null) return;
            if (this.cameraMotorEditor.PaintSceneMotor(this.cameraMotor.transform)) 
			{
				this.Repaint();
			}

            if (Camera.current != null) this.previewAR = Camera.current.aspect;
		}

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private List<Type> GetAllClassTypesOf(Type parentType)
		{
			List<Type> result = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; ++i)
			{
				Type[] types = assemblies[i].GetTypes();
				for (int j = 0; j < types.Length; ++j)
				{
					if (types[j].BaseType == parentType)
					{
						result.Add(types[j]);
					}
				}
			}

			return result;
		}

		private void GenerateMotorTypes()
		{
            List<Type> cameraMotorTypesData = this.GetAllClassTypesOf(typeof(ICameraMotorType));
			int cameraMotorTypesLength = cameraMotorTypesData.Count;
			this.cameraMotorsClasses = new CameraMotorTypeData(cameraMotorTypesLength);

            for (int i = 0; i < cameraMotorTypesLength; ++i)
			{
				if (this.spCameraMotorType.objectReferenceValue != null &&
					this.spCameraMotorType.objectReferenceValue.GetType() == cameraMotorTypesData[i])
				{
					this.selectMotorIndex = i;
				}

				this.cameraMotorsClasses.types[i] = cameraMotorTypesData[i];
				this.cameraMotorsClasses.names[i] = (string)cameraMotorTypesData[i].GetField("NAME").GetValue(null);
				this.cameraMotorsClasses.indexes[i] = i;
			}
		}

		private void ChangeMotorType(object genericIndex)
		{
            int cameraIndex = (int)genericIndex;
            if (this.spCameraMotorType.objectReferenceValue != null)
            {
                DestroyImmediate(this.spCameraMotorType.objectReferenceValue);
            }

            this.selectMotorIndex = cameraIndex;
            Type type = this.cameraMotorsClasses.types[cameraIndex];
            ICameraMotorType motorType = (ICameraMotorType)this.cameraMotor.gameObject.AddComponent(type);

            this.UpdateMotorType(motorType);
			this.cameraMotorEditor.OnCreate(this.cameraMotor.transform);
		}

        private void UpdateMotorType(ICameraMotorType motorType)
		{
            
            this.spCameraMotorType.objectReferenceValue = motorType;
            this.spCameraMotorType.objectReferenceValue.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            this.cameraMotorEditor = (ICameraMotorTypeEditor)Editor.CreateEditor(this.spCameraMotorType.objectReferenceValue);

			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			serializedObject.Update();
		}

        // CAMERA PREVIEW METHODS: ----------------------------------------------------------------

        private void InitPreviewCamera()
        {
            this.previewCamera = EditorUtility.CreateGameObjectWithHideFlags(
                "Preview Camera",
                HideFlags.HideAndDontSave,
                typeof(Camera)
            ).GetComponent<Camera>();

            this.previewCamera.cameraType = CameraType.Preview;
            this.previewCamera.depth = -999;
        }

        public override bool HasPreviewGUI()
        {
            return this.cameraMotorEditor.ShowPreviewCamera();
        }

        public override bool RequiresConstantRepaint()
        {
            return this.cameraMotorEditor.ShowPreviewCamera();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (!this.cameraMotorEditor.ShowPreviewCamera()) return;

            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                if (this.previewRect != r)
                {
                    Vector2 size1 = new Vector2(r.width, r.width / this.previewAR);
                    Vector2 size2 = new Vector2(r.height * this.previewAR, r.height);

                    float scale1 = Mathf.Min(r.width / size1.x, r.height / size1.y);
                    float scale2 = Mathf.Min(r.width / size2.x, r.height / size2.y);

                    Vector2 size = (scale1 < scale2 ? size1 * scale1 : size2 * scale2);

                    this.previewTexture = new RenderTexture(
                        (int)size.x * Mathf.CeilToInt(EditorGUIUtility.pixelsPerPoint),
                        (int)size.y * Mathf.CeilToInt(EditorGUIUtility.pixelsPerPoint),
                        24,
                        RenderTextureFormat.Default
                    );

                    this.previewTexture.Create();
                    this.previewCamera.targetTexture = this.previewTexture;
                }

                this.previewCamera.transform.SetPositionAndRotation(
                    this.cameraMotor.transform.position,
                    this.cameraMotor.transform.rotation
                );

                ICameraMotorType motorType = this.cameraMotor.cameraMotorType;
                bool orthographic = (
                    motorType.setCameraProperties && 
                    motorType.projection == ICameraMotorType.Projection.Orthographic
                );

                this.previewCamera.orthographic = orthographic;
                this.previewCamera.orthographicSize = (orthographic && motorType.setCameraProperties
                    ? motorType.cameraSize
                    : 5f
                );

                this.previewCamera.fieldOfView = (!orthographic && motorType.setCameraProperties
                    ? motorType.fieldOfView
                    : 60f
                );
                
                this.previewCamera.Render();
                GUI.DrawTexture(r, this.previewTexture, ScaleMode.ScaleToFit, false);
            }
        }

		// HIERARCHY CONTEXT MENU: ----------------------------------------------------------------

		[MenuItem("GameObject/Game Creator/Other/Camera Motor", false, 0)]
		public static void CreateCameraMotor()
		{
			GameObject cameraMotor = CreateSceneObject.Create("Camera Motor");
			cameraMotor.AddComponent<CameraMotor>();
		}
	}
}