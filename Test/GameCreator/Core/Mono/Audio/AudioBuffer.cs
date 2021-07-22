namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.Audio;

    [System.Serializable]
	public class AudioBuffer
	{
        private const float ZERO = 0.001f;

        // PROPERTIES: ----------------------------------------------------------------------------

        private AudioSource audio;

        private int indexVolume;
		private float clipVolume;
        private float smoothTime;

        private float opacityValue;
        private float opacityTarget;
        private float opacityVelocity;

		// INITIALIZE: ----------------------------------------------------------------------------

		public AudioBuffer(AudioSource audio, int indexVolume)
		{
			this.audio = audio;
            this.clipVolume = 1f;
			this.indexVolume = indexVolume;
            this.smoothTime = 1f;

            this.opacityValue = 1f;
            this.opacityTarget = 1f;
            this.opacityVelocity = 0f;
		}

		// UPDATE: --------------------------------------------------------------------------------

		public void Update()
		{
			if (!this.audio.clip || !this.audio.isPlaying) return;

            this.opacityValue = Mathf.SmoothDamp(
                this.opacityValue,
                this.opacityTarget,
                ref this.opacityVelocity,
                this.smoothTime
            );

            this.audio.volume = (
                this.opacityValue * this.clipVolume *
                AudioManager.Instance.GetGlobalVolume(this.indexVolume) *
                AudioManager.Instance.GetGlobalVolume(AudioManager.INDEX_VOLUME_MASTR)
            );

            if (Mathf.Approximately(this.opacityTarget + this.opacityValue, 0f))
            {
                this.audio.Stop();
                this.audio.clip = null;
            }
		}

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Play(AudioClip audioClip, float fadeDuration = 0.0f,
            float clipVolume = 1.0f, AudioMixerGroup mixer = null)
        {
            this.smoothTime = fadeDuration;
            this.clipVolume = clipVolume;

            this.audio.clip = audioClip;
            this.audio.outputAudioMixerGroup = mixer;
			this.audio.Play();

            this.opacityValue = fadeDuration > ZERO ? 0f : 1f;
            this.opacityTarget = 1f;
            this.opacityVelocity = 0f;
        }

		public void Stop(float fadeDuration = 0.0f)
		{
            this.smoothTime = fadeDuration;

            this.opacityTarget = 0f;
            this.opacityVelocity = 0f;

            if (fadeDuration <= ZERO)
            {
                this.audio.Stop();
                this.audio.clip = null;
            }
		}

		public AudioClip GetAudioClip()
		{
			return this.audio.clip;
		}

        public void SetPosition(Vector3 position)
        {
            this.audio.transform.position = position;
        }

        public void SetPitch(float pitch)
        {
            this.audio.pitch = pitch;
        }

        public void SetSpatialBlend(float spatialBlend)
        {
            this.audio.spatialize = !Mathf.Approximately(spatialBlend, 0);
            this.audio.spatialBlend = spatialBlend;
        }
	}
}