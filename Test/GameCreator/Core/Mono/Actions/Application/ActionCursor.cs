namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionCursor : IAction 
	{
		public bool changeTexture = false;
        public Texture2DProperty texture = new Texture2DProperty();
		public Vector2 hotspot = Vector2.zero;
		public bool changeCursorLock = false;
		public CursorLockMode cursorLockMode = CursorLockMode.None;
		public bool changeVisibility = false;
		public bool isVisible = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.changeTexture) Cursor.SetCursor(this.texture.GetValue(target), this.hotspot, CursorMode.Auto);
            if (this.changeCursorLock) Cursor.lockState = this.cursorLockMode;
            if (this.changeVisibility) Cursor.visible = this.isVisible;

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Application/Cursor";
		private const string NODE_TITLE = "Change cursor parameters";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spChangeTexture;
		private SerializedProperty spTexture;
		private SerializedProperty spHotspot;
		private SerializedProperty spChangeCursorLock;
		private SerializedProperty spCursorLockMode;
		private SerializedProperty spChangeVisibility;
		private SerializedProperty spIsVisible;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return NODE_TITLE;
		}

		protected override void OnEnableEditorChild ()
		{
			this.spChangeTexture = this.serializedObject.FindProperty("changeTexture");
			this.spTexture = this.serializedObject.FindProperty("texture");
			this.spHotspot = this.serializedObject.FindProperty("hotspot");
			this.spChangeCursorLock = this.serializedObject.FindProperty("changeCursorLock");
			this.spCursorLockMode = this.serializedObject.FindProperty("cursorLockMode");
			this.spChangeVisibility = this.serializedObject.FindProperty("changeVisibility");
			this.spIsVisible = this.serializedObject.FindProperty("isVisible");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spChangeTexture);
            EditorGUI.BeginDisabledGroup(!this.spChangeTexture.boolValue);
            EditorGUILayout.PropertyField(this.spTexture);
            EditorGUILayout.PropertyField(this.spHotspot);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.spChangeCursorLock);
			if (this.spChangeCursorLock.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.spCursorLockMode);
				EditorGUI.indentLevel--;
			}

            EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.spChangeVisibility);
			if (this.spChangeVisibility.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.spIsVisible);
				EditorGUI.indentLevel--;
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}