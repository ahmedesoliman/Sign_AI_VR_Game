namespace GameCreator.Characters
{
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using GameCreator.Core;

	[CustomEditor(typeof(CharacterAnimator))]
	public class CharacterAnimatorEditor : Editor 
	{
		private const string MSG_EMPTY_MODEL = "Drop a model from your project or load the default Character.";
		private const string PATH_DEFAULT_MODEL = "Assets/Plugins/GameCreator/Characters/Models/Character.fbx";
        private const string PATH_FPS_MODEL = "Assets/Plugins/GameCreator/Characters/Models/CharacterFPS.fbx";

        private const string PATH_DEFAULT_RCONT = "Assets/Plugins/GameCreator/Characters/Animations/Controllers/Locomotion.controller";

        private const string MSG_PREFAB_INSTANCE_TITLE = "Cannot restructure Prefab instance";
        private const string MSG_PREFAB_INSTANCE_BODY = "You can open the Prefab in Prefab Mode " +
            "to change the 3D model or unpack the Prefab instance to remove its Prefab connection.";
        private const string MSG_PREFAB_INSTANCE_OK = "Ok";
        private const string MSG_PREFAB_INSTANCE_OPEN = "Open Prefab";

        // PROPERTIES: ----------------------------------------------------------------------------

        private CharacterAnimator characterAnimator;
		private CharacterEditor.Section sectionModel;
        private CharacterEditor.Section sectionIK;
        private CharacterEditor.Section sectionRagdoll;

		private SerializedProperty spAnimator;
        private SerializedProperty spDefaultState;
        private bool isDraggingModel = false;

        private SerializedProperty spUseFootIK;
        private SerializedProperty spFootLayerMask;
        private SerializedProperty spUseHandIK;
        private SerializedProperty spUseSmartHeadIK;
        private SerializedProperty spUseProceduralLanding;

        private SerializedProperty spAutoInitRagdoll;
        private SerializedProperty spRagdollMass;
        private SerializedProperty spStableTimeout;
        private SerializedProperty spStandFaceUp;
        private SerializedProperty spStandFaceDown;
        private SerializedProperty spTimeScaleCoefficient;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected void OnEnable()
		{
			this.characterAnimator = (CharacterAnimator)this.target;

			string iconModelPath = Path.Combine(CharacterEditor.CHARACTER_ICONS_PATH, "CharacterAnimModel.png");
			Texture2D iconModel = AssetDatabase.LoadAssetAtPath<Texture2D>(iconModelPath);
			this.sectionModel = new CharacterEditor.Section("Character Model", iconModel, this.Repaint);

            string iconIKPath = Path.Combine(CharacterEditor.CHARACTER_ICONS_PATH, "CharacterAnimIK.png");
            Texture2D iconIK = AssetDatabase.LoadAssetAtPath<Texture2D>(iconIKPath);
            this.sectionIK = new CharacterEditor.Section("Inverse Kinematics", iconIK, this.Repaint);

            string iconRagdollPath = Path.Combine(CharacterEditor.CHARACTER_ICONS_PATH, "CharacterAnimRagdoll.png");
            Texture2D iconRagdoll = AssetDatabase.LoadAssetAtPath<Texture2D>(iconRagdollPath);
            this.sectionRagdoll = new CharacterEditor.Section("Ragdoll", iconRagdoll, this.Repaint);

			this.spAnimator = serializedObject.FindProperty("animator");
            this.spDefaultState = serializedObject.FindProperty("defaultState");

            this.spUseFootIK = serializedObject.FindProperty("useFootIK");
            this.spFootLayerMask = serializedObject.FindProperty("footLayerMask");
            this.spUseHandIK = serializedObject.FindProperty("useHandIK");
            this.spUseSmartHeadIK = serializedObject.FindProperty("useSmartHeadIK");
            this.spUseProceduralLanding = serializedObject.FindProperty("useProceduralLanding");

            this.spAutoInitRagdoll = serializedObject.FindProperty("autoInitializeRagdoll");
            this.spRagdollMass = serializedObject.FindProperty("ragdollMass");
            this.spStableTimeout = serializedObject.FindProperty("stableTimeout");
            this.spStandFaceUp = serializedObject.FindProperty("standFaceUp");
            this.spStandFaceDown = serializedObject.FindProperty("standFaceDown");

            this.spTimeScaleCoefficient = serializedObject.FindProperty("timeScaleCoefficient");

        }

		// INSPECTOR GUI: -------------------------------------------------------------------------

		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			EditorGUILayout.Space();

            this.PaintAnimModel();
            this.PaintAnimIK();
            this.PaintAnimRagdoll();

			EditorGUILayout.Space();
			serializedObject.ApplyModifiedProperties();
		}

		private void PaintAnimModel()
		{
			this.sectionModel.PaintSection();
			using (var group = new EditorGUILayout.FadeGroupScope (this.sectionModel.state.faded))
			{
				if (group.visible)
				{
					EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

					EditorGUILayout.PropertyField(this.spAnimator);
                    EditorGUILayout.PropertyField(this.spTimeScaleCoefficient);
                    EditorGUILayout.PropertyField(this.spDefaultState);

					if (this.spAnimator.objectReferenceValue == null)
					{
						EditorGUILayout.Space();
						EditorGUILayout.HelpBox(MSG_EMPTY_MODEL, MessageType.Warning);
						this.PaintChangeModel();
					}
					else
					{
                        EditorGUILayout.Space();
                        this.PaintChangeModel();if (((Animator)this.spAnimator.objectReferenceValue).applyRootMotion)
						{
                            Animator reference = (Animator)this.spAnimator.objectReferenceValue;
                            reference.applyRootMotion = false;
                            this.spAnimator.objectReferenceValue = reference;

                            serializedObject.ApplyModifiedProperties();
                            serializedObject.Update();
						}
					}

					EditorGUILayout.EndVertical();
				}
			}
		}

        private void PaintAnimIK()
        {
            this.sectionIK.PaintSection();
            using (var group = new EditorGUILayout.FadeGroupScope(this.sectionIK.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                    EditorGUILayout.PropertyField(this.spUseFootIK);
                    if (this.spUseFootIK.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(this.spFootLayerMask);
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.PropertyField(this.spUseHandIK);
                    EditorGUILayout.PropertyField(this.spUseSmartHeadIK);
                    EditorGUILayout.PropertyField(this.spUseProceduralLanding);
                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void PaintAnimRagdoll()
        {
            this.sectionRagdoll.PaintSection();
            using (var group = new EditorGUILayout.FadeGroupScope(this.sectionRagdoll.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                    EditorGUILayout.PropertyField(this.spAutoInitRagdoll);
                    EditorGUILayout.PropertyField(this.spRagdollMass);
                    EditorGUILayout.PropertyField(this.spStableTimeout);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(this.spStandFaceUp);
                    EditorGUILayout.PropertyField(this.spStandFaceDown);
                    EditorGUILayout.EndVertical();
                }
            }
        }

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void PaintChangeModel()
		{
            Event evt = Event.current;
            Rect rect = GUILayoutUtility.GetRect(
                0.0f, 
                50.0f, 
                GUILayout.ExpandWidth(true)
            );

            float dropPadding = 2f;
            Rect dropRect = new Rect(
                rect.x + EditorGUIUtility.labelWidth + dropPadding,
                rect.y,
                rect.width - EditorGUIUtility.labelWidth - (2f * dropPadding),
                rect.height
            );

            GUIStyle styleDropZone = (this.isDraggingModel
                ? CoreGUIStyles.GetDropZoneActive()
                : CoreGUIStyles.GetDropZoneNormal()
            );
            
            GUI.Box(dropRect, "Drop your 3D model", styleDropZone);

            Rect buttonRectA = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetButtonLeft());
            buttonRectA = new Rect(
                buttonRectA.x + EditorGUIUtility.labelWidth, 
                buttonRectA.y,
                (buttonRectA.width / 2f) - EditorGUIUtility.labelWidth / 2.0f, 
                buttonRectA.height
            );
            
            Rect buttonRectB = new Rect(
                buttonRectA.x + buttonRectA.width,
                buttonRectA.y,
                buttonRectA.width - 2f,
                buttonRectA.height
            );

            if (GUI.Button(buttonRectA, "Default Character", CoreGUIStyles.GetButtonLeft()))
            {
                GameObject prefabDefault = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_DEFAULT_MODEL);
                this.LoadCharacter(prefabDefault);
            }

            if (GUI.Button(buttonRectB, "FPS Character", CoreGUIStyles.GetButtonRight()))
            {
                GameObject prefabFPS = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_FPS_MODEL);
                this.LoadCharacter(prefabFPS);
            }

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    this.isDraggingModel = false;
                    if (!dropRect.Contains(evt.mousePosition)) break;
                    if (DragAndDrop.objectReferences.Length != 1) break;

                    GameObject draggedObject = DragAndDrop.objectReferences[0] as GameObject;
                    if (draggedObject == null) break;

                    bool prefabAllowed = (
                        PrefabUtility.GetPrefabAssetType(draggedObject) == PrefabAssetType.Model ||
                        PrefabUtility.GetPrefabAssetType(draggedObject) == PrefabAssetType.Regular ||
                        PrefabUtility.GetPrefabAssetType(draggedObject) == PrefabAssetType.Variant
                    );

                    if (!prefabAllowed) break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragUpdated)
                    {
                        this.isDraggingModel = true;
                    }
                    else if (evt.type == EventType.DragPerform)
                    {
                        this.isDraggingModel = false;

                        DragAndDrop.AcceptDrag();
                        this.LoadCharacter(draggedObject);
                    }
                    break;
            }
		}

		private Rect GetButtonRect()
		{
			Rect buttonRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
			return new Rect(
				buttonRect.x + EditorGUIUtility.labelWidth, buttonRect.y,
				buttonRect.width - EditorGUIUtility.labelWidth, buttonRect.height
			);
		}

        private void LoadCharacter(GameObject prefab)
		{
			if (prefab == null) return;
			if (prefab.GetComponentInChildren<Animator>() == null) return;
            if (PrefabUtility.IsPartOfNonAssetPrefabInstance(this.characterAnimator.gameObject))
            {
                bool enterPrefabMode = EditorUtility.DisplayDialog(
                    MSG_PREFAB_INSTANCE_TITLE,
                    MSG_PREFAB_INSTANCE_BODY,
                    MSG_PREFAB_INSTANCE_OPEN,
                    MSG_PREFAB_INSTANCE_OK
                );

                if (enterPrefabMode)
                {
                    string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this.target);
                    AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(path));
                }

                return;
            }

			GameObject instance = Instantiate<GameObject>(prefab);
			instance.name = prefab.name;

			instance.transform.SetParent(this.characterAnimator.transform);
			instance.transform.localPosition = Vector3.zero;
			instance.transform.localRotation = Quaternion.identity;

			Animator instanceAnimator = instance.GetComponentInChildren<Animator>();
			RuntimeAnimatorController rc = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(PATH_DEFAULT_RCONT);
			instanceAnimator.runtimeAnimatorController = rc;

			if (this.spAnimator.objectReferenceValue != null)
			{
				Animator previous = (Animator)this.spAnimator.objectReferenceValue;
				DestroyImmediate(previous.gameObject);
			}

			this.spAnimator.objectReferenceValue = instanceAnimator;
			serializedObject.ApplyModifiedProperties();
			serializedObject.Update();
		}
	}
}