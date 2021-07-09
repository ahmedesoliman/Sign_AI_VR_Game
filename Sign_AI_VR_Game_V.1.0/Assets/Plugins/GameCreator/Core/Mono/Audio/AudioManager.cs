namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.Audio;

    [AddComponentMenu("Game Creator/Managers/AudioManager", 100)]
	public class AudioManager : Singleton<AudioManager>, IGameSave
	{
		[System.Serializable]
		public class Volume
		{
            public float mastr;
			public float music;
            public float sound;
            public float voice;

			public Volume(float mastr, float music, float sound, float voice)
			{
                this.mastr = mastr;
				this.music = music;
				this.sound = sound;
                this.voice = voice;
			}
		}

		private const int MAX_MUSIC_SOURCES = 10;
		private const int MAX_SOUND_SOURCES = 50;
        private const int MAX_VOICE_SOURCES = 10;

        private static float[] GLOBAL_VOLUMES =
        {
            1.0f, // mastr
            1.0f, // music
            1.0f, // sound
            1.0f, // voice
        };

        public const int INDEX_VOLUME_MASTR = 0;
        public const int INDEX_VOLUME_MUSIC = 1;
        public const int INDEX_VOLUME_SOUND = 2;
        public const int INDEX_VOLUME_VOICE = 3;

        // PROPERTIES: ----------------------------------------------------------------------------

        private int musicIndex;
        private int sound2DIndex;
        private int sound3DIndex;
        private int voiceIndex;

		private AudioBuffer[] musicSources;
        private AudioBuffer[] sound2DSources;
        private AudioBuffer[] sound3DSources;
        private AudioBuffer[] voiceSources;

		// INITIALIZE: ----------------------------------------------------------------------------

		protected override void OnCreate ()
		{
			this.musicSources = new AudioBuffer[MAX_MUSIC_SOURCES];
			this.sound2DSources = new AudioBuffer[MAX_SOUND_SOURCES];
            this.sound3DSources = new AudioBuffer[MAX_SOUND_SOURCES];
            this.voiceSources = new AudioBuffer[MAX_VOICE_SOURCES];

			for (int i = 0; i < this.musicSources.Length; ++i) this.musicSources[i] = this.CreateMusicSource(i);
			for (int i = 0; i < this.sound2DSources.Length; ++i) this.sound2DSources[i] = this.CreateSoundSource(i, "2D");
            for (int i = 0; i < this.sound3DSources.Length; ++i) this.sound3DSources[i] = this.CreateSoundSource(i, "3D");
            for (int i = 0; i < this.voiceSources.Length; ++i) this.voiceSources[i] = this.CreateVoiceSource(i);

			SaveLoadManager.Instance.Initialize(this);
		}

		private AudioBuffer CreateMusicSource(int index)
		{
            AudioSource clip = this.CreateAudioAsset("music", index, true);
			return new AudioBuffer(clip, INDEX_VOLUME_MUSIC);
		}

		private AudioBuffer CreateSoundSource(int index, string suffix)
		{
            AudioSource clip = this.CreateAudioAsset("sound" + suffix, index, false);
			return new AudioBuffer(clip, INDEX_VOLUME_SOUND);
		}

        private AudioBuffer CreateVoiceSource(int index)
        {
            AudioSource clip = this.CreateAudioAsset("voice", index, false);
            return new AudioBuffer(clip, INDEX_VOLUME_VOICE);
        }

        private AudioSource CreateAudioAsset(string audioName, int index, bool loop)
		{
			GameObject asset = new GameObject(audioName + "_" + index);
			asset.transform.parent = transform;

            AudioSource clip = asset.AddComponent<AudioSource>();
			clip.playOnAwake = false;
            clip.loop = loop;

			return clip;
		}

		// UPDATE: --------------------------------------------------------------------------------

		private void Update()
		{
			for (int i = 0; i < this.musicSources.Length; ++i)
			{
				this.musicSources[i].Update();
			}

			for (int i = 0; i < this.sound2DSources.Length; ++i)
			{
				this.sound2DSources[i].Update();
			}

            for (int i = 0; i < this.sound3DSources.Length; ++i)
            {
                this.sound3DSources[i].Update();
            }

            for (int i = 0; i < this.voiceSources.Length; ++i)
            {
                this.voiceSources[i].Update();
            }
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public void PlayMusic(AudioClip audioClip, float fadeIn = 0f, float volume = 1f, AudioMixerGroup mixer = null)
        {
			this.musicIndex = (++this.musicIndex < this.musicSources.Length ? this.musicIndex : 0);
			this.musicSources[this.musicIndex].Play(audioClip, fadeIn, volume, mixer);
		}

		public void StopMusic(AudioClip audioClip, float fadeOut = 0.0f)
		{
			for (int i = 0; i < this.musicSources.Length; ++i)
			{
                if (this.musicSources[i].GetAudioClip() == audioClip)
                {
                    this.musicSources[i].Stop(fadeOut);
                }
			}
		}

        public void StopAllMusic(float fadeOut = 0.0f)
        {
            for (int i = 0; i < this.musicSources.Length; ++i)
            {
                this.musicSources[i].Stop(fadeOut);
            }
        }

        public void PlaySound2D(AudioClip audioClip, float fadeIn = 0f,
            float volume = 1f, AudioMixerGroup mixer = null)
		{
			this.sound2DIndex = (++this.sound2DIndex < this.sound2DSources.Length ? this.sound2DIndex : 0);
			this.sound2DSources[this.sound2DIndex].Play(audioClip, fadeIn, volume, mixer);
		}

		public void StopSound2D(AudioClip audioClip, float fadeOut = 0f)
		{
			for (int i = 0; i < this.sound2DSources.Length; ++i)
			{
				AudioClip clip = this.sound2DSources[i].GetAudioClip();
				if (clip != null && clip.name == audioClip.name)
				{
					this.sound2DSources[i].Stop(fadeOut);
				}
			}
		}

        public void PlaySound3D(AudioClip audioClip, float fadeIn, Vector3 position,
            float spatialBlend, float pitch, float volume = 1f, AudioMixerGroup mixer = null)
        {
            this.sound3DIndex = (++this.sound3DIndex < this.sound3DSources.Length ? this.sound3DIndex : 0);
            this.sound3DSources[this.sound3DIndex].SetSpatialBlend(spatialBlend);
            this.sound3DSources[this.sound3DIndex].SetPitch(pitch);
            this.sound3DSources[this.sound3DIndex].SetPosition(position);
            this.sound3DSources[this.sound3DIndex].Play(audioClip, fadeIn, volume, mixer);
        }

        public void StopSound3D(AudioClip audioClip, float fadeOut = 0f)
        {
            for (int i = 0; i < this.sound3DSources.Length; ++i)
            {
                AudioClip clip = this.sound3DSources[i].GetAudioClip();
                if (clip != null && clip.name == audioClip.name)
                {
                    this.sound3DSources[i].Stop(fadeOut);
                }
            }
        }

        public void StopSound(AudioClip audioClip, float fadeOut = 0f)
        {
            this.StopSound2D(audioClip, fadeOut);
            this.StopSound3D(audioClip, fadeOut);
        }

        public void StopAllSounds(float fadeOut = 0f)
        {
            for (int i = 0; i < this.sound2DSources.Length; ++i)
            {
                this.sound2DSources[i].Stop(fadeOut);
            }

            for (int i = 0; i < this.sound3DSources.Length; ++i)
            {
                this.sound3DSources[i].Stop(fadeOut);
            }
        }

        public void PlayVoice(AudioClip audioClip, float fadeIn = 0f,
            float volume = 1f, AudioMixerGroup mixer = null)
        {
            this.voiceIndex = (++this.voiceIndex < this.voiceSources.Length ? this.voiceIndex : 0);
            this.voiceSources[this.voiceIndex].Play(audioClip, fadeIn, volume, mixer);
        }

        public void StopVoice(AudioClip audioClip, float fadeOut = 0f)
        {
            for (int i = 0; i < this.voiceSources.Length; ++i)
            {
                AudioClip clip = this.voiceSources[i].GetAudioClip();
                if (clip != null && clip.name == audioClip.name)
                {
                    this.voiceSources[i].Stop(fadeOut);
                }
            }
        }

        public void StopAllVoices(float fadeOut = 0f)
        {
            for (int i = 0; i < this.voiceSources.Length; ++i)
            {
                this.voiceSources[i].Stop(fadeOut);
            }
        }

        public void StopAllAudios(float fadeOut = 0f)
        {
            this.StopAllMusic(fadeOut);
            this.StopAllSounds(fadeOut);
            this.StopAllVoices(fadeOut);
        }

        // VOLUME METHODS: ------------------------------------------------------------------------

        public void SetGlobalVolume(int index, float volume)
        {
            AudioManager.GLOBAL_VOLUMES[index] = volume;
        }

        public void SetGlobalMastrVolume(float volume)
        {
            this.SetGlobalVolume(INDEX_VOLUME_MASTR, volume);
        }

        public void SetGlobalMusicVolume(float volume) 
		{
            this.SetGlobalVolume(INDEX_VOLUME_MUSIC, volume);
        }

		public void SetGlobalSoundVolume(float volume)
		{
            this.SetGlobalVolume(INDEX_VOLUME_SOUND, volume);
        }

        public void SetGlobalVoiceVolume(float volume)
        {
            this.SetGlobalVolume(INDEX_VOLUME_VOICE, volume);
        }

        public float GetGlobalVolume(int index) { return AudioManager.GLOBAL_VOLUMES[index]; }

        public float GetGlobalMastrVolume() { return this.GetGlobalVolume(INDEX_VOLUME_MASTR); }
        public float GetGlobalMusicVolume() { return this.GetGlobalVolume(INDEX_VOLUME_MUSIC); }
		public float GetGlobalSoundVolume() { return this.GetGlobalVolume(INDEX_VOLUME_SOUND); }
        public float GetGlobalVoiceVolume() { return this.GetGlobalVolume(INDEX_VOLUME_VOICE); }

        // INTERFACE ISAVELOAD: -------------------------------------------------------------------

        public string GetUniqueName()
		{
			return "volume";
		}

		public System.Type GetSaveDataType()
		{
			return typeof(AudioManager.Volume);
		}

		public object GetSaveData()
		{
			return new AudioManager.Volume(
				this.GetGlobalMastrVolume(),
                this.GetGlobalMusicVolume(),
                this.GetGlobalSoundVolume(),
                this.GetGlobalVoiceVolume()
            );
		}

		public void ResetData()
		{
            AudioManager.Instance.SetGlobalMastrVolume(1.0f);
            AudioManager.Instance.SetGlobalMusicVolume(1.0f);
			AudioManager.Instance.SetGlobalSoundVolume(1.0f);
            AudioManager.Instance.SetGlobalVoiceVolume(1.0f);
		}

		public void OnLoad(object generic)
		{
            AudioManager.Volume volume = generic as AudioManager.Volume;
            if (volume == null) return;

            AudioManager.Instance.SetGlobalMastrVolume(volume.mastr);
            AudioManager.Instance.SetGlobalMusicVolume(volume.music);
			AudioManager.Instance.SetGlobalSoundVolume(volume.sound);
            AudioManager.Instance.SetGlobalVoiceVolume(volume.voice);
		}
	}
}