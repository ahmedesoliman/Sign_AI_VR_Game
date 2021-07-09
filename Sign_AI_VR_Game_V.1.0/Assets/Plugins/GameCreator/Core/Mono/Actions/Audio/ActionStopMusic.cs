namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionStopMusic : IAction 
	{
        public enum MusicType
        {
            AllMusic,
            AudioClip
        }

        public MusicType type = MusicType.AllMusic;
        [Indent] public AudioClip audioClip;

        [Range(0f, 10f)]
        public float fadeOut;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            switch (this.type)
            {
                case MusicType.AllMusic:
                    AudioManager.Instance.StopAllMusic(this.fadeOut);
                    break;

                case MusicType.AudioClip:
                    AudioManager.Instance.StopMusic(this.audioClip, this.fadeOut);
                    break;
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Audio/Stop Music";
		private const string NODE_TITLE = "Stop Music {0}";

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE,
				this.fadeOut > 0f ? "(" + this.fadeOut.ToString() + "s)" : ""
			);
		}

        private SerializedProperty spType;
        private SerializedProperty spAudioClip;
        private SerializedProperty spFadeOut;

        protected override void OnEnableEditorChild()
        {
            this.spType = this.serializedObject.FindProperty("type");
            this.spAudioClip = this.serializedObject.FindProperty("audioClip");
            this.spFadeOut = this.serializedObject.FindProperty("fadeOut");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spType);
            if (this.spType.enumValueIndex == (int)MusicType.AudioClip)
            {
                EditorGUILayout.PropertyField(this.spAudioClip);
            }

            EditorGUILayout.PropertyField(this.spFadeOut);

            this.serializedObject.ApplyModifiedProperties();
        }

        #endif
    }
}