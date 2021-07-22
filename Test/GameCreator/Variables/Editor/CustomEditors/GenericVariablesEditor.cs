namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;

    public abstract class GenericVariablesEditor<TEditor, TTarget> : MultiSubEditor<TEditor, TTarget>
        where TEditor : Editor
        where TTarget : Object
    {
        private class ItemReturnOperation
        {
            public bool removeIndex = false;
        }

        private const float TAG_PADDING = 1f;
        private const float TAG_WIDTH = 60f;

        private const string PROP_REFERENCES = "references";

        private const string CREATEVAR_PLACEHOLDER = "(New Variable Name)";
        private const string CREATEVAR_CONTROL_ID = "create-variable-text";

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool enableEditor = false;
        private GUIStyle styleCreateVarText;
        private GUIStyle styleCreateVarButton;
        private GUIStyle styleCreateVarPlaceholder;

        private EditorSortableList editorSortableList;
        protected SerializedProperty spReferences;

        protected string search = "";
        protected int tagsMask = ~0;
        private string createVarInputText = "";

        // INITIALIZERS: --------------------------------------------------------------------------

        protected virtual void OnEnable()
        {
            this.enableEditor = true;
            this.forceInitialize = true;
        }

        protected virtual void OnDisable()
        {
            this.CleanSubEditors();
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract TTarget[] GetReferences();
        protected abstract string GetReferenceName(int index);
        protected abstract Variable.DataType GetReferenceType(int index);
        protected abstract bool MatchSearch(int index, string search, int tagsMask);
        protected abstract TTarget CreateReferenceInstance(string name);
        protected abstract void DeleteReferenceInstance(int index);
        protected abstract Tag[] GetReferenceTags(int index);

        protected virtual void BeforePaintSubEditor(int index) {}
        protected virtual void AfterPaintSubEditorsList() { }

        // PAINT METHODS: -------------------------------------------------------------------------

        public void OnInspectorGUI(string search, int tagsMask)
        {
            this.search = search;
            this.tagsMask = tagsMask;
            bool usingSearch = !string.IsNullOrEmpty(this.search);
            this.PaintInspector(usingSearch);
        }

        public override void OnInspectorGUI()
        {
            this.PaintInspector(false);
        }

        private void PaintInspector(bool usingSearch)
        {
            if (this.enableEditor)
            {
                this.enableEditor = false;
                this.spReferences = serializedObject.FindProperty("references");

                this.UpdateSubEditors(this.GetReferences());
                this.editorSortableList = new EditorSortableList();
                this.InitializeStyles();
            }

            serializedObject.Update();
            this.UpdateSubEditors(this.GetReferences());

            this.PaintCreateVariable(usingSearch);
            this.PaintVariablesList(usingSearch);
            this.AfterPaintSubEditorsList();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void PaintCreateVariable(bool usingSearch)
        {
            if (usingSearch) return;

            EditorGUILayout.Space();
            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth,
                EditorGUIUtility.singleLineHeight
            );
            Rect rectText = new Rect(rect.x, rect.y, rect.width - 25f, rect.height);
            Rect rectCreate = new Rect(
                rectText.x + rectText.width,
                rectText.y,
                25f,
                rectText.height
            );

            GUI.SetNextControlName(CREATEVAR_CONTROL_ID);
            string textResult = EditorGUI.TextField(
                rectText,
                (string.IsNullOrEmpty(this.createVarInputText) ? CREATEVAR_PLACEHOLDER : this.createVarInputText),
                (string.IsNullOrEmpty(this.createVarInputText) ? this.styleCreateVarPlaceholder : this.styleCreateVarText)
            );

            if (textResult == CREATEVAR_PLACEHOLDER) this.createVarInputText = "";
            else this.createVarInputText = textResult;

            bool pressEnter = (
                Event.current.isKey &&
                Event.current.keyCode == KeyCode.Return && 
                GUI.GetNameOfFocusedControl() == CREATEVAR_CONTROL_ID
            );

            if (GUI.Button(rectCreate, "+", this.styleCreateVarButton) || pressEnter)
            {
                this.CreateVariable(this.createVarInputText);
                Event.current.Use();
            }
        }

        private void PaintVariablesList(bool usingSearch)
        {
            int removeReferenceIndex = -1;
            bool forceRepaint = false;

            int spReferencesSize = this.spReferences.arraySize;
            for (int i = 0; i < spReferencesSize; ++i)
            {
                if (usingSearch)
                {
                    if (!this.MatchSearch(i, this.search, this.tagsMask)) continue;
                }
                else
                {
                    bool forceSortRepaint = this.editorSortableList.CaptureSortEvents(this.handleRect[i], i);
                    forceRepaint = forceSortRepaint || forceRepaint;
                }

                EditorGUILayout.BeginVertical();
                ItemReturnOperation returnOperation = this.PaintReferenceHeader(i, usingSearch);
                if (returnOperation.removeIndex) removeReferenceIndex = i;

                using (var group = new EditorGUILayout.FadeGroupScope(this.isExpanded[i].faded))
                {
                    if (group.visible)
                    {
                        EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                        if (this.subEditors[i] != null)
                        {
                            this.BeforePaintSubEditor(i);
                            this.subEditors[i].OnInspectorGUI();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.EndVertical();

                if (Event.current.type == EventType.Repaint)
                {
                    this.objectRect[i] = GUILayoutUtility.GetLastRect();
                }

                this.editorSortableList.PaintDropPoints(this.objectRect[i], i, spReferencesSize);
            }

            EditorGUILayout.Space();

            if (removeReferenceIndex >= 0)
            {
                this.DeleteReferenceInstance(removeReferenceIndex);
            }

            EditorSortableList.SwapIndexes swapIndexes = this.editorSortableList.GetSortIndexes();
            if (swapIndexes != null)
            {
                this.spReferences.MoveArrayElement(swapIndexes.src, swapIndexes.dst);
                this.MoveSubEditorsElement(swapIndexes.src, swapIndexes.dst);
            }

            if (EditorApplication.isPlaying) forceRepaint = true;
            if (forceRepaint) this.Repaint();
        }

        private ItemReturnOperation PaintReferenceHeader(int i, bool usingSearch)
        {
            ItemReturnOperation returnOperation = new ItemReturnOperation();

            Rect rectHeader = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
            if (!usingSearch) this.PaintDragHandle(i, rectHeader);

            string variableName = (this.isExpanded[i].target ? " ▾ " : " ▸ ");
            variableName += this.GetReferenceName(i);

            Texture2D variableIcon = VariableEditorUtils.GetIcon(this.GetReferenceType(i));

            GUIStyle style = (this.isExpanded[i].target
                ? CoreGUIStyles.GetToggleButtonMidOn()
                : CoreGUIStyles.GetToggleButtonMidOff()
            );

            Rect rectDelete = new Rect(
                rectHeader.x + rectHeader.width - 25f,
                rectHeader.y,
                25f,
                rectHeader.height
            );

            Rect rectMain = new Rect(
                rectHeader.x + 25f,
                rectHeader.y,
                rectHeader.width - (25f * 2f),
                rectHeader.height
            );

            if (usingSearch)
            {
                style = (this.isExpanded[i].target
                    ? CoreGUIStyles.GetToggleButtonLeftOn()
                    : CoreGUIStyles.GetToggleButtonLeftOff()
                );

                rectMain = new Rect(
                    rectHeader.x,
                    rectHeader.y,
                    rectHeader.width - 25f,
                    rectHeader.height
                );
            }

            if (GUI.Button(rectMain, new GUIContent(variableName, variableIcon), style))
            {
                this.ToggleExpand(i);
            }

            GUIContent gcDelete = ClausesUtilities.Get(ClausesUtilities.Icon.Delete);
            if (GUI.Button(rectDelete, gcDelete, CoreGUIStyles.GetButtonRight()))
            {
                returnOperation.removeIndex = true;
            }

            this.PaintTags(i);
            return returnOperation;
        }

        private void PaintDragHandle(int i, Rect rectHeader)
        {
            Rect handle = new Rect(rectHeader.x, rectHeader.y, 25f, rectHeader.height);
            GUI.Label(handle, "=", CoreGUIStyles.GetButtonLeft());

            if (Event.current.type == EventType.Repaint)
            {
                this.handleRect[i] = handle;
            }

            EditorGUIUtility.AddCursorRect(this.handleRect[i], MouseCursor.Pan);
        }

        private void PaintTags(int index)
        {
            Rect rectSpace = this.objectRect[index];
            rectSpace = new Rect(
                rectSpace.x, 
                rectSpace.y + TAG_PADDING, 
                rectSpace.width - 25f,
                20f - (TAG_PADDING * 2f)
            );

            Tag[] tags = this.GetReferenceTags(index);
            for (int i = 0; i < tags.Length; ++i)
            {
                if (string.IsNullOrEmpty(tags[i].name)) continue;

                Rect rect = new Rect(
                    rectSpace.x + rectSpace.width - TAG_WIDTH,
                    rectSpace.y,
                    TAG_WIDTH,
                    rectSpace.height
                );

                rectSpace = new Rect(
                    rectSpace.x,
                    rectSpace.y,
                    rectSpace.width - rect.width,
                    rectSpace.height
                );

                Color tempColor = GUI.backgroundColor;
                GUI.backgroundColor = tags[i].GetColor();
                EditorGUI.LabelField(rect, tags[i].name, CoreGUIStyles.GetLabelTag());
                GUI.backgroundColor = tempColor;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void InitializeStyles()
        {
            float height = 18f;
            RectOffset margin = new RectOffset(0, 0, 2, 2);
            RectOffset padding = new RectOffset(5, 5, 0, 0);

            this.styleCreateVarText = new GUIStyle(EditorStyles.textField);
            this.styleCreateVarText.alignment = TextAnchor.MiddleRight;
            this.styleCreateVarText.margin = margin;
            this.styleCreateVarText.padding = padding;
            this.styleCreateVarText.fixedHeight = height;

            this.styleCreateVarPlaceholder = new GUIStyle(this.styleCreateVarText);
            this.styleCreateVarPlaceholder.fontStyle = FontStyle.Italic;
            this.styleCreateVarPlaceholder.normal.textColor = new Color(
                this.styleCreateVarPlaceholder.normal.textColor.r,
                this.styleCreateVarPlaceholder.normal.textColor.g,
                this.styleCreateVarPlaceholder.normal.textColor.b,
                0.5f
            );

            this.styleCreateVarButton = new GUIStyle(CoreGUIStyles.GetButtonRight());
            this.styleCreateVarButton.margin = margin;
            this.styleCreateVarButton.padding = new RectOffset(0, 0, 0, 0);
            this.styleCreateVarButton.fixedHeight = height;
        }

        protected TTarget CreateVariable(string variableName)
        {
            variableName = VariableEditor.ProcessName(variableName);

            EditorGUI.FocusTextInControl(null);
            GUIUtility.keyboardControl = 0;

            TTarget instance = this.CreateReferenceInstance(variableName);
            this.spReferences.AddToObjectArray<TTarget>(instance);
            this.AddSubEditorElement(instance, -1, true);

            return instance;
        }
    }
}