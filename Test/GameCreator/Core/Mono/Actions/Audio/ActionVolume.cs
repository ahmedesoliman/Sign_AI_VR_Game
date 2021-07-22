namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Variables;

    #if UNITY_EDITOR
    using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionVolume : IAction 
	{
        public enum VolumeType
        {
            Master = AudioManager.INDEX_VOLUME_MASTR,
            Music = AudioManager.INDEX_VOLUME_MUSIC,
            Sound = AudioManager.INDEX_VOLUME_SOUND,
            Voice = AudioManager.INDEX_VOLUME_VOICE,
        }

        public VolumeType type = VolumeType.Music;
        public NumberProperty volume = new NumberProperty(1.0f);

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            float value = this.volume.GetValue(target);
            AudioManager.Instance.SetGlobalVolume((int)this.type, value);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Audio/Change Volume";
		private const string NODE_TITLE = "Change {0} volume to {1}";

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.type, this.volume);
		}

		#endif
	}
}