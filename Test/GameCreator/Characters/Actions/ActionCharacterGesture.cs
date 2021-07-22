namespace GameCreator.Characters
{
    using System;
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
	public class ActionCharacterGesture : IAction
	{
        public TargetCharacter character = new TargetCharacter();
        public AnimationClip clip;
        public AvatarMask avatarMask;

        public NumberProperty speed = new NumberProperty(1.0f);

        public float fadeIn = 0.1f;
        public float fadeOut = 0.1f;
        public bool waitTillComplete = false;

        private CharacterAnimator characterAnimator;
        private bool forceStop;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = this.character.GetCharacter(target);
            if (this.clip != null && charTarget != null && charTarget.GetCharacterAnimator() != null)
            {
                this.characterAnimator = charTarget.GetCharacterAnimator();
                this.characterAnimator.CrossFadeGesture(
                    this.clip, this.speed.GetValue(target), this.avatarMask,
                    this.fadeIn, this.fadeOut
                );
            }

            return !this.waitTillComplete;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
            this.forceStop = false;
            Character charTarget = this.character.GetCharacter(target);
            if (this.clip != null && charTarget != null && charTarget.GetCharacterAnimator() != null)
            {
                if (this.waitTillComplete)
                {
                    float wait = Time.time + (this.clip.length / this.speed.GetValue(target));

                    WaitUntil waitUntil = new WaitUntil(() => this.forceStop || Time.time > wait);
                    yield return waitUntil;
                }
            }

			yield return 0;
		}

        public override void Stop()
        {
            this.forceStop = true;
            if (this.characterAnimator == null) return;
            this.characterAnimator.StopGesture(this.fadeOut);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Character/Character Gesture";
        private const string NODE_TITLE = "Character {0} do gesture {1}";

        private static readonly GUIContent GC_MASK = new GUIContent("Mask (optional)");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spCharacter;
        private SerializedProperty spClip;
        private SerializedProperty spAvatarMask;
        private SerializedProperty spWaitTillComplete;
        private SerializedProperty spSpeed;
        private SerializedProperty spFadeIn;
        private SerializedProperty spFadeOut;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
		{
            string clipName = (this.clip == null ? "none" : this.clip.name);
            if (clipName.Contains("@"))
            {
                string[] split = clipName.Split(new char[] {'@'}, 2, StringSplitOptions.RemoveEmptyEntries);
                clipName = split[split.Length - 1];
            }
            
            return string.Format(NODE_TITLE, this.character.ToString(), clipName);
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCharacter = this.serializedObject.FindProperty("character");
            this.spClip = this.serializedObject.FindProperty("clip");
            this.spAvatarMask = this.serializedObject.FindProperty("avatarMask");
            this.spWaitTillComplete = this.serializedObject.FindProperty("waitTillComplete");
            this.spSpeed = this.serializedObject.FindProperty("speed");
            this.spFadeIn = this.serializedObject.FindProperty("fadeIn");
            this.spFadeOut = this.serializedObject.FindProperty("fadeOut");
        }

		protected override void OnDisableEditorChild ()
		{
            this.spCharacter = null;
            this.spClip = null;
            this.spAvatarMask = null;
            this.spWaitTillComplete = null;
            this.spSpeed = null;
            this.spFadeIn = null;
            this.spFadeOut = null;
        }

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCharacter);
            EditorGUILayout.PropertyField(this.spClip);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spAvatarMask, GC_MASK);
            EditorGUILayout.PropertyField(this.spSpeed);

            EditorGUILayout.PropertyField(this.spFadeIn);
            EditorGUILayout.PropertyField(this.spFadeOut);
            EditorGUILayout.PropertyField(this.spWaitTillComplete);
            EditorGUI.indentLevel--;

            this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
