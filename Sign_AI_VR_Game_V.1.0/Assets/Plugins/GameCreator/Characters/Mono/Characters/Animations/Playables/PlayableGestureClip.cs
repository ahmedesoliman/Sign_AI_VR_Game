namespace GameCreator.Characters
{
    using System;
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Playables;

    public class PlayableGestureClip : PlayableGesture
    {
        private static AnimationClip GESTURE_DEBUG;

        private PlayableGestureClip(AnimationClip animationClip, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed)
            : base(avatarMask, fadeIn, fadeOut, speed)
        {
            if (GESTURE_DEBUG != null) GESTURE_DEBUG = animationClip;
        }

        public static PlayableGestureClip Create<TInput0, TOutput>(
            AnimationClip animationClip, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed,
            ref PlayableGraph graph, ref TInput0 input0, ref TOutput output)
            where TInput0 : struct, IPlayable
            where TOutput : struct, IPlayable
        {
            PlayableGestureClip gesture = new PlayableGestureClip(
                animationClip, avatarMask,
                fadeIn, fadeOut, speed
            );

            AnimationClipPlayable input1 = AnimationClipPlayable.Create(graph, animationClip);

            input1.SetTime(0f);
            input1.SetSpeed(speed);
            input1.SetDuration(animationClip.length);

            gesture.Setup(ref graph, ref input0, ref input1, ref output);
            return gesture;
        }

        public static PlayableGestureClip CreateAfter(
            AnimationClip animationClip, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed,
            ref PlayableGraph graph, PlayableBase previous)
        {
            PlayableGestureClip gesture = new PlayableGestureClip(
                animationClip, avatarMask,
                fadeIn, fadeOut, speed
            );

            AnimationClipPlayable input1 = AnimationClipPlayable.Create(graph, animationClip);

            input1.SetTime(0f);
            input1.SetSpeed(speed);
            input1.SetDuration(animationClip.length);

            gesture.Setup(ref graph, previous, ref input1);
            return gesture;
        }
    }
}
