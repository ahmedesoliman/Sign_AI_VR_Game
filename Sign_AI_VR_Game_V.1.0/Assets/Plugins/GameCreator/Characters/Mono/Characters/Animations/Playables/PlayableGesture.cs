namespace GameCreator.Characters
{
    using System;
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Playables;

    public abstract class PlayableGesture : PlayableBase
    {
        public struct Parameter
        {
            public int id;
            public float value;

            public Parameter(int id, float value)
            {
                this.id = id;
                this.value = value;
            }
            
            public Parameter(string parameter, float value)
                : this(Animator.StringToHash(parameter), value)
            { }
        }
        
        private double endFreezeTime = -100.0;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected PlayableGesture(AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed)
            : base(avatarMask, fadeIn, fadeOut, speed, 1f)
        { }

        // UPDATE: --------------------------------------------------------------------------------

        public override bool Update()
        {
            if (this.endFreezeTime > 0f && Time.time > this.endFreezeTime)
            {
                this.Stop(0f);
                return true;
            }

            if (this.Input1.IsDone())
            {
                this.Stop(0f);
                return true;
            }

            float time = (float)this.Input1.GetTime();
            if (time + this.fadeOut >= this.Input1.GetDuration())
            {
                float t = ((float)this.Input1.GetDuration() - time) / this.fadeOut;

                t = Mathf.Clamp01(t);
                this.UpdateMixerWeights(t);
            }
            else if (time <= this.fadeIn)
            {
                float t = time / this.fadeIn;

                t = Mathf.Clamp01(t);
                this.UpdateMixerWeights(t);
            }
            else
            {
                this.UpdateMixerWeights(1f);
            }

            return false;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void StretchDuration(float extraTime)
        {
            if (this.Input1.GetDuration() - this.Input1.GetTime() < extraTime)
            {
                this.Input1.SetSpeed(0f);
                this.endFreezeTime = Time.time + extraTime;
            }
        }

        public override void Stop(float fadeOut)
        {
            base.Stop(fadeOut);
            this.fadeOut = fadeOut;

            this.Input1.SetDuration(Math.Min(
                this.Input1.GetTime() + fadeOut,
                this.Input1.GetDuration())
            );
        }
    }
}
