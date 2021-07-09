namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using UnityEngine.Audio;
    using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
#endif

    [AddComponentMenu("")]
	public class ActionAudioMixerParameter : IAction 
	{
        public AudioMixer audioMixer;

        [Space]
        public string parameter = "MyParameter";
        public NumberProperty value = new NumberProperty(1.0f);

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.audioMixer)
            {
                this.audioMixer.SetFloat(this.parameter, this.value.GetValue(target));
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Audio/Audio Mixer Parameter";
		private const string NODE_TITLE = "Change {0} mixer {1} parameter";

		public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE,
                this.audioMixer != null ? this.audioMixer.name : "(none)",
                this.parameter
            );
		}

		#endif
	}
}