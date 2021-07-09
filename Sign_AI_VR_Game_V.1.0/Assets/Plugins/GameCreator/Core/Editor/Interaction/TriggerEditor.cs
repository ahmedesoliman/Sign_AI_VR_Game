namespace GameCreator.Core
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEngine;
	using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEditorInternal;

	[CustomEditor(typeof(Trigger))]
	public class TriggerEditor : Editor
	{
        private const string MSG_REQUIRE_HAVE_COLLIDER = "This type of Trigger requires a Collider. Select one from below";

		private const BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
		private const float DOTTED_LINES_SIZE = 2.0f;
		private const string KEY_IGNITER_INDEX_PREF = "gamecreator-igniters-index";

        private const string ICONS_PATH = "Assets/Plugins/GameCreator/Extra/Icons/Trigger/{0}";
        private const float ITEMS_TOOLBAR_WIDTH = 25f;

        private const string PROP_OPTION = "option";
        private const string PROP_ACTIONS = "actions";
        private const string PROP_CONDITIONS = "conditions";

		private static readonly Type[] COLLIDER_TYPES = new Type[]
		{
			typeof(SphereCollider),
			typeof(BoxCollider),
			typeof(CapsuleCollider),
			typeof(MeshCollider)
		};

		private class IgniterCache
		{
			public GUIContent name;
			public string comment;
			public bool requiresCollider;
			public SerializedObject serializedObject;

			public IgniterCache(UnityEngine.Object reference)
			{
				if (reference == null)
				{
					this.name = new GUIContent("Undefined");
					this.requiresCollider = false;
					this.serializedObject = null;
					return;
				}

				string igniterName = (string)reference.GetType().GetField("NAME", BINDING_FLAGS).GetValue(null);
				string iconPath = (string)reference.GetType().GetField("ICON_PATH", BINDING_FLAGS).GetValue(null);

                if (!string.IsNullOrEmpty(igniterName))
                {
                    string[] igniterNameSplit = igniterName.Split(new char[]{'/'});
                    igniterName = igniterNameSplit[igniterNameSplit.Length - 1];
                }

				Texture2D igniterIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(iconPath, igniterName + ".png"));
				if (igniterIcon == null) igniterIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath + "Default.png");
				if (igniterIcon == null) igniterIcon = EditorGUIUtility.FindTexture("GameObject Icon");

				this.name = new GUIContent(" " + igniterName, igniterIcon);
				this.comment = (string)reference.GetType().GetField("COMMENT", BINDING_FLAGS).GetValue(null);
				this.requiresCollider = (bool)reference.GetType().GetField("REQUIRES_COLLIDER", BINDING_FLAGS).GetValue(null);
				this.serializedObject = new SerializedObject(reference);
			}
		}

		private static string[] IGNITERS_PLATFORM_NAMES = new string[0];

        private static GUIContent GC_ACTIONS;
        private static GUIContent GC_CONDITIONS;
        private static GUIContent GC_SETTINGS;
        private static GUIContent GC_HOTSPOT;

		// PROPERTIES: ----------------------------------------------------------------------------

		private Trigger trigger;

		private int ignitersIndex = 0;
		private SerializedProperty spIgnitersKeys;
		private SerializedProperty spIgnitersValues;
		private IgniterCache[] ignitersCache;
		private bool updateIgnitersPlatforms = false;
		private Rect selectIgniterButtonRect = Rect.zero;

		private SerializedProperty spTrigger;
		private SerializedProperty spTriggerKeyCode;

        private SerializedProperty spItems;
        private EditorSortableList sortableList;

		private bool foldoutAdvancedSettings = false;
		private SerializedProperty spMinDistance;
		private SerializedProperty spMinDistanceToPlayer;

		// INITIALIZERS: -----------------------------------------------------------------------------------------------

		private void OnEnable()
		{
            if (target == null || serializedObject == null) return;
			this.trigger = (Trigger)target;

			SerializedProperty spIgniters = serializedObject.FindProperty("igniters");
			this.spIgnitersKeys = spIgniters.FindPropertyRelative("keys");
			this.spIgnitersValues = spIgniters.FindPropertyRelative("values");

			if (this.spIgnitersKeys.arraySize == 0)
			{
				Igniter igniter = this.trigger.gameObject.AddComponent<IgniterStart>();
				igniter.Setup(this.trigger);
				igniter.enabled = false;

				this.spIgnitersKeys.InsertArrayElementAtIndex(0);
				this.spIgnitersValues.InsertArrayElementAtIndex(0);

				this.spIgnitersKeys.GetArrayElementAtIndex(0).intValue = Trigger.ALL_PLATFORMS_KEY;
				this.spIgnitersValues.GetArrayElementAtIndex(0).objectReferenceValue = igniter;

				this.serializedObject.ApplyModifiedPropertiesWithoutUndo();
				this.serializedObject.Update();
			}

			this.UpdateIgnitersPlatforms();

			this.ignitersIndex = EditorPrefs.GetInt(KEY_IGNITER_INDEX_PREF, 0);
			if (this.ignitersIndex >= this.spIgnitersKeys.arraySize)
			{
				this.ignitersIndex = this.spIgnitersKeys.arraySize - 1;
				EditorPrefs.SetInt(KEY_IGNITER_INDEX_PREF, this.ignitersIndex);
			}

            this.spItems = serializedObject.FindProperty("items");
            this.sortableList = new EditorSortableList();

			this.spMinDistance = serializedObject.FindProperty("minDistance");
			this.spMinDistanceToPlayer = serializedObject.FindProperty("minDistanceToPlayer");
		}

		// INSPECTOR: --------------------------------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
            if (target == null || serializedObject == null) return;
			serializedObject.Update();

			if (this.updateIgnitersPlatforms)
			{
				this.UpdateIgnitersPlatforms();
				this.updateIgnitersPlatforms = false;
			}

            if (GC_ACTIONS == null || GC_CONDITIONS == null || GC_HOTSPOT == null || GC_SETTINGS == null)
            {
                GC_ACTIONS = new GUIContent(
                    AssetDatabase.LoadAssetAtPath<Texture2D>(string.Format(ICONS_PATH, "actions.png")),
                    "Create an Actions slot"
                );
                GC_CONDITIONS = new GUIContent(
                    AssetDatabase.LoadAssetAtPath<Texture2D>(string.Format(ICONS_PATH, "conditions.png")),
                    "Create a Conditions slot"
                );
                GC_HOTSPOT = new GUIContent(
                    AssetDatabase.LoadAssetAtPath<Texture2D>(string.Format(ICONS_PATH, "hotspot.png")),
                    "Add a Hotspot"
                );
                GC_SETTINGS = new GUIContent(
                    AssetDatabase.LoadAssetAtPath<Texture2D>(string.Format(ICONS_PATH, "settings.png")),
                    "Open/Close Advanced Settings"
                );
            }

			this.DoLayoutConfigurationOptions();

            this.PaintItemsToolbar();
            this.PaintItems();

			EditorGUILayout.Space();
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}

		private void PaintAdvancedSettings()
		{
            EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(this.spMinDistance);
            EditorGUI.BeginDisabledGroup(!this.spMinDistance.boolValue);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spMinDistanceToPlayer);
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel--;
		}

		private void DoLayoutConfigurationOptions()
		{
			int removeIndex = -1;

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.BeginHorizontal();

            int ignIndex = GUILayout.Toolbar(this.ignitersIndex, IGNITERS_PLATFORM_NAMES);
			if (ignIndex != this.ignitersIndex)
			{
				this.ignitersIndex = ignIndex;
				EditorPrefs.SetInt(KEY_IGNITER_INDEX_PREF, this.ignitersIndex);
			}

			if (GUILayout.Button("+", CoreGUIStyles.GetButtonLeft(), GUILayout.Width(25f)))
			{
				this.SelectPlatformMenu();
			}

			EditorGUI.BeginDisabledGroup(this.ignitersIndex == 0);
            if (GUILayout.Button("-", CoreGUIStyles.GetButtonMid(), GUILayout.Width(25f)))
			{
				removeIndex = this.ignitersIndex;
			}
			EditorGUI.EndDisabledGroup();

            GUIStyle settingStyle = (this.foldoutAdvancedSettings
                ? CoreGUIStyles.GetToggleButtonRightOn()
                : CoreGUIStyles.GetToggleButtonRightOff()
            );
            
            if (GUILayout.Button(GC_SETTINGS, settingStyle, GUILayout.Width(25f), GUILayout.Height(18f)))
            {
                this.foldoutAdvancedSettings = !this.foldoutAdvancedSettings;
            }

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.PrefixLabel(this.ignitersCache[this.ignitersIndex].name, EditorStyles.miniBoldLabel);
			GUILayout.FlexibleSpace();

            if (GUILayout.Button("Change Trigger", GUILayout.Width(SelectTypePanel.WINDOW_WIDTH)))
			{
				SelectTypePanel selectTypePanel = new SelectTypePanel(this.SelectNewIgniter, "Triggers", typeof(Igniter));
				PopupWindow.Show(this.selectIgniterButtonRect, selectTypePanel);
			}

			if (UnityEngine.Event.current.type == EventType.Repaint)
			{
				this.selectIgniterButtonRect = GUILayoutUtility.GetLastRect();
			}

			EditorGUILayout.EndHorizontal();

			if (this.ignitersCache[this.ignitersIndex].serializedObject != null)
			{
				string comment = this.ignitersCache[this.ignitersIndex].comment;
				if (!string.IsNullOrEmpty(comment)) EditorGUILayout.HelpBox(comment, MessageType.Info);

				Igniter.PaintEditor(this.ignitersCache[this.ignitersIndex].serializedObject);
			}

			if (this.ignitersCache[this.ignitersIndex].requiresCollider)
			{
				Collider collider = this.trigger.GetComponent<Collider>();
				if (!collider) this.PaintNoCollider();
			}

            if (this.foldoutAdvancedSettings)
            {
                EditorGUILayout.Space();
                this.PaintAdvancedSettings();
            }

			EditorGUILayout.EndVertical();

			if (removeIndex > 0)
			{
				UnityEngine.Object obj = this.spIgnitersValues.GetArrayElementAtIndex(removeIndex).objectReferenceValue;
				this.spIgnitersValues.GetArrayElementAtIndex(removeIndex).objectReferenceValue = null;

				this.spIgnitersKeys.DeleteArrayElementAtIndex(removeIndex);
				this.spIgnitersValues.DeleteArrayElementAtIndex(removeIndex);

				if (obj != null) DestroyImmediate(obj, true);

                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.trigger.gameObject.scene);
                this.serializedObject.ApplyModifiedPropertiesWithoutUndo();
				this.serializedObject.Update();

				this.updateIgnitersPlatforms = true;
				if (this.ignitersIndex >= this.spIgnitersKeys.arraySize)
                {
                    this.ignitersIndex = this.spIgnitersKeys.arraySize - 1;
                }
			}
		}

		private void SelectPlatformCallback(object data)
		{
			if (this.trigger.igniters.ContainsKey((int)data)) return;

			int index = this.spIgnitersKeys.arraySize;
			this.spIgnitersKeys.InsertArrayElementAtIndex(index);
			this.spIgnitersValues.InsertArrayElementAtIndex(index);

			this.spIgnitersKeys.GetArrayElementAtIndex(index).intValue = (int)data;

			Igniter igniter = this.trigger.gameObject.AddComponent<IgniterStart>();
			igniter.Setup(this.trigger);
			igniter.enabled = false;

			this.spIgnitersValues.GetArrayElementAtIndex(index).objectReferenceValue = igniter;

			this.ignitersIndex = index;
			EditorPrefs.SetInt(KEY_IGNITER_INDEX_PREF, this.ignitersIndex);

            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.trigger.gameObject.scene);
            this.serializedObject.ApplyModifiedPropertiesWithoutUndo();
			this.serializedObject.Update();

			this.updateIgnitersPlatforms = true;
		}

		private void SelectPlatformMenu()
		{
			GenericMenu menu = new GenericMenu();

			foreach(Trigger.Platforms platform in Enum.GetValues(typeof(Trigger.Platforms)))
			{
				bool disabled = this.trigger.igniters.ContainsKey((int)platform);
				menu.AddItem(new GUIContent(platform.ToString()), disabled, this.SelectPlatformCallback, (int)platform);	
			}

			menu.ShowAsContext();
		}

        private void PaintItemsToolbar()
        {
            Rect rectItem = GUILayoutUtility.GetRect(
                GUIContent.none, CoreGUIStyles.GetToggleButtonOff()
            );

            Rect rectItem1 = new Rect(
                rectItem.x,
                rectItem.y,
                ITEMS_TOOLBAR_WIDTH,
                rectItem.height
            );
            Rect rectItem2 = new Rect(
                rectItem1.x + rectItem1.width,
                rectItem1.y,
                rectItem1.width,
                rectItem1.height
            );
            Rect rectItem3 = new Rect(
                rectItem2.x + rectItem2.width,
                rectItem2.y,
                rectItem2.width,
                rectItem2.height
            );
            Rect rectItemH = new Rect(
                //rectItem.x + (rectItem.width - ITEMS_TOOLBAR_WIDTH),
                rectItem3.x + rectItem3.width + 5f,
                rectItem2.y,
                ITEMS_TOOLBAR_WIDTH,
                rectItem2.height
            );

            if (GUI.Button(rectItem1, GC_ACTIONS, CoreGUIStyles.GetButtonLeft()))
            {
                int index = this.spItems.arraySize;
                this.spItems.InsertArrayElementAtIndex(index);

                SerializedProperty spItem = this.spItems.GetArrayElementAtIndex(index);
                spItem.FindPropertyRelative(PROP_OPTION).intValue = (int)Trigger.ItemOpts.Actions;
                spItem.FindPropertyRelative(PROP_ACTIONS).objectReferenceValue = this.CreateSubObject<Actions>();
                spItem.FindPropertyRelative(PROP_CONDITIONS).objectReferenceValue = null;
            }

            if (GUI.Button(rectItem2, GC_CONDITIONS, CoreGUIStyles.GetButtonMid()))
            {
                int index = this.spItems.arraySize;
                this.spItems.InsertArrayElementAtIndex(index);

                SerializedProperty spItem = this.spItems.GetArrayElementAtIndex(index);
                spItem.FindPropertyRelative(PROP_OPTION).intValue = (int)Trigger.ItemOpts.Conditions;
                spItem.FindPropertyRelative(PROP_ACTIONS).objectReferenceValue = null;
                spItem.FindPropertyRelative(PROP_CONDITIONS).objectReferenceValue = this.CreateSubObject<Conditions>();
            }

            if (GUI.Button(rectItem3, "+", CoreGUIStyles.GetButtonRight()))
            {
                int index = this.spItems.arraySize;
                this.spItems.InsertArrayElementAtIndex(index);

                SerializedProperty spItem = this.spItems.GetArrayElementAtIndex(index);
                spItem.FindPropertyRelative(PROP_OPTION).intValue = (int)Trigger.ItemOpts.Actions;
                spItem.FindPropertyRelative(PROP_ACTIONS).objectReferenceValue = null;
                spItem.FindPropertyRelative(PROP_CONDITIONS).objectReferenceValue = null;
            }

            EditorGUI.BeginDisabledGroup(this.trigger.gameObject.GetComponent<Hotspot>() != null);
            if (GUI.Button(rectItemH, GC_HOTSPOT))
            {
                Undo.AddComponent<Hotspot>(this.trigger.gameObject);
            }
            EditorGUI.EndDisabledGroup();
        }

        private void PaintItems()
        {
            int itemsCount = this.spItems.arraySize;
            int removeIndex = -1;
            bool forceRepaint = false;

            GUIContent gcDelete = ClausesUtilities.Get(ClausesUtilities.Icon.Delete);

            for (int i = 0; i < itemsCount; ++i)
            {
                SerializedProperty spItem = this.spItems.GetArrayElementAtIndex(i);
                SerializedProperty spIOption = spItem.FindPropertyRelative(PROP_OPTION);
                SerializedProperty spIActions = spItem.FindPropertyRelative(PROP_ACTIONS);
                SerializedProperty spIConditions = spItem.FindPropertyRelative(PROP_CONDITIONS);

                Rect rectItem = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOff());

                Rect rectHandle = new Rect(
                    rectItem.x,
                    rectItem.y,
                    25f,
                    rectItem.height
                );

                Rect rectToggle = new Rect(
                    rectHandle.x + rectHandle.width,
                    rectHandle.y,
                    25f,
                    rectHandle.height
                );

                Rect rectDelete = new Rect(
                    rectItem.x + (rectItem.width - 25f),
                    rectToggle.y,
                    25f,
                    rectToggle.height
                );

                Rect rectCont = new Rect(
                    rectToggle.x + rectToggle.width,
                    rectToggle.y,
                    rectItem.width - (rectHandle.width + rectToggle.width + rectDelete.width),
                    rectToggle.height
                );

                GUI.Label(rectHandle, "=", CoreGUIStyles.GetButtonLeft());
                bool forceSortRepaint = this.sortableList.CaptureSortEvents(rectHandle, i);
                forceRepaint = forceSortRepaint || forceRepaint;

                EditorGUIUtility.AddCursorRect(rectHandle, MouseCursor.Pan);

                GUIContent gcToggle = null;
                if (spIOption.intValue == (int)Trigger.ItemOpts.Actions) gcToggle = GC_ACTIONS;
                if (spIOption.intValue == (int)Trigger.ItemOpts.Conditions) gcToggle = GC_CONDITIONS;

                if (GUI.Button(rectToggle, gcToggle, CoreGUIStyles.GetButtonMid()))
                {
                    switch (spIOption.intValue)
                    {
                        case (int)Trigger.ItemOpts.Actions:
                            spIOption.intValue = (int)Trigger.ItemOpts.Conditions;
                            break;

                        case (int)Trigger.ItemOpts.Conditions:
                            spIOption.intValue = (int)Trigger.ItemOpts.Actions;
                            break;
                    }
                }

                GUI.Label(rectCont, string.Empty, CoreGUIStyles.GetButtonMid());
                Rect rectField = new Rect(
                    rectCont.x + 2f,
                    rectCont.y + (rectCont.height/2f - EditorGUIUtility.singleLineHeight/2f),
                    rectCont.width - 7f,
                    EditorGUIUtility.singleLineHeight
                );

                switch (spIOption.intValue)
                {
                    case (int)Trigger.ItemOpts.Actions :
                        EditorGUI.PropertyField(rectField, spIActions, GUIContent.none, true);
                        break;

                    case (int)Trigger.ItemOpts.Conditions:
                        EditorGUI.PropertyField(rectField, spIConditions, GUIContent.none, true);
                        break;
                }


                if (GUI.Button(rectDelete, gcDelete, CoreGUIStyles.GetButtonRight()))
                {
                    removeIndex = i;
                }

                this.sortableList.PaintDropPoints(rectItem, i, itemsCount);
            }

            if (removeIndex != -1 && removeIndex < this.spItems.arraySize)
            {
                SerializedProperty spItem = this.spItems.GetArrayElementAtIndex(removeIndex);
                SerializedProperty spIOption = spItem.FindPropertyRelative(PROP_OPTION);
                SerializedProperty spIActions = spItem.FindPropertyRelative(PROP_ACTIONS);
                SerializedProperty spIConditions = spItem.FindPropertyRelative(PROP_CONDITIONS);
                UnityEngine.Object @object = null;
                switch (spIOption.intValue)
                {
                    case (int)Trigger.ItemOpts.Actions: @object = spIActions.objectReferenceValue; break;
                    case (int)Trigger.ItemOpts.Conditions: @object = spIConditions.objectReferenceValue; break;
                }

                this.spItems.DeleteArrayElementAtIndex(removeIndex);
            }

            EditorSortableList.SwapIndexes swapIndexes = this.sortableList.GetSortIndexes();
            if (swapIndexes != null)
            {
                this.spItems.MoveArrayElement(swapIndexes.src, swapIndexes.dst);
            }

            if (forceRepaint) this.Repaint();
        }

        private T CreateSubObject<T>() where T : MonoBehaviour
        {
            if (PrefabUtility.IsPartOfPrefabAsset(this.trigger.gameObject))
            {
                return CreatePrefabObject.AddGameObjectToPrefab<T>(
                    this.trigger.gameObject,
                    typeof(T).Name
                );
            }

            GameObject asset = CreateSceneObject.Create(typeof(T).Name, false);
            return asset.AddComponent<T>();
        }

		private void PaintNoCollider()
		{
			EditorGUILayout.HelpBox(MSG_REQUIRE_HAVE_COLLIDER, MessageType.Error);

			EditorGUILayout.BeginHorizontal();
			for (int i = 0; i < COLLIDER_TYPES.Length; ++i)
			{
				GUIStyle style = CoreGUIStyles.GetButtonMid();
				if (i == 0) style = CoreGUIStyles.GetButtonLeft();
				else if (i >= COLLIDER_TYPES.Length - 1) style = CoreGUIStyles.GetButtonRight();

				if (GUILayout.Button(COLLIDER_TYPES[i].Name, style))
				{
					Undo.AddComponent(this.trigger.gameObject, COLLIDER_TYPES[i]);
				}
			}

			EditorGUILayout.EndHorizontal();
		}

		// SCENE METHODS: -------------------------------------------------------------------------

		private void OnSceneGUI()
		{
			for (int i = 0; i < this.trigger.items.Count; ++i)
            {
                if (this.trigger.items[i].option == Trigger.ItemOpts.Actions &&
                    this.trigger.items[i].actions != null)
                {
                    this.PaintLine(
                        this.trigger.transform, 
                        this.trigger.items[i].actions.transform, 
                        Color.cyan
                    );
                }
                else if (this.trigger.items[i].option == Trigger.ItemOpts.Conditions &&
                         this.trigger.items[i].conditions != null)
                {
                    this.PaintLine(
                        this.trigger.transform,
                        this.trigger.items[i].conditions.transform,
                        Color.green
                    );
                }
            }
		}

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private Rect GetCenteredRect(Rect rect, float height)
		{
			return new Rect(
				rect.x, 
				rect.y + (rect.height - height)/2.0f, 
				rect.width, 
				height
			);
		}

		private void UpdateIgnitersPlatforms()
		{
			int numKeys = this.spIgnitersKeys.arraySize;

			this.ignitersCache = new IgniterCache[numKeys];
			IGNITERS_PLATFORM_NAMES = new string[numKeys];

			for (int i = 0; i < numKeys; ++i)
			{
				if (i == 0) IGNITERS_PLATFORM_NAMES[0] = "Any Platform";
				else 
				{
					int key = this.spIgnitersKeys.GetArrayElementAtIndex(i).intValue;
					IGNITERS_PLATFORM_NAMES[i] = ((Trigger.Platforms)key).ToString();
				}

				UnityEngine.Object reference = this.spIgnitersValues.GetArrayElementAtIndex(i).objectReferenceValue;
				this.ignitersCache[i] = new IgniterCache(reference);
			}
		}

		private void SelectNewIgniter(Type igniterType)
		{
			SerializedProperty property = this.spIgnitersValues.GetArrayElementAtIndex(this.ignitersIndex);
			if (property.objectReferenceValue != null)
			{
				DestroyImmediate(property.objectReferenceValue, true);
				property.objectReferenceValue = null;
			}

			Igniter igniter = (Igniter)this.trigger.gameObject.AddComponent(igniterType);
			igniter.Setup(this.trigger);
			igniter.enabled = false;

			property.objectReferenceValue = igniter;
			this.ignitersCache[this.ignitersIndex] = new IgniterCache(igniter);

            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.trigger.gameObject.scene);
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
            serializedObject.Update();
		}

		private void PaintLine(Transform transform1, Transform transform2, Color color)
		{
			Handles.color = color;
			Handles.DrawDottedLine(
				transform1.position, 
				transform2.position,
				DOTTED_LINES_SIZE
			);
		}

		// HIERARCHY CONTEXT MENU: ----------------------------------------------------------------

		[MenuItem("GameObject/Game Creator/Trigger", false, 0)]
		public static void CreateTrigger()
		{
			GameObject trigger = CreateSceneObject.Create("Trigger");
			SphereCollider collider = trigger.AddComponent<SphereCollider>();
			collider.isTrigger = true;
			trigger.AddComponent<Trigger>();
		}
	}
}