namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using UnityEngine.Audio;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionPlaySound3D : IAction 
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

        [Range(0f, 10f)]
        public float fadeIn;

        [Range(0.0f, 1.0f)]
        public float volume = 1f;

        [Range(0.0f, 1.0f)]
        public float spatialBlend = 0.85f;
        public NumberProperty pitch = new NumberProperty(1.0f);
        public TargetPosition position = new TargetPosition(TargetPosition.Target.Player);

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

            AudioManager.Instance.PlaySound3D(
                this.audioClip, 
                this.fadeIn, 
                this.position.GetPosition(target),
                this.spatialBlend,
                this.pitch.GetValue(target),
                this.volume,
                mixer
            );

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Audio/Play Sound 3D";
		private const string NODE_TITLE = "Play 3D Sound {0} {1}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.audioClip == null ? "(none)" : this.audioClip.name,
                this.fadeIn > 0f ? "(" + this.fadeIn.ToString() + "s)" : ""
            );
        }

        private SerializedProperty spAudioClip;
        private SerializedProperty spAudioMixer;
        private SerializedProperty spMixerGroup;

        private SerializedProperty spFadeIn;
        private SerializedProperty spVolume;
        private SerializedProperty spSpatialBlend;
        private SerializedProperty spPitch;
        private SerializedProperty spPosition;

        protected override void OnEnableEditorChild()
        {
            this.spAudioClip = this.serializedObject.FindProperty("audioClip");
            this.spAudioMixer = this.serializedObject.FindProperty("audioMixer");
            this.spMixerGroup = this.serializedObject.FindProperty("mixerGroup");
            this.spFadeIn = this.serializedObject.FindProperty("fadeIn");
            this.spVolume = this.serializedObject.FindProperty("volume");
            this.spSpatialBlend = this.serializedObject.FindProperty("spatialBlend");
            this.spPitch = this.serializedObject.FindProperty("pitch");
            this.spPosition = this.serializedObject.FindProperty("position");
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

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spFadeIn);
            EditorGUILayout.PropertyField(this.spVolume);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spSpatialBlend);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spPitch);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spPosition);

            this.serializedObject.ApplyModifiedProperties();
        }

        #endif
    }
}