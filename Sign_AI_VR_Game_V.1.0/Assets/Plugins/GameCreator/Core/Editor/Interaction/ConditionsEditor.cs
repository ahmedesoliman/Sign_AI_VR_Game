using System;

namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
    using UnityEditor.SceneManagement;

    [CustomEditor(typeof(Conditions))]
    public class ConditionsEditor : MultiSubEditor<ClauseEditor, Clause>
	{
        private static Clause CLIPBOARD_CLAUSE;

		private const string MSG_EMTPY_CONDITIONS = "There are no Clauses associated with this Conditions component.";
		private const string MSG_REMOVE_TITLE = "Are you sure you want to delete this Conditions Group?";
		private const string MSG_REMOVE_DESCR = "All information associated with this Conditions Group will be lost.";
		private const string MSG_PREFAB_UNSUPPORTED = "<b>Game Creator</b> does not support creating <i>Prefabs</i> from <b>Conditions</b>";

		private const string PROP_INSTANCEID = "instanceID";
		private const string PROP_CLAUSES = "clauses";
		private const string PROP_DEFREAC = "defaultActions";

        private Conditions instance;
        private SerializedProperty spClauses;
		private SerializedProperty spDefaultActions;

		private ActionsEditor actionsEditor;
		private EditorSortableList editorSortableList;

		// INITIALIZERS: --------------------------------------------------------------------------

		private void OnEnable()
		{
            if (target == null || serializedObject == null) return;
            this.forceInitialize = true;

            this.instance = (Conditions)target;
            this.spClauses = serializedObject.FindProperty(PROP_CLAUSES);
            this.spDefaultActions = serializedObject.FindProperty(PROP_DEFREAC);

            if (this.instance.defaultActions != null)
            {
                this.actionsEditor = Editor.CreateEditor(this.instance.defaultActions) as ActionsEditor;
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            serializedObject.Update();

            this.UpdateSubEditors(this.instance.clauses);
            this.editorSortableList = new EditorSortableList();
		}

		protected override void Setup(ClauseEditor editor, int editorIndex)
		{
            editor.spClauses = this.spClauses;
			editor.parentConditions = this.instance;
		}

		private void OnDisable()
		{
			if (this.actionsEditor)
			{
				DestroyImmediate(this.actionsEditor);
			}
			
			this.CleanSubEditors();
		}

		// INSPECTOR: -----------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
            if (target == null || serializedObject == null) return;

            serializedObject.Update();
			this.UpdateSubEditors(this.instance.clauses);

            this.PaintConditions();

			serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}

		private void PaintConditions()
		{
			if (this.spClauses != null && this.spClauses.arraySize > 0)
			{
                this.PaintClauses();
			}
			else
			{
                EditorGUILayout.HelpBox(MSG_EMTPY_CONDITIONS, MessageType.None);
			}

            float widthAddClause = 100f;
            Rect rectControls = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
            Rect rectAddClause = new Rect(
                rectControls.x + (rectControls.width/2.0f) - (widthAddClause + 25f)/2.0f,
                rectControls.y,
                widthAddClause,
                rectControls.height
            );

            Rect rectPaste = new Rect(
                rectAddClause.x + rectAddClause.width,
                rectControls.y,
                25f,
                rectControls.height
            );

            if (GUI.Button(rectAddClause, "Add Clause", CoreGUIStyles.GetButtonLeft()))
			{
                Clause clauseCreated = this.instance.gameObject.AddComponent<Clause>();

                int clauseCreatedIndex = this.spClauses.arraySize;
				this.spClauses.InsertArrayElementAtIndex(clauseCreatedIndex);
				this.spClauses.GetArrayElementAtIndex(clauseCreatedIndex).objectReferenceValue = clauseCreated;

				this.AddSubEditorElement(clauseCreated, -1, true);

                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
				serializedObject.Update();
			}

            GUIContent gcPaste = ClausesUtilities.Get(ClausesUtilities.Icon.Paste);
            EditorGUI.BeginDisabledGroup(CLIPBOARD_CLAUSE == null);
            if (GUI.Button(rectPaste, gcPaste, CoreGUIStyles.GetButtonRight()))
            {
                Clause copy = this.instance.gameObject.AddComponent<Clause>();
                EditorUtility.CopySerialized(CLIPBOARD_CLAUSE, copy);

                if (copy.conditionsList != null)
                {
                    IConditionsList conditionsListSource = copy.conditionsList;
                    IConditionsList conditionsListCopy = this.instance.gameObject.AddComponent<IConditionsList>();

                    EditorUtility.CopySerialized(conditionsListSource, conditionsListCopy);
                    ConditionsEditor.DuplicateIConditionList(conditionsListSource, conditionsListCopy);

                    SerializedObject soCopy = new SerializedObject(copy);
                    soCopy.FindProperty(ClauseEditor.PROP_CONDITIONSLIST).objectReferenceValue = conditionsListCopy;
                    soCopy.ApplyModifiedPropertiesWithoutUndo();
                    soCopy.Update();
                }

                int clauseIndex = this.spClauses.arraySize;
                this.spClauses.InsertArrayElementAtIndex(clauseIndex);
                this.spClauses.GetArrayElementAtIndex(clauseIndex).objectReferenceValue = copy;

                this.AddSubEditorElement(copy, -1, true);

                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();

                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
                DestroyImmediate(CLIPBOARD_CLAUSE.gameObject, true);
                CLIPBOARD_CLAUSE = null;
            }
            EditorGUI.EndDisabledGroup();

            GUIContent gcElse = ClausesUtilities.Get(ClausesUtilities.Icon.Else);
            Rect rectElse = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.boldLabel);
            EditorGUI.LabelField(rectElse, gcElse, EditorStyles.boldLabel);

			ActionsEditor.Return returnActions = ActionsEditor.PaintActionsGUI(
				this.instance.gameObject, 
				this.spDefaultActions,
				this.actionsEditor
			);

			if (returnActions != null)
			{
				this.spDefaultActions = returnActions.spParentActions;
				this.actionsEditor = returnActions.parentActionsEditor;

                serializedObject.ApplyModifiedPropertiesWithoutUndo();
				serializedObject.Update();
			}

			EditorGUILayout.Space();
		}

		private void PaintClauses()
		{
            int removeClauseIndex = -1;
            int duplicateClauseIndex = -1;
            int copyClauseIndex = -1;

			bool forceRepaint = false;
            int clauseSize = this.spClauses.arraySize;

			for (int i = 0; i < clauseSize; ++i)
			{
				if (this.subEditors == null || i >= this.subEditors.Length || this.subEditors[i] == null) continue;

				bool repaint = this.editorSortableList.CaptureSortEvents(this.subEditors[i].handleDragRect, i);
				forceRepaint = repaint || forceRepaint;

				EditorGUILayout.BeginVertical();
                Rect rectHeader = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
                this.PaintDragHandle(i, rectHeader);

				EditorGUIUtility.AddCursorRect(this.subEditors[i].handleDragRect, MouseCursor.Pan);
				string name = (this.isExpanded[i].target ? "▾ " : "▸ ") + this.instance.clauses[i].description;
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

				if (GUI.Button(rectMain, name, style))
				{
                    this.ToggleExpand(i);
				}

                GUIContent gcCopy = ClausesUtilities.Get(ClausesUtilities.Icon.Copy);
                GUIContent gcDuplicate = ClausesUtilities.Get(ClausesUtilities.Icon.Duplicate);
                GUIContent gcDelete = ClausesUtilities.Get(ClausesUtilities.Icon.Delete);

                if (GUI.Button(rectCopy, gcCopy, CoreGUIStyles.GetButtonMid()))
                {
                    copyClauseIndex = i;
                }

                if (GUI.Button(rectDuplicate, gcDuplicate, CoreGUIStyles.GetButtonMid()))
                {
                    duplicateClauseIndex = i;
                }

                if (GUI.Button(rectDelete, gcDelete, CoreGUIStyles.GetButtonRight()))
				{
					if (EditorUtility.DisplayDialog(MSG_REMOVE_TITLE, MSG_REMOVE_DESCR, "Yes", "Cancel"))
					{
						removeClauseIndex = i;
					}
				}

				using (var group = new EditorGUILayout.FadeGroupScope (this.isExpanded[i].faded))
				{
					if (group.visible)
					{
						EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                        this.subEditors[i].OnClauseGUI();
						EditorGUILayout.EndVertical();
					}
				}

				EditorGUILayout.EndVertical();
				if (UnityEngine.Event.current.type == EventType.Repaint)
				{
                    this.subEditors[i].clauseRect = GUILayoutUtility.GetLastRect();
				}

                this.editorSortableList.PaintDropPoints(this.subEditors[i].clauseRect, i, clauseSize);
			}

            if (copyClauseIndex >= 0)
            {
                Clause source = (Clause)this.subEditors[copyClauseIndex].target;
                GameObject copyInstance = EditorUtility.CreateGameObjectWithHideFlags(
                    "Clause (Copy)",
                    HideFlags.HideAndDontSave
                );

                CLIPBOARD_CLAUSE = (Clause)copyInstance.AddComponent(source.GetType());
                EditorUtility.CopySerialized(source, CLIPBOARD_CLAUSE);

                if (CLIPBOARD_CLAUSE.conditionsList != null)
                {
                    IConditionsList conditionsListSource = CLIPBOARD_CLAUSE.conditionsList;
                    IConditionsList conditionsListCopy = this.instance.gameObject.AddComponent<IConditionsList>();

                    EditorUtility.CopySerialized(conditionsListSource, conditionsListCopy);
                    ConditionsEditor.DuplicateIConditionList(conditionsListSource, conditionsListCopy);

                    SerializedObject soCopy = new SerializedObject(CLIPBOARD_CLAUSE);
                    soCopy.FindProperty(ClauseEditor.PROP_CONDITIONSLIST).objectReferenceValue = conditionsListCopy;
                    soCopy.ApplyModifiedPropertiesWithoutUndo();
                    soCopy.Update();
                }
            }

            if (duplicateClauseIndex >= 0)
            {
                int srcIndex = duplicateClauseIndex;
                int dstIndex = duplicateClauseIndex + 1;

                Clause source = (Clause)this.subEditors[srcIndex].target;
                Clause copy = (Clause)this.instance.gameObject.AddComponent(source.GetType());
                EditorUtility.CopySerialized(source, copy);

                if (copy.conditionsList != null)
                {
                    IConditionsList conditionsListSource = copy.conditionsList;
                    IConditionsList conditionsListCopy = this.instance.gameObject.AddComponent<IConditionsList>();

                    EditorUtility.CopySerialized(conditionsListSource, conditionsListCopy);
                    ConditionsEditor.DuplicateIConditionList(conditionsListSource, conditionsListCopy);

                    SerializedObject soCopy = new SerializedObject(copy);
                    soCopy.FindProperty(ClauseEditor.PROP_CONDITIONSLIST).objectReferenceValue = conditionsListCopy;

                    if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
                    soCopy.ApplyModifiedPropertiesWithoutUndo();
                    soCopy.Update();
                }

                this.spClauses.InsertArrayElementAtIndex(dstIndex);
                this.spClauses.GetArrayElementAtIndex(dstIndex).objectReferenceValue = copy;

                this.spClauses.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                this.spClauses.serializedObject.Update();

                this.AddSubEditorElement(copy, dstIndex, true);
            }

			if (removeClauseIndex >= 0)
			{
                this.subEditors[removeClauseIndex].OnDestroyClause();
                Clause rmClause = (Clause)this.spClauses
					.GetArrayElementAtIndex(removeClauseIndex).objectReferenceValue;

				this.spClauses.DeleteArrayElementAtIndex(removeClauseIndex);
				this.spClauses.RemoveFromObjectArrayAt(removeClauseIndex);
				
                DestroyImmediate(rmClause, true);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
            }

			EditorSortableList.SwapIndexes swapIndexes = this.editorSortableList.GetSortIndexes();
			if (swapIndexes != null)
			{
				this.spClauses.MoveArrayElement(swapIndexes.src, swapIndexes.dst);
				this.MoveSubEditorsElement(swapIndexes.src, swapIndexes.dst);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
            }

			if (forceRepaint) this.Repaint();
		}

        private void PaintDragHandle(int i, Rect rectHeader)
        {
            Rect rect = new Rect(
                rectHeader.x,
                rectHeader.y,
                25f,
                rectHeader.height
            );

            GUI.Label(rect, "=", CoreGUIStyles.GetButtonLeft());
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                this.subEditors[i].handleDragRect = rect;
            }
        }

        public static void DuplicateIConditionList(IConditionsList source, IConditionsList dest)
        {
            if (source == null || source.conditions == null || source.conditions.Length == 0) return;
            ICondition[] conditions = new ICondition[source.conditions.Length];

            for (int i = 0; i < source.conditions.Length; i++)
            {
                ICondition sourceAction = source.conditions[i];
                if (sourceAction == null) continue;
                conditions[i] = dest.gameObject.AddComponent(sourceAction.GetType()) as ICondition;
                EditorUtility.CopySerialized(sourceAction, conditions[i]);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(source.gameObject.scene);
            }

            dest.conditions = conditions;
        }

		// HIERARCHY CONTEXT MENU: ----------------------------------------------------------------

		[MenuItem("GameObject/Game Creator/Conditions", false, 0)]
		public static void CreateConditions()
		{
            GameObject conditionsAsset = CreateSceneObject.Create("Conditions");
            conditionsAsset.AddComponent<Conditions>();
		}
	}
}