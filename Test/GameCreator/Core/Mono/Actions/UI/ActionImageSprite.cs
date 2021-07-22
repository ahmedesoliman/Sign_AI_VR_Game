namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.UI;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionImageSprite : IAction
	{
        public Image image;
        public SpriteProperty sprite = new SpriteProperty();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.image != null)
            {
                this.image.sprite = this.sprite.GetValue(target);
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    public static new string NAME = "UI/Image Sprite";
		private const string NODE_TITLE = "Change Image sprite";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spImage;
        private SerializedProperty spSprite;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return NODE_TITLE;
		}

		protected override void OnEnableEditorChild ()
		{
            this.spImage = this.serializedObject.FindProperty("image");
            this.spSprite = this.serializedObject.FindProperty("sprite");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spImage = null;
            this.spSprite = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spImage);
            EditorGUILayout.PropertyField(this.spSprite);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
