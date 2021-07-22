namespace GameCreator.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using GameCreator.Core;

    public abstract class IGenericBehavior<T> : PlayableBehaviour
    {
        protected T interactable;
        protected bool execute = false;

        public GameObject invoker;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            this.execute = true;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying) return;
            this.interactable = (T)playerData;

            if (this.interactable != null && this.execute)
            {
                this.Execute();
                this.execute = false;
            }
        }

        protected abstract void Execute();
    }
}