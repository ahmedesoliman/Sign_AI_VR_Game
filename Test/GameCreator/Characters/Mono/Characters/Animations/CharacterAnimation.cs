namespace GameCreator.Characters
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Playables;

    public class CharacterAnimation
    {
        public enum Layer
        {
            Layer1,
            Layer2,
            Layer3,
        }

        public const float EPSILON = 0.01f;

        private const string GRAPH_NAME = "Character Graph";
        private const string ERR_NORTC = "No Runtime Controller found in Character Animation";

        // PROPERTIES: ----------------------------------------------------------------------------

        private CharacterAnimator characterAnimator;
        private RuntimeAnimatorController runtimeController;

        private PlayableGraph graph;
        private AnimatorControllerPlayable runtimeControllerPlayable;

        private AnimationMixerPlayable mixerGesturesOutput;
        private AnimationMixerPlayable mixerGesturesInput;
        private AnimationMixerPlayable mixerStatesOutput;
        private AnimationMixerPlayable mixerStatesInput;

        private List<PlayableGesture> gestures;
        private List<PlayableState> states;

        // INITIALIZERS: --------------------------------------------------------------------------

        public CharacterAnimation(CharacterAnimator characterAnimator, CharacterState defaultState = null)
        {
            this.characterAnimator = characterAnimator;
            this.runtimeController = defaultState != null
                ? defaultState.GetRuntimeAnimatorController()
                : characterAnimator.animator.runtimeAnimatorController;

            this.Setup();
        }

        public void OnDestroy()
        {
            if (!this.graph.Equals(null)) this.graph.Destroy();
        }

        public void ChangeRuntimeController(RuntimeAnimatorController controller = null)
        {
            if (controller != null) this.runtimeController = controller;
            this.Setup();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Update()
        {
            for (int i = this.states.Count - 1; i >= 0; --i)
            {
                bool remove = this.states[i].Update();
                if (remove)
                {
                    this.states[i].Destroy();
                    this.states.RemoveAt(i);
                }
            }

            for (int i = this.gestures.Count - 1; i >= 0; --i)
            {
                bool remove = this.gestures[i].Update();
                if (remove)
                {
                    this.gestures[i].Destroy();
                    this.gestures.RemoveAt(i);
                }
            }
        }

        // GESTURE METHODS: -----------------------------------------------------------------------

        public void PlayGesture(AnimationClip animationClip, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed)
        {
            this.StopGesture(fadeIn);
            this.gestures.Add(PlayableGestureClip.Create(
                animationClip, avatarMask,
                fadeIn, fadeOut, speed,
                ref this.graph,
                ref this.mixerGesturesInput,
                ref this.mixerGesturesOutput
            ));
        }

        public void CrossFadeGesture(AnimationClip animationClip, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed)
        {
            if (this.gestures.Count == 0)
            {
                this.gestures.Add(PlayableGestureClip.Create(
                    animationClip, avatarMask,
                    fadeIn, fadeOut, speed,
                    ref this.graph,
                    ref this.mixerGesturesInput,
                    ref this.mixerGesturesOutput
                ));
            }
            else
            {
                PlayableGesture previous = gestures[this.gestures.Count - 1];
                previous.StretchDuration(fadeIn);

                this.gestures.Add(PlayableGestureClip.CreateAfter(
                    animationClip, avatarMask,
                    fadeIn, fadeOut, speed,
                    ref this.graph,
                    previous
                ));
            }
        }
        
        public void CrossFadeGesture(RuntimeAnimatorController rtc, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed, 
            params PlayableGesture.Parameter[] parameters)
        {
            if (this.gestures.Count == 0)
            {
                this.gestures.Add(PlayableGestureRTC.Create(
                    rtc, avatarMask,
                    fadeIn, fadeOut, speed,
                    ref this.graph,
                    ref this.mixerGesturesInput,
                    ref this.mixerGesturesOutput,
                    parameters
                ));
            }
            else
            {
                PlayableGesture previous = gestures[this.gestures.Count - 1];
                previous.StretchDuration(fadeIn);

                this.gestures.Add(PlayableGestureRTC.CreateAfter(
                    rtc, avatarMask,
                    fadeIn, fadeOut, speed,
                    ref this.graph,
                    previous,
                    parameters
                ));
            }
        }

        public void StopGesture(float fadeOut)
        {
            for (int i = this.gestures.Count - 1; i >= 0; --i)
            {
                this.gestures[i].Stop(fadeOut);
            }
        }

        // STATE METHODS: -------------------------------------------------------------------------

        public void SetState(AnimationClip animationClip, AvatarMask avatarMask,
            float weight, float transition, float speed, int layer)
        {
            PlayableState prevPlayable;
            PlayableState nextPlayable;

            int insertIndex = this.GetSurroundingStates(layer,
                out prevPlayable,
                out nextPlayable
            );

            if (prevPlayable == null && nextPlayable == null)
            {
                this.states.Add(PlayableStateClip.Create(
                    animationClip, avatarMask, layer, 0f,
                    transition, speed, weight,
                    ref this.graph,
                    ref this.mixerStatesInput,
                    ref this.mixerStatesOutput
                ));
            }
            else if (prevPlayable != null)
            {
                if (prevPlayable.Layer == layer)
                {
                    prevPlayable.StretchDuration(transition);
                }

                this.states.Insert(insertIndex, PlayableStateClip.CreateAfter(
                    animationClip, avatarMask, layer, 0f,
                    transition, speed, weight,
                    ref this.graph,
                    prevPlayable
                ));
            }
            else if (nextPlayable != null)
            {
                this.states.Insert(insertIndex, PlayableStateClip.CreateBefore(
                    animationClip, avatarMask, layer, 0f,
                    transition, speed, weight,
                    ref this.graph,
                    nextPlayable
                ));
            }
        }

        public void SetState(CharacterState stateAsset, AvatarMask avatarMask,
            float weight, float transition, float speed, int layer, 
            params PlayableGesture.Parameter[] parameters)
        {
            PlayableState prevPlayable;
            PlayableState nextPlayable;

            int insertIndex = this.GetSurroundingStates(layer,
                out prevPlayable,
                out nextPlayable
            );

            if (prevPlayable == null && nextPlayable == null)
            {
                this.states.Add(PlayableStateCharacter.Create(
                    stateAsset, avatarMask, this, layer,
                    this.runtimeControllerPlayable.GetTime(),
                    transition, speed, weight,
                    ref this.graph,
                    ref this.mixerStatesInput,
                    ref this.mixerStatesOutput,
                    parameters
                ));
            }
            else if (prevPlayable != null)
            {
                if (prevPlayable.Layer == layer)
                {
                    prevPlayable.StretchDuration(transition);
                }

                this.states.Insert(insertIndex, PlayableStateCharacter.CreateAfter(
                    stateAsset, avatarMask, this, layer,
                    this.runtimeControllerPlayable.GetTime(),
                    transition, speed, weight,
                    ref this.graph,
                    prevPlayable,
                    parameters
                ));
            }
            else if (nextPlayable != null)
            {
                this.states.Insert(insertIndex, PlayableStateCharacter.CreateBefore(
                    stateAsset, avatarMask, this, layer,
                    this.runtimeControllerPlayable.GetTime(),
                    transition, speed, weight,
                    ref this.graph,
                    nextPlayable,
                    parameters
                ));
            }
        }

        public void SetState(RuntimeAnimatorController rtc, AvatarMask avatarMask,
            float weight, float transition, float speed, int layer, bool syncTime,
            params PlayableGesture.Parameter[] parameters)
        {
            PlayableState prevPlayable;
            PlayableState nextPlayable;

            int insertIndex = this.GetSurroundingStates(layer,
                out prevPlayable,
                out nextPlayable
            );

            if (prevPlayable == null && nextPlayable == null)
            {
                this.states.Add(PlayableStateRTC.Create(
                    rtc, avatarMask, layer,
                    syncTime ? this.runtimeControllerPlayable.GetTime() : 0f,
                    transition, speed, weight,
                    ref this.graph,
                    ref this.mixerStatesInput,
                    ref this.mixerStatesOutput,
                    parameters
                ));
            }
            else if (prevPlayable != null)
            {
                if (prevPlayable.Layer == layer)
                {
                    prevPlayable.StretchDuration(transition);
                }

                this.states.Insert(insertIndex, PlayableStateRTC.CreateAfter(
                    rtc, avatarMask, layer,
                    syncTime ? this.runtimeControllerPlayable.GetTime() : 0f,
                    transition, speed, weight,
                    ref this.graph,
                    prevPlayable,
                    parameters
                ));
            }
            else if (nextPlayable != null)
            {
                this.states.Insert(insertIndex, PlayableStateRTC.CreateBefore(
                    rtc, avatarMask, layer,
                    syncTime ? this.runtimeControllerPlayable.GetTime() : 0f,
                    transition, speed, weight,
                    ref this.graph,
                    nextPlayable,
                    parameters
                ));
            }
        }

        public void ResetState(float time, int layer)
        {
            for (int i = 0; i < this.states.Count; ++i)
            {
                if (this.states[i].Layer == layer)
                {
                    this.states[i].OnExitState();
                    this.states[i].Stop(time);
                }
            }
        }

        public void ChangeStateWeight(int layer, float weight)
        {
            for (int i = this.states.Count - 1; i >= 0; --i)
            {
                if (this.states[i].Layer == layer)
                {
                    this.states[i].SetWeight(weight);
                }
            }
        }

        public CharacterState GetState(int layer)
        {
            for (int i = this.states.Count - 1; i >= 0; --i)
            {
                if (this.states[i].Layer == layer)
                {
                    return this.states[i].CharacterState;
                }
            }

            return null;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Setup()
        {
            if (!this.runtimeController) throw new Exception(ERR_NORTC);

            if (this.characterAnimator.animator.playableGraph.IsValid())
            {
                this.characterAnimator.animator.playableGraph.Destroy();
            }

            if (this.graph.IsValid()) this.graph.Destroy();

            this.graph = PlayableGraph.Create(GRAPH_NAME);
            this.graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            AnimationPlayableOutput output = AnimationPlayableOutput.Create(
                this.graph, GRAPH_NAME,
                this.characterAnimator.animator
            );

            this.SetupSectionDefaultStates();
            this.SetupSectionStates();
            this.SetupSectionGestures();

            output.SetSourcePlayable(this.mixerGesturesOutput);
            output.SetSourceOutputPort(0);

            this.graph.Play();
        }

        private void SetupSectionDefaultStates()
        {
            this.runtimeControllerPlayable = AnimatorControllerPlayable.Create(
                this.graph,
                this.runtimeController
            );

            this.mixerStatesInput = AnimationMixerPlayable.Create(this.graph, 1, true);
            this.mixerStatesInput.ConnectInput(0, this.runtimeControllerPlayable, 0, 1f);
            this.mixerStatesInput.SetInputWeight(0, 1f);
        }

        private void SetupSectionStates()
        {
            this.states = new List<PlayableState>();

            this.mixerStatesOutput = AnimationMixerPlayable.Create(this.graph, 1, true);
            this.mixerStatesOutput.ConnectInput(0, this.mixerStatesInput, 0, 1f);
            this.mixerStatesOutput.SetInputWeight(0, 1f);
        }

        private void SetupSectionGestures()
        {
            this.gestures = new List<PlayableGesture>();

            this.mixerGesturesInput = AnimationMixerPlayable.Create(this.graph, 1, true);
            this.mixerGesturesInput.ConnectInput(0, this.mixerStatesOutput, 0, 1f);
            this.mixerGesturesInput.SetInputWeight(0, 1f);

            this.mixerGesturesOutput = AnimationMixerPlayable.Create(this.graph, 1, true);
            this.mixerGesturesOutput.ConnectInput(0, this.mixerGesturesInput, 0, 1f);
            this.mixerGesturesOutput.SetInputWeight(0, 1f);
        }

        private int GetSurroundingStates(int layer, out PlayableState prev, out PlayableState next)
        {
            prev = null;
            next = null;

            for (int i = 0; i < this.states.Count; ++i)
            {
                if (this.states[i].Layer <= layer)
                {
                    prev = this.states[i];
                    return i;
                }

                next = this.states[i];
            }

            return 0;
        }
    }
}
