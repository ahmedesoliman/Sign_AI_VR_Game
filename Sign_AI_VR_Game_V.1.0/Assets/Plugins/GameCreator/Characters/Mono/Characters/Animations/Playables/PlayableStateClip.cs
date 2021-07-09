namespace GameCreator.Characters
{
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Playables;

    public class PlayableStateClip : PlayableState
    {
        private PlayableStateClip(AnimationClip animationClip, AvatarMask avatarMask,
            int layer, float time, float speed, float weight)
            : base(avatarMask, layer, time, speed, weight)
        {
            this.AnimationClip = animationClip;
        }

        // STATIC CONSTRUCTORS: -------------------------------------------------------------------

        public static PlayableStateClip Create<TInput0, TOutput>(
            AnimationClip animationClip, AvatarMask avatarMask, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, ref TInput0 input0, ref TOutput output)
            where TInput0 : struct, IPlayable
            where TOutput : struct, IPlayable
        {
            PlayableStateClip state = new PlayableStateClip(
                animationClip, avatarMask,
                layer, fade, speed, weight
            );

            AnimationClipPlayable input1 = AnimationClipPlayable.Create(graph, animationClip);

            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.Setup(ref graph, ref input0, ref input1, ref output);
            return state;
        }

        public static PlayableState CreateAfter(
            AnimationClip animationClip, AvatarMask avatarMask, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, PlayableBase previous)
        {
            PlayableStateClip state = new PlayableStateClip(
                animationClip, avatarMask,
                layer, fade, speed, weight
            );

            AnimationClipPlayable input1 = AnimationClipPlayable.Create(graph, animationClip);
            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.Setup(ref graph, previous, ref input1);
            return state;
        }

        public static PlayableState CreateBefore(
            AnimationClip animationClip, AvatarMask avatarMask, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, PlayableBase next)
        {
            PlayableStateClip state = new PlayableStateClip(
                animationClip, avatarMask,
                layer, fade, speed, weight
            );

            AnimationClipPlayable input1 = AnimationClipPlayable.Create(graph, animationClip);
            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.Setup(ref graph, ref input1, next);
            return state;
        }
    }
}
