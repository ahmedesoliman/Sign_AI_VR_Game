namespace GameCreator.Characters
{
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;

    [CustomEditor(typeof(CharacterState), true)]
    public class CharacterStateEditor : Editor
    {
        private const string PATH_RTC_SIM = "Assets/Plugins/GameCreator/Characters/Animations/Overriders/Simple.overrideController";
        private const string PATH_RTC_LOC = "Assets/Plugins/GameCreator/Characters/Animations/Overriders/Locomotion.overrideController";

        private static readonly GUIContent GC_OTHER = new GUIContent("Animator");
        private static readonly GUIContent GC_AVATAR = new GUIContent("Avatar Mask (optional)");

        private const string MSG_SIM = "Set a State where the character uses one single animation";
        private const string MSG_LOC = "Use your own animations using the built-in transitions";
        private const string MSG_OTH = "Add your custom Animator";

        // PROPERTIES: ----------------------------------------------------------------------------

        private Editor editorAnimatorSim;
        private Editor editorAnimatorLoc;
        private Editor editorAnimatorOth;

        private SerializedProperty spType;
        private SerializedProperty spRtcSim;
        private SerializedProperty spRtcLoc;
        private SerializedProperty spRtcOth;

        private SerializedProperty spEnterClip;
        private SerializedProperty spEnterMask;
        private SerializedProperty spExitClip;
        private SerializedProperty spExitMask;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            this.spType = serializedObject.FindProperty("type");
            this.spRtcSim = serializedObject.FindProperty("rtcSimple");
            this.spRtcLoc = serializedObject.FindProperty("rtcLocomotion");
            this.spRtcOth = serializedObject.FindProperty("rtcOther");

            this.spEnterClip = serializedObject.FindProperty("enterClip");
            this.spEnterMask = serializedObject.FindProperty("enterAvatarMask");
            this.spExitClip = serializedObject.FindProperty("exitClip");
            this.spExitMask = serializedObject.FindProperty("exitAvatarMask");

            this.editorAnimatorSim = Editor.CreateEditor(this.spRtcSim.objectReferenceValue);
            this.editorAnimatorLoc = Editor.CreateEditor(this.spRtcLoc.objectReferenceValue);
        }

		// PAINT METHODS: -------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
            serializedObject.Update();

            EditorGUILayout.PropertyField(this.spType);

            switch (this.spType.intValue)
            {
                case (int)CharacterState.StateType.Simple :
                    EditorGUILayout.HelpBox(MSG_SIM, MessageType.Info);
                    this.editorAnimatorSim.OnInspectorGUI();
                    break;

                case (int)CharacterState.StateType.Locomotion:
                    EditorGUILayout.HelpBox(MSG_LOC, MessageType.Info);
                    this.editorAnimatorLoc.OnInspectorGUI();
                    break;

                case (int)CharacterState.StateType.Other:
                    EditorGUILayout.HelpBox(MSG_OTH, MessageType.Info);
                    EditorGUILayout.PropertyField(this.spRtcOth, GC_OTHER);
                    break;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.spEnterClip);
            EditorGUILayout.PropertyField(this.spEnterMask, GC_AVATAR);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spExitClip);
            EditorGUILayout.PropertyField(this.spExitMask, GC_AVATAR);

            serializedObject.ApplyModifiedProperties();
		}

        ///////////////////////////////////////////////////////////////////////////////////////////
		// FACTORY METHODS: -----------------------------------------------------------------------

        [MenuItem("Assets/Create/Game Creator/Characters/Simple State", false, 50)]
        public static void CreateCharacterState_Simple()
        {
            CharacterState state = CharacterStateEditor.CreateState("StateSimple.asset");
            state.type = CharacterState.StateType.Simple;
            state.name = "Simple Character State";
        }

        [MenuItem("Assets/Create/Game Creator/Characters/Locomotion State", false, 50)]
        public static void CreateCharacterState_Locomotion()
        {
            CharacterState state = CharacterStateEditor.CreateState("StateLocomotion.asset");
            state.type = CharacterState.StateType.Locomotion;
            state.name = "Locomotion Character State";
        }

        [MenuItem("Assets/Create/Game Creator/Characters/Advanced State", false, 50)]
        public static void CreateCharacterState_Advanced()
        {
            CharacterState state = CharacterStateEditor.CreateState("StateAdvanced.asset");
            state.type = CharacterState.StateType.Locomotion;
            state.name = "Advanced Character State";
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static CharacterState CreateState(string stateName)
        {
            CharacterState asset = ScriptableObject.CreateInstance<CharacterState>();

            AnimatorOverrideController rtcSim = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(PATH_RTC_SIM);
            AnimatorOverrideController rtcLoc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(PATH_RTC_LOC);

            AnimatorOverrideController assetRTCSim = Object.Instantiate<AnimatorOverrideController>(rtcSim);
            AnimatorOverrideController assetRTCLoc = Object.Instantiate<AnimatorOverrideController>(rtcLoc);

            assetRTCSim.name = "Simple";
            assetRTCLoc.name = "Locomotion";

            assetRTCSim.hideFlags = HideFlags.HideInHierarchy;
            assetRTCLoc.hideFlags = HideFlags.HideInHierarchy;

            asset.rtcSimple = assetRTCSim;
            asset.rtcLocomotion = assetRTCLoc;

            string creationPath = Path.Combine(CreateCoreAsset.GetSelectionPath(), stateName);
            AssetDatabase.CreateAsset(asset, creationPath);
            AssetDatabase.SaveAssets();

            AssetDatabase.AddObjectToAsset(assetRTCSim, asset);
            AssetDatabase.AddObjectToAsset(assetRTCLoc, asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(creationPath);

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return asset;
        }
    }
}