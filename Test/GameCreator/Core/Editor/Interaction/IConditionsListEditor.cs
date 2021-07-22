namespace GameCreator.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
    using UnityEditor.SceneManagement;

    [CustomEditor(typeof(IConditionsList), true)]
    public class IConditionsListEditor : MultiSubEditor<IConditionEditor, ICondition>
	{
		private class ItemReturnOperation
		{
			public bool removeIndex = false;
			public bool duplicateIndex = false;
            public bool copyIndex = false;
		}

        private static ICondition CLIPBOARD_ICONDITION = null;

		private const string MSG_OVERWRITE_TITLE = "There's already a Conditions component.";
		private const string MSG_OVERWRITE_DESCR = "Do you want to replace the previous Conditions game object with an empty one?";
        private const string MSG_EMPTY_CONDITIONS = "There is no Conditions attached. Add an existing component or create a new one.";
        private const string MSG_UNDEF_CONDITION_1 = "This Conditions is not set as an instance of an object.";
		private const string MSG_UNDEF_CONDITION_2 = "Check if you disabled or uninstalled a module that defined it.";

        public const string PROP_CONDITIONS = "conditions";

        public IConditionsList instance;
		public SerializedProperty spConditions;

        private Rect addConditionsButtonRect = Rect.zero;
		private EditorSortableList editorSortableList;

		// INITIALIZERS: --------------------------------------------------------------------------

		private void OnEnable()
		{
            if (target == null || serializedObject == null) return;

            this.forceInitialize = true;
            this.instance = (IConditionsList)target;
            this.spConditions = serializedObject.FindProperty(PROP_CONDITIONS);

            this.UpdateSubEditors(instance.conditions);
            this.editorSortableList = new EditorSortableList();

			if (this.target != null) this.target.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

		private void OnDisable()
		{
			this.CleanSubEditors();
			this.editorSortableList = null;
		}

        protected override void Setup (IConditionEditor editor, int editorIndex)
		{
            editor.spConditions = this.spConditions;
            editor.Setup(this.spConditions, editorIndex);
		}

        public void OnDestroyConditionsList()
        {
            for (int i = 0; i < this.instance.conditions.Length; ++i)
            {
                DestroyImmediate(this.instance.conditions[i]);
            }
        }

		// INSPECTOR: -----------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
            if (target == null || serializedObject == null) return;
			serializedObject.Update();
            this.UpdateSubEditors(this.instance.conditions);

			int removConditionIndex = -1;
			int duplicateConditionIndex = -1;
            int copyConditionIndex = -1;
			bool forceRepaint = false;
            bool conditionsCollapsed = true;

            int spConditionsSize = this.spConditions.arraySize;
			for (int i = 0; i < spConditionsSize; ++i)
			{
				bool forceSortRepaint = this.editorSortableList.CaptureSortEvents(this.handleRect[i], i);
				forceRepaint = forceSortRepaint || forceRepaint;

				GUILayout.BeginVertical();
                ItemReturnOperation returnOperation = this.PaintConditionsHeader(i);
				if (returnOperation.removeIndex) removConditionIndex = i;
				if (returnOperation.duplicateIndex) duplicateConditionIndex = i;
                if (returnOperation.copyIndex) copyConditionIndex = i;

                conditionsCollapsed &= this.isExpanded[i].target;
				using (var group = new EditorGUILayout.FadeGroupScope (this.isExpanded[i].faded))
				{
					if (group.visible)
					{
						EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
						if (this.subEditors[i] != null) this.subEditors[i].OnInspectorGUI();
						else 
						{
                            EditorGUILayout.HelpBox(MSG_UNDEF_CONDITION_1, MessageType.Warning);
                            EditorGUILayout.HelpBox(MSG_UNDEF_CONDITION_2, MessageType.None);
						}
						EditorGUILayout.EndVertical();
					}
				}

				GUILayout.EndVertical();

				if (UnityEngine.Event.current.type == EventType.Repaint)
				{
					this.objectRect[i] = GUILayoutUtility.GetLastRect();
				}

				this.editorSortableList.PaintDropPoints(this.objectRect[i], i, spConditionsSize);
			}

            Rect rectControls = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
            Rect rectAddConditions = new Rect(
                rectControls.x,
                rectControls.y,
                SelectTypePanel.WINDOW_WIDTH,
                rectControls.height
            );

            Rect rectPaste = new Rect(
                rectAddConditions.x + rectAddConditions.width,
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

            if (GUI.Button(rectAddConditions, "Add Condition", CoreGUIStyles.GetToggleButtonLeftAdd()))
			{
                SelectTypePanel selectTypePanel = new SelectTypePanel(this.AddNewCondition, "Conditions", typeof(ICondition));
				PopupWindow.Show(this.addConditionsButtonRect, selectTypePanel);
			}

			if (UnityEngine.Event.current.type == EventType.Repaint)
			{
                this.addConditionsButtonRect = rectAddConditions;
			}

            GUIContent gcPaste = ClausesUtilities.Get(ClausesUtilities.Icon.Paste);
            EditorGUI.BeginDisabledGroup(CLIPBOARD_ICONDITION == null);
            if (GUI.Button(rectPaste, gcPaste, CoreGUIStyles.GetButtonMid()))
            {
                ICondition conditionCreated = (ICondition)this.instance.gameObject.AddComponent(CLIPBOARD_ICONDITION.GetType());
                EditorUtility.CopySerialized(CLIPBOARD_ICONDITION, conditionCreated);

                this.spConditions.AddToObjectArray(conditionCreated);
                this.AddSubEditorElement(conditionCreated, -1, true);

                DestroyImmediate(CLIPBOARD_ICONDITION.gameObject, true);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
                CLIPBOARD_ICONDITION = null;
            }
            EditorGUI.EndDisabledGroup();

            GUI.Label(rectUnused, "", CoreGUIStyles.GetToggleButtonMidOn());

            GUIContent gcToggle = (conditionsCollapsed
               ? ClausesUtilities.Get(ClausesUtilities.Icon.Collapse)
               : ClausesUtilities.Get(ClausesUtilities.Icon.Expand)
            );

            EditorGUI.BeginDisabledGroup(this.instance.conditions.Length == 0);
            if (GUI.Button(rectCollapse, gcToggle, CoreGUIStyles.GetButtonMid()))
            {
                for (int i = 0; i < this.subEditors.Length; ++i)
                {
                    this.SetExpand(i, !conditionsCollapsed);
                }
            }
            EditorGUI.EndDisabledGroup();

            GUIContent gcPlay = ClausesUtilities.Get(ClausesUtilities.Icon.Play);
            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            if (GUI.Button(rectPlay, gcPlay, CoreGUIStyles.GetButtonRight()))
            {
                Debug.LogFormat(
                    "<b>Conditions Evaluation:</b> {0}",
                    this.instance.Check(this.instance.gameObject)
                );
            }
            EditorGUI.EndDisabledGroup();

			if (removConditionIndex >= 0)
			{
                ICondition source = (ICondition)this.spConditions.GetArrayElementAtIndex(removConditionIndex).objectReferenceValue;

                this.spConditions.RemoveFromObjectArrayAt(removConditionIndex);
				this.RemoveSubEditorsElement(removConditionIndex);
				DestroyImmediate(source, true);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
            }

            if (duplicateConditionIndex >= 0)
			{
				int srcIndex = duplicateConditionIndex;
				int dstIndex = duplicateConditionIndex + 1;

                ICondition source = (ICondition)this.subEditors[srcIndex].target;
                ICondition copy = (ICondition)this.instance.gameObject.AddComponent(source.GetType());

                this.spConditions.InsertArrayElementAtIndex(dstIndex);
                this.spConditions.GetArrayElementAtIndex(dstIndex).objectReferenceValue = copy;

				EditorUtility.CopySerialized(source, copy);
				this.AddSubEditorElement(copy, dstIndex, true);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
            }

            if (copyConditionIndex >= 0)
            {
                ICondition source = (ICondition)this.subEditors[copyConditionIndex].target;

                GameObject conditionInstance = new GameObject();
                conditionInstance.hideFlags = HideFlags.HideAndDontSave;

                CLIPBOARD_ICONDITION = (ICondition)conditionInstance.AddComponent(source.GetType());
                EditorUtility.CopySerialized(source, CLIPBOARD_ICONDITION);
            }

			EditorSortableList.SwapIndexes swapIndexes = this.editorSortableList.GetSortIndexes();
			if (swapIndexes != null)
			{
                this.spConditions.MoveArrayElement(swapIndexes.src, swapIndexes.dst);
				this.MoveSubEditorsElement(swapIndexes.src, swapIndexes.dst);
				
				this.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
            }

			if (EditorApplication.isPlaying) forceRepaint = true;
			if (forceRepaint) this.Repaint();
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}

		private ItemReturnOperation PaintConditionsHeader(int i)
		{
			ItemReturnOperation returnOperation = new ItemReturnOperation();

            Rect rectHeader = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
            this.PaintDragHandle(i, rectHeader);

            Texture2D conditionIcon = (i < this.subEditors.Length && this.subEditors[i] != null ?
                this.subEditors[i].GetIcon()
                : null
            );
            
            string conditionName = (this.isExpanded[i].target ? " ▾ " : " ▸ ");
            conditionName += (this.instance.conditions[i] != null 
                     ? this.instance.conditions[i].GetNodeTitle()
				: "<i>Undefined Condition</i>"
			);

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

            if (GUI.Button(rectMain, new GUIContent(conditionName, conditionIcon), style))
			{
                this.ToggleExpand(i);
			}

            GUIContent gcCopy = ClausesUtilities.Get(ClausesUtilities.Icon.Copy);
            GUIContent gcDuplicate = ClausesUtilities.Get(ClausesUtilities.Icon.Duplicate);
            GUIContent gcDelete = ClausesUtilities.Get(ClausesUtilities.Icon.Delete);

            if (this.instance.conditions[i] != null && GUI.Button(rectCopy, gcCopy, CoreGUIStyles.GetButtonMid()))
            {
                returnOperation.copyIndex = true;
            }

            if (this.instance.conditions[i] != null && GUI.Button(rectDuplicate, gcDuplicate, CoreGUIStyles.GetButtonMid()))
			{
				returnOperation.duplicateIndex = true;
			}

            if (GUI.Button(rectDelete, gcDelete, CoreGUIStyles.GetButtonRight()))
			{
				returnOperation.removeIndex = true;
			}

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

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public void AddNewCondition(Type conditionType)
		{
            ICondition conditionCreated = (ICondition)this.instance.gameObject.AddComponent(conditionType);

            this.spConditions.AddToObjectArray(conditionCreated);
			this.AddSubEditorElement(conditionCreated, -1, true);
            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
        }
	}
}
