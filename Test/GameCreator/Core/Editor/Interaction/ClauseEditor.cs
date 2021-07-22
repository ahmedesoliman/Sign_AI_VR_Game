namespace GameCreator.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;
    using UnityEditor.SceneManagement;

    [CustomEditor(typeof(Clause))]
    public class ClauseEditor : Editor
	{
		private const string MSG_EMTPY_CONDITIONS = "There are no Conditions. Create one pressing the button below.";
		private const string MSG_REMOVE_TITLE = "Are you sure you want to delete this Condition?";
		private const string MSG_REMOVE_DESCR = "This operation can't be undone.";
		private const string MSG_RM_ACTIONS_TITLE = "Do you want to remove the Actions gameObject attached to this Action?";
		private const string MSG_RM_ACTIONS_DESCR = "You can either remove the Action keeping the Actions or delete both.";
		private const string MSG_UNDEF_COND_1 = "This Condition is not set as an instance of an object.";
		private const string MSG_UNDEF_COND_2 = "Check if you disabled or uninstalled a module that defined it.";

        public const string PROP_DESCRIPTION = "description";
        public const string PROP_CONDITIONSLIST = "conditionsList";
        public const string PROP_ACTION = "actions";

        public SerializedProperty spClauses;

        public Clause instance;
        public Conditions parentConditions;
		public SerializedProperty spDescription;
        public SerializedProperty spConditionsList;
		public SerializedProperty spActions;

		private ActionsEditor actionsEditor;
        private IConditionsListEditor conditionsListEditor;
		public Rect handleDragRect;
        public Rect clauseRect;

		// INITIALIZERS: -----------------------------------------------------------------------------------------------

		private void OnEnable()
		{
            if (target == null || serializedObject == null) return;
            this.instance = (Clause)this.target;
            this.instance.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

            this.handleDragRect = Rect.zero;
            this.clauseRect = Rect.zero;

            this.spDescription = serializedObject.FindProperty(PROP_DESCRIPTION);
            this.spConditionsList = serializedObject.FindProperty(PROP_CONDITIONSLIST);
            this.spActions = serializedObject.FindProperty(PROP_ACTION);

            if (this.instance.actions != null)
            {
                this.actionsEditor = Editor.CreateEditor(this.instance.actions) as ActionsEditor;
            }

            if (this.instance.conditionsList == null)
            {
                IConditionsList cList = this.instance.gameObject.AddComponent<IConditionsList>();
                this.spConditionsList.objectReferenceValue = cList;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            this.conditionsListEditor = Editor.CreateEditor(this.instance.conditionsList) as IConditionsListEditor;
		}
		
		private void OnDisable()
		{
			if (this.actionsEditor != null)
			{
				DestroyImmediate(this.actionsEditor);
			}
			
			if (this.conditionsListEditor != null)
			{
				DestroyImmediate(this.conditionsListEditor);
			}
		}

        public void OnDestroyClause()
		{
            this.instance = (Clause)this.target;

            if (this.conditionsListEditor != null)
            {
                this.conditionsListEditor.OnDestroyConditionsList();
                DestroyImmediate(this.instance.conditionsList, true);
            }

            if (this.instance.actions != null)
            {
                int actionsID = this.instance.actions.gameObject.GetInstanceID();
                if (actionsID == this.parentConditions.gameObject.GetInstanceID()) return;

                if (EditorUtility.DisplayDialog(MSG_RM_ACTIONS_TITLE, MSG_RM_ACTIONS_DESCR, "Delete", "Keep"))
                {
                    DestroyImmediate(this.instance.actions.gameObject, true);
                }
            }
		}

		// INSPECTOR: --------------------------------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox("Can't edit a Clause from here", MessageType.Warning);
		}

        public void OnClauseGUI()
		{
            if (target == null || serializedObject == null) return;
            serializedObject.Update();
			
			EditorGUILayout.PropertyField(this.spDescription);

            GUIContent gcIf = ClausesUtilities.Get(ClausesUtilities.Icon.If);
            Rect rectIf = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.boldLabel);
            EditorGUI.LabelField(rectIf, gcIf, EditorStyles.boldLabel);

            this.conditionsListEditor.OnInspectorGUI();
			EditorGUILayout.Space();

            GUIContent gcThen = ClausesUtilities.Get(ClausesUtilities.Icon.Then);
            Rect rectThen = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.boldLabel);
            EditorGUI.LabelField(rectThen, gcThen, EditorStyles.boldLabel);

			ActionsEditor.Return returnActions = ActionsEditor.PaintActionsGUI(
				this.parentConditions.gameObject, 
				this.spActions,
				this.actionsEditor
			);

			if (returnActions != null)
			{
				this.spActions = returnActions.spParentActions;
				this.actionsEditor = returnActions.parentActionsEditor;

                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
				serializedObject.Update();
			}

			serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}
	}
}