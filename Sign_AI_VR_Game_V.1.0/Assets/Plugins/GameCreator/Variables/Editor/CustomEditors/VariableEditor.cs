namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;
    using System.Text.RegularExpressions;

    [CustomEditor(typeof(MBVariable))]
    public class MBVariableEditor : VariableEditor 
    {
        protected override bool UseTags() { return false; }
        protected override bool CanSave() { return ((MBVariable)this.target).CanSave(); }

        public override Variable GetRuntimeVariable()
        {
            return LocalVariablesUtilities.Get(
                ((MBVariable)this.target).gameObject,
                this.spVariableName.stringValue,
                false
            );
        }
    }

    [CustomEditor(typeof(SOVariable))]
    public class SOVariableEditor : VariableEditor 
    { 
        protected override bool UseTags() { return true; }
        protected override bool CanSave() { return ((SOVariable)this.target).CanSave(); }

        public override Variable GetRuntimeVariable()
        {
            return GlobalVariablesUtilities.Get(
                this.spVariableName.stringValue
            );
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////

    public abstract class VariableEditor : Editor
    {
        private static readonly GUIContent GUICONTENT_NAME = new GUIContent("Name");
        private static readonly GUIContent GUICONTENT_TYPE = new GUIContent("Type");
        private static readonly GUIContent GUICONTENT_TAGS = new GUIContent("Tags");
        private static readonly GUIContent GUICONTENT_VALUE = new GUIContent("Value");

        private static readonly Regex REGEX_VARNAME = new Regex(@"[^\p{L}\p{Nd}-_]");
        private static readonly Regex REGEX_VARPATH = new Regex(@"[^\p{L}\p{Nd}-_\/]");

        private const string PROP_VARIABLE = "variable";
        private const string PROP_NAME = "name";
        private const string PROP_SAVE = "save";
        private const string PROP_TYPE = "type";
        private const string PROP_TAGS = "tags";

        // PROPERTIES: ----------------------------------------------------------------------------

        public SerializedProperty spVariable;
        public bool editableType = true;
        public bool editableCommon = true;

        public SerializedProperty spVariableName;
        public SerializedProperty spVariableSave;
        public SerializedProperty spVariableType;
        public SerializedProperty spVariableTags;

        public SerializedProperty spVariableStr;
        public SerializedProperty spVariableInt;
        public SerializedProperty spVariableNum;
        public SerializedProperty spVariableBol;
        public SerializedProperty spVariableCol;
        public SerializedProperty spVariableVc2;
        public SerializedProperty spVariableVc3;
        public SerializedProperty spVariableTxt;
        public SerializedProperty spVariableSpr;
        public SerializedProperty spVariableObj;
        public SerializedProperty spVariableTrn;
        public SerializedProperty spVariableRbd;

        private SerializedProperty spRuntimeValue;

		// INITIALIZERS: --------------------------------------------------------------------------

		private void OnEnable()
		{
            this.target.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

            this.spVariable = serializedObject.FindProperty(PROP_VARIABLE);

            this.spVariableName = this.spVariable.FindPropertyRelative(PROP_NAME);
            this.spVariableSave = this.spVariable.FindPropertyRelative(PROP_SAVE);
            this.spVariableType = this.spVariable.FindPropertyRelative(PROP_TYPE);
            this.spVariableTags = this.spVariable.FindPropertyRelative(PROP_TAGS);

            this.spVariableStr = this.spVariable.FindPropertyRelative("varStr");
            this.spVariableInt = this.spVariable.FindPropertyRelative("varInt");
            this.spVariableNum = this.spVariable.FindPropertyRelative("varNum");
            this.spVariableBol = this.spVariable.FindPropertyRelative("varBol");
            this.spVariableCol = this.spVariable.FindPropertyRelative("varCol");
            this.spVariableVc2 = this.spVariable.FindPropertyRelative("varVc2");
            this.spVariableVc3 = this.spVariable.FindPropertyRelative("varVc3");
            this.spVariableTxt = this.spVariable.FindPropertyRelative("varTxt");
            this.spVariableSpr = this.spVariable.FindPropertyRelative("varSpr");
            this.spVariableObj = this.spVariable.FindPropertyRelative("varObj");
            this.spVariableTrn = this.spVariable.FindPropertyRelative("varTrn");
            this.spVariableRbd = this.spVariable.FindPropertyRelative("varRbd");
		}

        // PUBLIC METHODDS: -----------------------------------------------------------------------

        public string GetName()
        {
            if (this.spVariableSave.boolValue) return this.spVariableName.stringValue + " (save)";
            return this.spVariableName.stringValue;
        }

        public bool MatchSearch(string search, int tagsMask)
        {
            bool matchSearch = this.spVariableName.stringValue.Contains(search);
            bool matchTags = (tagsMask & this.spVariableTags.intValue) != 0;
            return matchSearch && matchTags;
        }

        public static string ProcessName(string name, bool isPath = false)
        {
            string processed = name.Trim();
            switch (isPath)
            {
                case true:  processed = REGEX_VARPATH.Replace(processed, "-"); break;
                case false: processed = REGEX_VARNAME.Replace(processed, "-"); break;
            }

            return processed;
        }

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected abstract bool UseTags();
        protected abstract bool CanSave();
        public abstract Variable GetRuntimeVariable();

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
		{
            serializedObject.Update();

            GUILayout.Space(2f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(2f);
            EditorGUILayout.BeginVertical();

            this.PaintCommon();
            this.PaintType();

            if (EditorApplication.isPlaying) this.PaintRuntimeValue();
            else this.PaintValue();

            this.PaintTags();

            EditorGUILayout.EndVertical();
            GUILayout.Space(2f);
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
		}

        private void PaintCommon()
        {
            if (!this.editableCommon) return;

            float saveWidth = 18f;
            float saveOffset = 5f;
            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.fieldWidth + EditorGUIUtility.fieldWidth,
                EditorGUIUtility.singleLineHeight
            );

            Rect rectLabel = new Rect(
                rect.x,
                rect.y,
                EditorGUIUtility.labelWidth,
                rect.height
            );
            Rect rectName = new Rect(
                rect.x + rectLabel.width,
                rect.y,
                rect.width - rectLabel.width - saveWidth - saveOffset,
                rect.height
            );
            Rect rectSave = new Rect(
                rectName.x + rectName.width + saveOffset,
                rect.y,
                saveWidth,
                rect.height
            );

            EditorGUI.PrefixLabel(rectLabel, GUICONTENT_NAME);
            string previousName = this.spVariableName.stringValue;

            EditorGUI.PropertyField(rectName, this.spVariableName, GUIContent.none);

            if (!this.CanSave() && this.spVariableSave.boolValue)
            {
                this.spVariableSave.boolValue = false;
            }

            EditorGUI.BeginDisabledGroup(!this.CanSave());
            EditorGUI.PropertyField(rectSave, this.spVariableSave, GUIContent.none);
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(2f);

            if (previousName != this.spVariableName.stringValue)
            {
                string varName = ProcessName(this.spVariableName.stringValue);
                this.spVariableName.stringValue = varName;
            }
        }


        private void PaintType()
        {
            if (!this.editableType) return;

            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.fieldWidth + EditorGUIUtility.fieldWidth,
                EditorGUIUtility.singleLineHeight
            );

            Rect rectLabel = new Rect(
                rect.x, 
                rect.y, 
                EditorGUIUtility.labelWidth, 
                rect.height
            );
            Rect rectDropdown = new Rect(
                rect.x + rectLabel.width, 
                rect.y, 
                rect.width - rectLabel.width,
                rect.height
            );

            EditorGUI.PrefixLabel(rectLabel, GUICONTENT_TYPE);

            string typeName = ((Variable.DataType)this.spVariableType.intValue).ToString();
            if (EditorGUI.DropdownButton(rectDropdown, new GUIContent(typeName), FocusType.Keyboard))
            {
                SelectTypePanel selectTypePanel = new SelectTypePanel(
                    this.ChangeTypeCallback, 
                    "Variables",
                    typeof(VariableBase),
                    rectDropdown.width
                );

                PopupWindow.Show(rectDropdown, selectTypePanel);
            }

            GUILayout.Space(2f);
        }

        private void PaintValue()
        {
            switch ((Variable.DataType)this.spVariableType.intValue)
            {
                case Variable.DataType.String : this.PaintProperty(this.spVariableStr); break;
                case Variable.DataType.Number: this.PaintProperty(this.spVariableNum); break;
                case Variable.DataType.Bool: this.PaintProperty(this.spVariableBol); break;
                case Variable.DataType.Color: this.PaintProperty(this.spVariableCol); break;
                case Variable.DataType.Vector2: this.PaintProperty(this.spVariableVc2); break;
                case Variable.DataType.Vector3: this.PaintProperty(this.spVariableVc3); break;
                case Variable.DataType.Texture2D: this.PaintProperty(this.spVariableTxt); break;
                case Variable.DataType.Sprite: this.PaintProperty(this.spVariableSpr); break;
                case Variable.DataType.GameObject: this.PaintProperty(this.spVariableObj); break;
            }
        }

        private void PaintTags()
        {
            if (!this.UseTags()) return;
            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.fieldWidth + EditorGUIUtility.fieldWidth,
                EditorGUIUtility.singleLineHeight
            );

            Rect rectLabel = new Rect(
                rect.x,
                rect.y,
                EditorGUIUtility.labelWidth,
                rect.height
            );
            Rect rectMask = new Rect(
                rect.x + rectLabel.width,
                rect.y,
                rect.width - rectLabel.width,
                rect.height
            );


            EditorGUI.PrefixLabel(rectLabel, GUICONTENT_TAGS);
            this.spVariableTags.intValue = EditorGUI.MaskField(
                rectMask,
                this.spVariableTags.intValue,
                GlobalTagsEditor.GetTagNames()
            );
        }

        public void PaintRuntimeValue()
        {
            Variable runtime = this.GetRuntimeVariable();
            object variable = runtime == null ? null : runtime.Get();

            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.fieldWidth + EditorGUIUtility.fieldWidth,
                EditorGUIUtility.singleLineHeight
            );

            EditorGUI.LabelField(
                rect,
                "Runtime Value", 
                variable == null ? "(null)" : variable.ToString(),
                EditorStyles.boldLabel
            );
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void ChangeTypeCallback(Type variableType)
        {
            MethodInfo methodInfo = variableType.GetMethod("GetDataType");
            if (methodInfo != null)
            {
                this.spVariableType.intValue = (int)methodInfo.Invoke(null, null);
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void PaintProperty(SerializedProperty property)
        {
            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth,
                EditorGUI.GetPropertyHeight(property)
            );
            Rect rectLabel = new Rect(
                rect.x,
                rect.y,
                EditorGUIUtility.labelWidth,
                rect.height
            );
            Rect rectField = new Rect(
                rectLabel.x + rectLabel.width,
                rect.y,
                rect.width - rectLabel.width,
                rect.height
            );

            EditorGUI.PrefixLabel(rectLabel, GUICONTENT_VALUE);
            EditorGUI.PropertyField(rectField, property, GUIContent.none);
            GUILayout.Space(2f);
        }
	}
}