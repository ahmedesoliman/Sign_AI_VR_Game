namespace GameCreator.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
    using UnityEditor.SceneManagement;

    [CustomEditor(typeof(Actions), true)]
	public class ActionsEditor : Editor
	{
		[System.Serializable]
		public class Return
		{
			public SerializedProperty spParentActions;
			public ActionsEditor parentActionsEditor;
		}

		private const string MSG_OVERWRITE_TITLE = "There's already a Actions component.";
		private const string MSG_OVERWRITE_DESCR = "Do you want to replace the previous Actions game object with an empty one?";
		private const string MSG_EMPTY_ACTIONS = "There is no Actions attached. Add an existing component or create a new one.";

		private static readonly GUIContent GUICONTENT_RUNINBACKGROUND = new GUIContent("Run in Background");

		private const string PROP_ACTIONSLIST = "actionsList";
		private const string PROP_RUNINBACKGROUND = "runInBackground";
		private const string PROP_DESTROYAFTERFIN = "destroyAfterFinishing";

		public Actions instance;
		public SerializedProperty spActionsList;
		public IActionsListEditor actionsListEditor;

		public SerializedProperty spRunInBackground;
		public SerializedProperty spDestroyAfterFinish;

		// INITIALIZERS: --------------------------------------------------------------------------

		private void OnEnable()
		{
            if (target == null || serializedObject == null) return;

            this.instance = (Actions)target;
            this.spActionsList = serializedObject.FindProperty(PROP_ACTIONSLIST);
            if (this.spActionsList.objectReferenceValue == null)
            {
                this.spActionsList.objectReferenceValue = this.instance.gameObject.AddComponent<IActionsList>();
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            this.spRunInBackground = serializedObject.FindProperty(PROP_RUNINBACKGROUND);
            this.spDestroyAfterFinish = serializedObject.FindProperty(PROP_DESTROYAFTERFIN);
		}

		private void OnDisable()
		{
			if (this.actionsListEditor != null)
			{
				DestroyImmediate(this.actionsListEditor);
			}
		}

		// INSPECTOR: -----------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
            if (target == null || serializedObject == null) return;
			serializedObject.Update();

			if (this.actionsListEditor == null) 
			{
				this.actionsListEditor = (IActionsListEditor)Editor.CreateEditor(
					this.spActionsList.objectReferenceValue, typeof(IActionsListEditor)
				);
			}

			if (this.actionsListEditor != null) 
			{
				this.actionsListEditor.OnInspectorGUI();
			}

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.PropertyField(this.spRunInBackground, GUICONTENT_RUNINBACKGROUND);
			EditorGUILayout.PropertyField(this.spDestroyAfterFinish);
			EditorGUILayout.EndVertical();

			serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}

		// ACTIONS CREATOR: -----------------------------------------------------------------------

		public static Return PaintActionsGUI(
			GameObject parent,
			SerializedProperty spParentActions,
			ActionsEditor parentActionsEditor)
		{
			KeyValuePair<bool, Actions> returnActions = ActionsEditor.PaintHeader(
				parent, spParentActions,parentActionsEditor
			);

			if (returnActions.Key)
			{
				spParentActions.objectReferenceValue = returnActions.Value;

				if (returnActions.Value == null) 
				{
					parentActionsEditor = null;
				}
				else 
				{
					parentActionsEditor = Editor.CreateEditor(returnActions.Value) as ActionsEditor;
					parentActionsEditor.serializedObject.Update();
					parentActionsEditor.serializedObject.ApplyModifiedPropertiesWithoutUndo();
				}

				return new Return
				{
					spParentActions = spParentActions,
					parentActionsEditor = parentActionsEditor
				};
			}

			return null;
		}

		private static KeyValuePair<bool, Actions> PaintHeader(
			GameObject parent,
			SerializedProperty spParentActions,
			ActionsEditor parentActionsEditor)
		{
			KeyValuePair<bool, Actions> returnActions = new KeyValuePair<bool, Actions>(false, null);
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Create Actions", EditorStyles.miniButton))
			{
				int option = 2;
				if (parentActionsEditor != null)
				{
					option = EditorUtility.DisplayDialogComplex(
						MSG_OVERWRITE_TITLE, 
						MSG_OVERWRITE_DESCR, 
						"Replace",
						"Cancel",
						"Keep both"
					);
				}

				if (option == 0)
				{
                    DestroyImmediate(parentActionsEditor.instance.gameObject);
					parentActionsEditor = null;
				}

				if (option != 1)
				{
					returnActions = new KeyValuePair<bool, Actions>(
						true, ActionsEditor.CreateActions(parent, "Actions")
					);
				}
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(spParentActions, GUIContent.none);
			if (EditorGUI.EndChangeCheck())
			{
				returnActions = new KeyValuePair<bool, Actions>(
					true,
					spParentActions.objectReferenceValue as Actions
				);
			}

			EditorGUILayout.EndHorizontal();

			if (parentActionsEditor == null)
			{
				EditorGUILayout.HelpBox(
					MSG_EMPTY_ACTIONS, 
					MessageType.None
				);

				return returnActions;
			}

			parentActionsEditor.OnInspectorGUI();
			return returnActions;
		}

		private static Actions CreateActions(GameObject parent, string name)
		{
			GameObject asset = new GameObject(name);
            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(asset.scene);

            if (parent != null) asset.transform.SetParent(parent.transform);
			asset.transform.localPosition = Vector3.zero;
			asset.transform.localRotation = Quaternion.identity;
			asset.transform.localScale = Vector3.one;

			Actions actions = asset.AddComponent<Actions>();
			return actions;
		}

		// HIERARCHY CONTEXT MENU: ----------------------------------------------------------------

		[MenuItem("GameObject/Game Creator/Actions", false, 0)]
		public static void CreateAction()
		{
			GameObject actions = CreateSceneObject.Create("Actions");
			actions.AddComponent<Actions>();
		}
	}
}