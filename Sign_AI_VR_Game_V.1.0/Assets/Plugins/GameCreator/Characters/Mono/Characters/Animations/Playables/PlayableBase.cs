namespace GameCreator.Characters
{
    using System.Collections;
    using GameCreator.Core;
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Playables;

    public abstract class PlayableBase
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected float fadeIn;
        protected float fadeOut;

        protected float speed;
        protected float weight;

        private AvatarMask avatarMask;
        private AnimationLayerMixerPlayable mixer;

        // INTERFACE PROPERTIES: ------------------------------------------------------------------

        public Playable Mixer => this.mixer;

        public Playable Input0 => this.mixer.GetInput(0);
        public Playable Input1 => this.mixer.GetInput(1);
        public Playable Output => this.mixer.GetOutput(0);

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected PlayableBase(AvatarMask avatarMask, float fadeIn, float fadeOut, float speed, float weight)
        {
            this.avatarMask = avatarMask;

            this.fadeIn = fadeIn;
            this.fadeOut = fadeOut;

            this.speed = speed;
            this.weight = weight;
        }

        public void Destroy()
        {
            Playable output = this.Output;
            Playable input0 = this.Input0;

            output.DisconnectInput(0);
            this.mixer.DisconnectInput(0);

            output.ConnectInput(0, input0, 0);

            switch (output.GetInputCount())
            {
                case 1:
                    output.SetInputWeight(0, 1f);
                    break;

                case 2:
                    float outputWeight = this.mixer.GetInputWeight(0);
                    output.SetInputWeight(0, outputWeight);
                    break;
            }

            IEnumerator destroy = this.DestroyNextFrame();
            CoroutinesManager.Instance.StartCoroutine(destroy);
        }

        private IEnumerator DestroyNextFrame()
        {
            yield return null;

            if (this.Input1.IsValid() && this.Input1.CanDestroy()) this.Input1.Destroy();
            if (this.Mixer.IsValid()  && this.Mixer.CanDestroy())  this.Mixer.Destroy();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual void Stop(float fadeOut)
        {
            this.fadeOut = fadeOut;
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------

        public abstract bool Update();

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected void Setup<TInput0, TInput1, TOutput>(
            ref PlayableGraph graph, ref TInput0 input0, ref TInput1 input1, ref TOutput output)
            where TInput0 : struct, IPlayable
            where TInput1 : struct, IPlayable
            where TOutput : struct, IPlayable
        {
            input0.GetOutput(0).DisconnectInput(0);
            output.DisconnectInput(0);

            this.SetupMixer(ref graph, ref input0, ref input1, ref output);
        }

        protected void Setup<TInput1>(
            ref PlayableGraph graph, PlayableBase previous, ref TInput1 input1)
            where TInput1 : struct, IPlayable
        {
            Playable input0 = previous.Mixer;
            Playable output = previous.Output;
            previous.Output.DisconnectInput(0);

            this.SetupMixer(ref graph, ref input0, ref input1, ref output);
        }

        protected void Setup<TInput1>(
            ref PlayableGraph graph, ref TInput1 input1, PlayableBase next)
            where TInput1 : struct, IPlayable
        {
            Playable input0 = next.Input0;
            Playable output = next.Mixer;
            output.DisconnectInput(0);

            this.SetupMixer(ref graph, ref input0, ref input1, ref output);
        }

        protected void UpdateMixerWeights(float weight)
        {
            float weight0 = 1f;
            float weight1 = weight;

            this.mixer.SetInputWeight(0, weight0);
            this.mixer.SetInputWeight(1, weight1);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void SetupMixer<TInput0, TInput1, TOutput>(
            ref PlayableGraph graph, ref TInput0 input0, ref TInput1 input1, ref TOutput output)
            where TInput0 : struct, IPlayable
            where TInput1 : struct, IPlayable
            where TOutput : struct, IPlayable
        {
            input1.SetSpeed(this.speed);

            this.mixer = AnimationLayerMixerPlayable.Create(graph, 2);
            this.mixer.ConnectInput(0, input0, 0, 0f);
            this.mixer.ConnectInput(1, input1, 0, 1f);

            if (this.avatarMask != null)
            {
                this.mixer.SetLayerMaskFromAvatarMask(1, this.avatarMask);
            }

            output.ConnectInput(0, this.mixer, 0, 1f);
            this.UpdateMixerWeights(this.fadeIn > CharacterAnimation.EPSILON
                ? 0f
                : this.weight
            );
        }
    }
}
