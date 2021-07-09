namespace GameCreator.Characters
{
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Playables;

    public abstract class PlayableState : PlayableBase
    {
        public int Layer { get; protected set; }

        public AnimationClip AnimationClip { get; protected set; }
        public CharacterState CharacterState { get; protected set; }

        protected bool isDisposing;
        private float currentWeight;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected PlayableState(AvatarMask avatarMask,
            int layer, float time, float speed, float weight)
            : base(avatarMask, time, 0f, speed, weight)
        {
            this.Layer = layer;
        }

        // UPDATE: --------------------------------------------------------------------------------

        public override bool Update()
        {
            if (this.Input1.IsDone())
            {
                this.Stop(0f);
                return true;
            }

            float increment = this.currentWeight < this.weight
                ? this.fadeIn
                : -this.fadeOut;

            if (Mathf.Abs(increment) < float.Epsilon) this.currentWeight = this.weight;
            else this.currentWeight += Time.deltaTime / increment;

            this.UpdateMixerWeights(Mathf.Clamp01(this.currentWeight));
            return this.isDisposing && this.currentWeight < float.Epsilon;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override void Stop(float fadeOut)
        {
            base.Stop(fadeOut);

            this.isDisposing = true;
            this.weight = 0f;
        }

        public void StretchDuration(float freezeTime)
        {
            double duration = this.Input1.GetTime() + freezeTime;
            this.Input1.SetDuration(duration);
            this.Input1.SetSpeed(1f);
        }

        public void SetWeight(float weight)
        {
            this.weight = weight;
        }

        public virtual void OnExitState()
        { }
    }
}
