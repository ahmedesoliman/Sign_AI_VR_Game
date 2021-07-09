namespace GameCreator.Characters
{
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.AI;
	using UnityEditor;
	using GameCreator.Core;

	[CustomEditor(typeof(PlayerCharacter))]
	public class PlayerCharacterEditor : CharacterEditor 
	{
		private const string PLAYER_PREFAB_PATH = "Assets/Plugins/GameCreator/Characters/Prefabs/Player.prefab";
		private const string SECTION_INPUT = "Player Input";

		private const string PROP_INPUTT = "inputType";
		private const string PROP_MOUSEB = "mouseButtonMove";
        private const string PROP_LAYERM = "mouseLayerMask";
        private const string PROP_INVERT = "invertAxis";
        private const string PROP_INPUT_JMP = "jumpKey";

        private const string PROP_USE_ACC = "useAcceleration";
        private const string PROP_ACC = "acceleration";
        private const string PROP_DEC = "deceleration";

        // PROPERTIES: ----------------------------------------------------------------------------

        private Section sectionInput;
		private SerializedProperty spInputType;
		private SerializedProperty spMouseButtonMove;
        private SerializedProperty spMouseLayerMask;
        private SerializedProperty spInvertAxis;
		private SerializedProperty spInputJump;

        private SerializedProperty spUseAcceleration;
        private SerializedProperty spAcceleration;
        private SerializedProperty spDeceleration;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected new void OnEnable()
		{
			base.OnEnable();

			string iconInputPath = Path.Combine(CHARACTER_ICONS_PATH, "PlayerInput.png");
			Texture2D iconInput = AssetDatabase.LoadAssetAtPath<Texture2D>(iconInputPath);
			this.sectionInput = new Section(SECTION_INPUT, iconInput, this.Repaint);

			this.spInputType = serializedObject.FindProperty(PROP_INPUTT);
			this.spMouseButtonMove = serializedObject.FindProperty(PROP_MOUSEB);
            this.spMouseLayerMask = serializedObject.FindProperty(PROP_LAYERM);
            this.spInvertAxis = serializedObject.FindProperty(PROP_INVERT);
            this.spInputJump = serializedObject.FindProperty(PROP_INPUT_JMP);

            this.spUseAcceleration = serializedObject.FindProperty(PROP_USE_ACC);
            this.spAcceleration = serializedObject.FindProperty(PROP_ACC);
            this.spDeceleration = serializedObject.FindProperty(PROP_DEC);

            if (this.spMouseLayerMask.intValue == 0)
            {
                this.spMouseLayerMask.intValue = ~0;
            }
		}

		protected new void OnDisable()
		{
			base.OnDisable();
		}

		// INSPECTOR GUI: -------------------------------------------------------------------------

		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			EditorGUILayout.Space();

			base.PaintInspector();
			this.sectionInput.PaintSection();
			using (var group = new EditorGUILayout.FadeGroupScope (this.sectionInput.state.faded))
			{
				if (group.visible)
				{
					EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

					EditorGUILayout.PropertyField(this.spInputType);
                    EditorGUI.indentLevel++;

					if (this.spInputType.intValue == (int)PlayerCharacter.INPUT_TYPE.PointAndClick ||
                        this.spInputType.intValue == (int)PlayerCharacter.INPUT_TYPE.FollowPointer)
					{
						EditorGUILayout.PropertyField(this.spMouseButtonMove);
					}

                    if (this.spInputType.intValue == (int)PlayerCharacter.INPUT_TYPE.PointAndClick)
                    {
                        EditorGUILayout.PropertyField(this.spMouseLayerMask);
                        if (this.spMouseLayerMask.intValue == 0)
                        {
                            this.spMouseLayerMask.intValue = ~0;
                        }
                    }

                    if (this.spInputType.intValue == (int)PlayerCharacter.INPUT_TYPE.SideScrollX ||
                        this.spInputType.intValue == (int)PlayerCharacter.INPUT_TYPE.SideScrollZ)
                    {
                        EditorGUILayout.PropertyField(this.spInvertAxis);
                    }

                    EditorGUI.indentLevel--;
					EditorGUILayout.PropertyField(this.spInputJump);

                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(this.spUseAcceleration);
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginDisabledGroup(!this.spUseAcceleration.boolValue);

                    EditorGUILayout.PropertyField(this.spAcceleration);
                    EditorGUILayout.PropertyField(this.spDeceleration);

                    EditorGUI.EndDisabledGroup();
                    EditorGUI.indentLevel--;

                    EditorGUILayout.EndVertical();
				}
			}

			EditorGUILayout.Space();
			serializedObject.ApplyModifiedProperties();
		}

		// MENU ITEM: -----------------------------------------------------------------------------

		[MenuItem("GameObject/Game Creator/Characters/Player", false, 0)]
		public static void CreatePlayer()
		{
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PLAYER_PREFAB_PATH);
			if (prefab == null) return;

            GameObject instance = Instantiate(prefab);
            instance.name = prefab.name;
			instance = CreateSceneObject.Create(instance, true);
		}
	}
}