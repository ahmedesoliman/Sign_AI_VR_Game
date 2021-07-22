namespace GameCreator.Characters
{
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Playables;

    public class PlayableStateRTC : PlayableState
    {
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected PlayableStateRTC(AvatarMask avatarMask,
            int layer,
            float time, float speed, float weight)
            : base(avatarMask, layer, time, speed, weight)
        { }

        // STATIC CONSTRUCTORS: -------------------------------------------------------------------

        public static PlayableStateRTC Create<TInput0, TOutput>(
            RuntimeAnimatorController runtimeController, AvatarMask avatarMask, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, ref TInput0 input0, ref TOutput output,
            params PlayableGesture.Parameter[] parameters)
            where TInput0 : struct, IPlayable
            where TOutput : struct, IPlayable
        {
            PlayableStateRTC state = new PlayableStateRTC(
                avatarMask, layer,
                fade, speed, weight
            );
            
            AnimatorControllerPlayable input1 = AnimatorControllerPlayable.Create(
                graph,
                runtimeController
            );
            
            foreach (PlayableGesture.Parameter parameter in parameters)
            {
                input1.SetFloat(parameter.id, parameter.value);
            }

            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.Setup(ref graph, ref input0, ref input1, ref output);
            return state;
        }

        public static PlayableStateRTC CreateAfter(
            RuntimeAnimatorController runtimeController, AvatarMask avatarMask, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, PlayableBase previous,
            params PlayableGesture.Parameter[] parameters)
        {
            PlayableStateRTC state = new PlayableStateRTC(
                avatarMask, layer,
                fade, speed, weight
            );

            AnimatorControllerPlayable input1 = AnimatorControllerPlayable.Create(
                graph,
                runtimeController
            );
            
            foreach (PlayableGesture.Parameter parameter in parameters)
            {
                input1.SetFloat(parameter.id, parameter.value);
            }

            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.Setup(ref graph, previous, ref input1);
            return state;
        }

        public static PlayableStateRTC CreateBefore(
            RuntimeAnimatorController runtimeController, AvatarMask avatarMask, int layer,
            double startTime, float fade, float speed, float weight,
            ref PlayableGraph graph, PlayableBase next,
            params PlayableGesture.Parameter[] parameters)
        {
            PlayableStateRTC state = new PlayableStateRTC(
                avatarMask, layer,
                fade, speed, weight
            );

            AnimatorControllerPlayable input1 = AnimatorControllerPlayable.Create(
                graph,
                runtimeController
            );
            
            foreach (PlayableGesture.Parameter parameter in parameters)
            {
                input1.SetFloat(parameter.id, parameter.value);
            }

            input1.SetTime(startTime);
            input1.SetSpeed(speed);

            state.Setup(ref graph, ref input1, next);
            return state;
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void Stop(float fadeOut)
        {
            base.Stop(fadeOut);
        }
    }
}
