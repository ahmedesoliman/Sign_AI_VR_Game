namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using UnityEngine.Audio;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("")]
	public class ActionPlaySound : IAction 
	{
        public enum AudioMixerType
        {
            None,
            Custom,
            DefaultSoundMixer
        }

		public AudioClip audioClip;
        public AudioMixerType audioMixer = AudioMixerType.DefaultSoundMixer;
        [Indent] public AudioMixerGroup mixerGroup;

        [Space] public float fadeIn;
        [Range(0f, 1f)] public float volume = 1.0f;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            AudioMixerGroup mixer = null;
            switch (this.audioMixer)
            {
                case AudioMixerType.DefaultSoundMixer:
                    mixer = DatabaseGeneral.Load().soundAudioMixer;
                    break;

                case AudioMixerType.Custom:
                    mixer = this.mixerGroup;
                    break;
            }

            AudioManager.Instance.PlaySound2D(this.audioClip, this.fadeIn, this.volume, mixer);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Audio/Play Sound";
		private const string NODE_TITLE = "Play Sound {0}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE,
				this.audioClip == null ? "(none)" : this.audioClip.name
			);
		}

        private SerializedProperty spAudioClip;
        private SerializedProperty spAudioMixer;
        private SerializedProperty spMixerGroup;

        private SerializedProperty spFadeIn;
        private SerializedProperty spVolume;

        protected override void OnEnableEditorChild()
        {
            this.spAudioClip = this.serializedObject.FindProperty("audioClip");
            this.spAudioMixer = this.serializedObject.FindProperty("audioMixer");
            this.spMixerGroup = this.serializedObject.FindProperty("mixerGroup");
            this.spFadeIn = this.serializedObject.FindProperty("fadeIn");
            this.spVolume = this.serializedObject.FindProperty("volume");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spAudioClip);
            EditorGUILayout.PropertyField(this.spAudioMixer);
            if (this.spAudioMixer.enumValueIndex == (int)AudioMixerType.Custom)
            {
                EditorGUILayout.PropertyField(this.spMixerGroup);
            }

            EditorGUILayout.PropertyField(this.spFadeIn);
            EditorGUILayout.PropertyField(this.spVolume);

            this.serializedObject.ApplyModifiedProperties();
        }

        #endif
    }
}