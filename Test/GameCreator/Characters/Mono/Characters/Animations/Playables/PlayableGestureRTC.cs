namespace GameCreator.Characters
{
    using System;
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Playables;

    public class PlayableGestureRTC : PlayableGesture
    {
        private static RuntimeAnimatorController GESTURE_DEBUG;

        private PlayableGestureRTC(RuntimeAnimatorController rtc, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed)
            : base(avatarMask, fadeIn, fadeOut, speed)
        {
            if (GESTURE_DEBUG != null) GESTURE_DEBUG = rtc;
        }

        public static PlayableGestureRTC Create<TInput0, TOutput>(
            RuntimeAnimatorController rtc, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed,
            ref PlayableGraph graph, ref TInput0 input0, ref TOutput output, 
            params Parameter[] parameters)
            where TInput0 : struct, IPlayable
            where TOutput : struct, IPlayable
        {
            PlayableGestureRTC gesture = new PlayableGestureRTC(
                rtc, avatarMask,
                fadeIn, fadeOut, speed
            );

            AnimatorControllerPlayable input1 = AnimatorControllerPlayable.Create(graph, rtc);

            foreach (Parameter parameter in parameters)
            {
                input1.SetFloat(parameter.id, parameter.value);
            }
            
            float duration = 0f;
            foreach (AnimationClip clip in rtc.animationClips)
            {
                duration = Mathf.Max(duration, clip.length);
            }
            
            input1.SetTime(0f);
            input1.SetSpeed(speed);
            input1.SetDuration(duration);

            gesture.Setup(ref graph, ref input0, ref input1, ref output);
            return gesture;
        }

        public static PlayableGestureRTC CreateAfter(
            RuntimeAnimatorController rtc, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed,
            ref PlayableGraph graph, PlayableBase previous,
            params Parameter[] parameters)
        {
            PlayableGestureRTC gesture = new PlayableGestureRTC(
                rtc, avatarMask,
                fadeIn, fadeOut, speed
            );

            AnimatorControllerPlayable input1 = AnimatorControllerPlayable.Create(graph, rtc);

            foreach (Parameter parameter in parameters)
            {
                input1.SetFloat(parameter.id, parameter.value);
            }
            
            float duration = 0f;
            foreach (AnimationClip clip in rtc.animationClips)
            {
                duration = Mathf.Max(duration, clip.length);
            }
            
            input1.SetTime(0f);
            input1.SetSpeed(speed);
            input1.SetDuration(duration);

            gesture.Setup(ref graph, previous, ref input1);
            return gesture;
        }
    }
}
