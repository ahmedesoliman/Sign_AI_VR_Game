namespace GameCreator.ModuleManager
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.AnimatedValues;
    using GameCreator.Core;

    [CustomEditor(typeof(AssetModule))]
    public class AssetModuleEditor : Editor
    {
        private const string TITLE = "{0} - {1}.{2}.{3}";
        private const string PASSWORD = "gamecreator";

        private const string ADMIN_ICON_PATH = "Assets/Plugins/GameCreator/Modules/Icons/UI/AdminLock.png";
        private const string MSG_DEV = "Restricted Area. Game Creator employees only";
        private static GUIContent GUICONTENT_ADMIN;

        // PROPERTIES: ----------------------------------------------------------------------------

        private string password = "";
        private AssetModule assetModule;

        private SerializedProperty spAdminLogin;
        private SerializedProperty spAdminOpen;

        private SerializedProperty spModuleID;
        private SerializedProperty spVersionMajor;
        private SerializedProperty spVersionMinor;
        private SerializedProperty spVersionPatch;

        private SerializedProperty spDisplayName;
        private SerializedProperty spDescription;
        private SerializedProperty spCategory;

        private SerializedProperty spDependencies;
        private SerializedProperty spTags;

        private SerializedProperty spAuthorName;
        private SerializedProperty spAuthorMail;
        private SerializedProperty spAuthorSite;

        private SerializedProperty spIncludesData;
        private SerializedProperty spCodePaths;
        private SerializedProperty spDataPaths;

        // INITIALIZERS: --------------------------------------------------------------------------
        
		private void OnEnable()
		{
            this.assetModule = (AssetModule)target;

            this.spAdminLogin = serializedObject.FindProperty("adminLogin");
            this.spAdminOpen = serializedObject.FindProperty("adminOpen");

            SerializedProperty spModule = serializedObject.FindProperty("module");
            this.spModuleID = spModule.FindPropertyRelative("moduleID");

            SerializedProperty spVersion = spModule.FindPropertyRelative("version");
            this.spVersionMajor = spVersion.FindPropertyRelative("major");
            this.spVersionMinor = spVersion.FindPropertyRelative("minor");
            this.spVersionPatch = spVersion.FindPropertyRelative("patch");

            this.spDisplayName = spModule.FindPropertyRelative("displayName");
            this.spDescription = spModule.FindPropertyRelative("description");
            this.spCategory = spModule.FindPropertyRelative("category");

            this.spDependencies = spModule.FindPropertyRelative("dependencies");
            this.spTags = spModule.FindPropertyRelative("tags");

            this.spAuthorName = spModule.FindPropertyRelative("authorName");
            this.spAuthorMail = spModule.FindPropertyRelative("authorMail");
            this.spAuthorSite = spModule.FindPropertyRelative("authorSite");

            this.spIncludesData = spModule.FindPropertyRelative("includesData");
            this.spCodePaths = spModule.FindPropertyRelative("codePaths");
            this.spDataPaths = spModule.FindPropertyRelative("dataPaths");
		}

        // PAINT METHODS: -------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
            serializedObject.Update();

            this.PaintInfo();
            this.PaintDeveloper();

            serializedObject.ApplyModifiedProperties();
		}

        private void PaintInfo()
        {
            string title = string.Format(
                TITLE,
                this.spModuleID.stringValue,
                this.spVersionMajor.intValue,
                this.spVersionMinor.intValue,
                this.spVersionPatch.intValue
            );

            EditorGUILayout.LabelField(this.spDisplayName.stringValue, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(title, EditorStyles.label);
        }

        private void PaintDeveloper()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUIStyle style = (this.spAdminOpen.boolValue
                ? CoreGUIStyles.GetToggleButtonNormalOn()
                : CoreGUIStyles.GetToggleButtonNormalOff()
            );

            if (GUICONTENT_ADMIN == null)
            {
                Texture2D adminTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(ADMIN_ICON_PATH);
                GUICONTENT_ADMIN = new GUIContent(" Settings", adminTexture);
            }

            if (GUILayout.Button(GUICONTENT_ADMIN, style))
            {
                this.spAdminOpen.boolValue = !this.spAdminOpen.boolValue;
            }

            if (this.spAdminOpen.boolValue)
            {
                if (this.spAdminLogin.boolValue)
                {
                    this.PaintDeveloperAdmin();
                }
                else
                {
                    EditorGUILayout.HelpBox(MSG_DEV, MessageType.Warning);
                    EditorGUILayout.BeginHorizontal();
                    this.password = EditorGUILayout.PasswordField(this.password);
                    if (GUILayout.Button("Sign in", EditorStyles.miniButton))
                    {
                        if (this.password == PASSWORD) this.spAdminLogin.boolValue = true;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void PaintDeveloperAdmin()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build Module", CoreGUIStyles.GetButtonLeft()))
            {
                EditorApplication.update += this.BuildModuleDeferred;
            }
            if (GUILayout.Button("Logout", CoreGUIStyles.GetButtonRight()))
            {
                this.spAdminLogin.boolValue = false;
                this.password = "";
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Information", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spModuleID);
            EditorGUILayout.PropertyField(this.spDisplayName);
            EditorGUILayout.PropertyField(this.spDescription);
            EditorGUILayout.PropertyField(this.spCategory);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Version", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spVersionMajor);
            EditorGUILayout.PropertyField(this.spVersionMinor);
            EditorGUILayout.PropertyField(this.spVersionPatch);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Author", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spAuthorName);
            EditorGUILayout.PropertyField(this.spAuthorMail);
            EditorGUILayout.PropertyField(this.spAuthorSite);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dependencies", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spDependencies, true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Tags", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spTags, true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Code Paths", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spCodePaths, true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Data Paths", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spIncludesData);
            EditorGUILayout.PropertyField(this.spDataPaths, true);
            EditorGUI.indentLevel--;

            EditorGUI.EndDisabledGroup();
        }

        private void BuildModuleDeferred()
        {
            EditorApplication.update -= this.BuildModuleDeferred;
            this.assetModule.BuildModule();
        }
	}
}