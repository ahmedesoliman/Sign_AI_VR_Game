namespace GameCreator.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;

    [CustomEditor(typeof(IActionsList), true)]
    public class IActionsListEditor : MultiSubEditor<IActionEditor, IAction>
    {
        private class ItemReturnOperation
        {
            public bool removeIndex = false;
            public bool duplicateIndex = false;
            public bool copyIndex = false;
        }

        private static IAction CLIPBOARD_IACTION = null;
        private static readonly Color COLOR_EXECUTING_ACTION = new Color(.66f, .82f, .96f);

        private const string MSG_OVERWRITE_TITLE = "There's already a Actions component.";
        private const string MSG_OVERWRITE_DESCR = "Do you want to replace the previous Actions game object with an empty one?";
        private const string MSG_EMPTY_ACTIONS = "There is no Actions attached. Add an existing component or create a new one.";
        private const string MSG_UNDEF_ACTION_1 = "This Action is not set as an instance of an object.";
        private const string MSG_UNDEF_ACTION_2 = "Check if you disabled or uninstalled a module that defined it.";

        private const string PROP_ACTIONS = "actions";
        private const string PROP_EXECI = "executingIndex";

        public IActionsList instance;
        public SerializedProperty spActions;
        public SerializedProperty spExecutingIndex;

        private Rect addActionsButtonRect = Rect.zero;
        private EditorSortableList editorSortableList;

        // INITIALIZERS: -----------------------------------------------------------------------------------------------

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;
            this.forceInitialize = true;

            this.instance = (IActionsList)target;
            this.spActions = serializedObject.FindProperty(PROP_ACTIONS);
            this.spExecutingIndex = serializedObject.FindProperty(PROP_EXECI);

            this.UpdateSubEditors(instance.actions);
            this.editorSortableList = new EditorSortableList();

            if (this.target != null) this.target.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }

        private void OnDisable()
        {
            this.CleanSubEditors();
            this.editorSortableList = null;
        }

        protected override void Setup(IActionEditor editor, int editorIndex)
        {
            editor.spActions = this.spActions;
            editor.Setup(this.spActions, editorIndex);
        }

        // INSPECTOR: --------------------------------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            if (target == null || serializedObject == null) return;
            serializedObject.Update();
            this.UpdateSubEditors(this.instance.actions);

            int removeActionIndex = -1;
            int duplicateActionIndex = -1;
            int copyActionIndex = -1;
            bool forceRepaint = false;
            bool actionsCollapsed = true;

            int spActionsSize = this.spActions.arraySize;
            for (int i = 0; i < spActionsSize; ++i)
            {
                bool forceSortRepaint = this.editorSortableList.CaptureSortEvents(this.handleRect[i], i);
                forceRepaint = forceSortRepaint || forceRepaint;

                GUILayout.BeginVertical();
                ItemReturnOperation returnOperation = this.PaintActionHeader(i);
                if (returnOperation.removeIndex) removeActionIndex = i;
                if (returnOperation.copyIndex) copyActionIndex = i;
                if (returnOperation.duplicateIndex) duplicateActionIndex = i;

                actionsCollapsed &= this.isExpanded[i].target;
                using (var group = new EditorGUILayout.FadeGroupScope(this.isExpanded[i].faded))
                {
                    if (group.visible)
                    {
                        EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                        if (this.subEditors[i] != null) this.subEditors[i].OnInspectorGUI();
                        else
                        {
                            EditorGUILayout.HelpBox(MSG_UNDEF_ACTION_1, MessageType.Warning);
                            EditorGUILayout.HelpBox(MSG_UNDEF_ACTION_2, MessageType.None);
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                GUILayout.EndVertical();

                if (UnityEngine.Event.current.type == EventType.Repaint)
                {
                    this.objectRect[i] = GUILayoutUtility.GetLastRect();
                }

                this.editorSortableList.PaintDropPoints(this.objectRect[i], i, spActionsSize);
            }

            Rect rectControls = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
            Rect rectAddActions = new Rect(
                rectControls.x,
                rectControls.y,
                SelectTypePanel.WINDOW_WIDTH,
                rectControls.height
            );

            Rect rectPaste = new Rect(
                rectAddActions.x + rectAddActions.width,
                rectControls.y,
                25f,
                rectControls.height
            );

            Rect rectPlay = new Rect(
                rectControls.x + rectControls.width - 25f,
                rectControls.y,
                25f,
                rectControls.height
            );

            Rect rectCollapse = new Rect(
                rectPlay.x - 25f,
                rectPlay.y,
                25f,
                rectPlay.height
            );

            Rect rectUnused = new Rect(
                rectPaste.x + rectPaste.width,
                rectControls.y,
                rectControls.width - ((rectPaste.x + rectPaste.width) - rectControls.x) - rectPlay.width - rectCollapse.width,
                rectControls.height
            );

            if (GUI.Button(rectAddActions, "Add Action", CoreGUIStyles.GetToggleButtonLeftAdd()))
            {
                SelectTypePanel selectTypePanel = new SelectTypePanel(this.AddNewAction, "Actions", typeof(IAction));
                PopupWindow.Show(this.addActionsButtonRect, selectTypePanel);
            }

            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                this.addActionsButtonRect = rectAddActions;
            }

            EditorGUI.BeginDisabledGroup(CLIPBOARD_IACTION == null);
            GUIContent gcPaste = ClausesUtilities.Get(ClausesUtilities.Icon.Paste);
            if (GUI.Button(rectPaste, gcPaste, CoreGUIStyles.GetButtonMid()))
            {
                IAction actionInstance = (IAction)this.instance.gameObject.AddComponent(CLIPBOARD_IACTION.GetType());
                EditorUtility.CopySerialized(CLIPBOARD_IACTION, actionInstance);

                this.spActions.AddToObjectArray(actionInstance);
                this.AddSubEditorElement(actionInstance, -1, true);

                DestroyImmediate(CLIPBOARD_IACTION.gameObject, true);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
                CLIPBOARD_IACTION = null;
            }
            EditorGUI.EndDisabledGroup();

            GUI.Label(rectUnused, "", CoreGUIStyles.GetToggleButtonMidOn());

            GUIContent gcToggle = (actionsCollapsed
               ? ClausesUtilities.Get(ClausesUtilities.Icon.Collapse)
               : ClausesUtilities.Get(ClausesUtilities.Icon.Expand)
            );
            
            EditorGUI.BeginDisabledGroup(this.instance.actions.Length == 0);
            if (GUI.Button(rectCollapse, gcToggle, CoreGUIStyles.GetButtonMid()))
            {
                for (int i = 0; i < this.subEditors.Length; ++i)
                {
                    this.SetExpand(i, !actionsCollapsed);
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            GUIContent gcPlay = ClausesUtilities.Get(ClausesUtilities.Icon.Play);
            if (GUI.Button(rectPlay, gcPlay, CoreGUIStyles.GetButtonRight()))
            {
                this.instance.Execute(this.instance.gameObject, () => { return; });
            }
            EditorGUI.EndDisabledGroup();

            if (removeActionIndex >= 0)
            {
                IAction source = (IAction)this.spActions.GetArrayElementAtIndex(removeActionIndex).objectReferenceValue;

                this.spActions.RemoveFromObjectArrayAt(removeActionIndex);
                this.RemoveSubEditorsElement(removeActionIndex);
                DestroyImmediate(source, true);

                this.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
            }

            if (copyActionIndex >= 0)
            {
                IAction source = (IAction)this.subEditors[copyActionIndex].target;
                GameObject clipboard = new GameObject("ActionClipboard")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };

                CLIPBOARD_IACTION = (IAction)clipboard.AddComponent(source.GetType());
                EditorUtility.CopySerialized(source, CLIPBOARD_IACTION);
            }

            if (duplicateActionIndex >= 0)
            {
                int srcIndex = duplicateActionIndex;
                int dstIndex = duplicateActionIndex + 1;

                IAction source = (IAction)this.subEditors[srcIndex].target;
                IAction copy = (IAction)this.instance.gameObject.AddComponent(source.GetType());

                this.spActions.InsertArrayElementAtIndex(dstIndex);
                this.spActions.GetArrayElementAtIndex(dstIndex).objectReferenceValue = copy;

                EditorUtility.CopySerialized(source, copy);
                this.AddSubEditorElement(copy, dstIndex, true);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
            }

            EditorSortableList.SwapIndexes swapIndexes = this.editorSortableList.GetSortIndexes();
            if (swapIndexes != null)
            {
                this.spActions.MoveArrayElement(swapIndexes.src, swapIndexes.dst);
                this.MoveSubEditorsElement(swapIndexes.src, swapIndexes.dst);
                
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
            }

            if (EditorApplication.isPlaying) forceRepaint = true;
            if (forceRepaint) this.Repaint();
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private ItemReturnOperation PaintActionHeader(int i)
        {
            ItemReturnOperation returnOperation = new ItemReturnOperation();

            Color defaultBackgroundColor = GUI.backgroundColor;
            if (this.spExecutingIndex.intValue == i) GUI.backgroundColor = COLOR_EXECUTING_ACTION;

            Rect rectHeader = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
            this.PaintDragHandle(i, rectHeader);

            Texture2D icon = (i < this.subEditors.Length && this.subEditors[i] != null ?
                this.subEditors[i].GetIcon()
                : null
            );
            
            string name = (this.isExpanded[i].target ? " ▾ " : " ▸ ");
            name += (this.instance.actions[i] != null
                ? this.instance.actions[i].GetNodeTitle()
                : "<i>Undefined Action</i>"
            );

            GUIStyle style = (this.isExpanded[i].target
                ? CoreGUIStyles.GetToggleButtonMidOn()
                : CoreGUIStyles.GetToggleButtonMidOff()
            );

            float opacity = 1.0f;
            if (this.subEditors[i] != null && this.subEditors[i].action != null)
            {
                opacity = this.subEditors[i].action.GetOpacity();
            }

            Color tempColor = style.normal.textColor;
            style.normal.textColor = new Color(
                style.normal.textColor.r,
                style.normal.textColor.g,
                style.normal.textColor.b,
                opacity
            );

            Rect rectDelete = new Rect(
                rectHeader.x + rectHeader.width - 25f,
                rectHeader.y,
                25f,
                rectHeader.height
            );

            Rect rectDuplicate = new Rect(
                rectDelete.x - 25f,
                rectHeader.y,
                25f,
                rectHeader.height
            );

            Rect rectCopy = new Rect(
                rectDuplicate.x - 25f,
                rectHeader.y,
                25f,
                rectHeader.height
            );

            Rect rectMain = new Rect(
                rectHeader.x + 25f,
                rectHeader.y,
                rectHeader.width - (25f * 4f),
                rectHeader.height
            );

            if (GUI.Button(rectMain, new GUIContent(name, icon), style))
            {
                this.ToggleExpand(i);
            }

            style.normal.textColor = tempColor;

            GUIContent gcCopy = ClausesUtilities.Get(ClausesUtilities.Icon.Copy);
            GUIContent gcDuplicate = ClausesUtilities.Get(ClausesUtilities.Icon.Duplicate);
            GUIContent gcDelete = ClausesUtilities.Get(ClausesUtilities.Icon.Delete);

            if (this.instance.actions[i] != null && GUI.Button(rectCopy, gcCopy, CoreGUIStyles.GetButtonMid()))
            {
                returnOperation.copyIndex = true;
            }

            if (this.instance.actions[i] != null && GUI.Button(rectDuplicate, gcDuplicate, CoreGUIStyles.GetButtonMid()))
            {
                returnOperation.duplicateIndex = true;
            }

            if (GUI.Button(rectDelete, gcDelete, CoreGUIStyles.GetButtonRight()))
            {
                returnOperation.removeIndex = true;
            }

            GUI.backgroundColor = defaultBackgroundColor;

            return returnOperation;
        }

        private void PaintDragHandle(int i, Rect rectHeader)
        {
            Rect handle = new Rect(rectHeader.x, rectHeader.y, 25f, rectHeader.height);
            GUI.Label(handle, "=", CoreGUIStyles.GetButtonLeft());

            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                this.handleRect[i] = handle;
            }

            EditorGUIUtility.AddCursorRect(this.handleRect[i], MouseCursor.Pan);
        }

        // PUBLIC METHODS: ---------------------------------------------------------------------------------------------

        public void AddNewAction(Type actionType)
        {
            IAction actionCreated = (IAction)this.instance.gameObject.AddComponent(actionType);

            this.spActions.AddToObjectArray(actionCreated);
            this.AddSubEditorElement(actionCreated, -1, true);
            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
        }
    }
}