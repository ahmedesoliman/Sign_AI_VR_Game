namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Characters;

    public class CharacterState : ScriptableObject
    {
        public enum StateType
        {
            Simple,
            Locomotion,
            Other
        }

        public StateType type = StateType.Simple;

        public AnimatorOverrideController rtcSimple;
        public AnimatorOverrideController rtcLocomotion;
        public RuntimeAnimatorController rtcOther;

        public AnimationClip enterClip;
        public AvatarMask enterAvatarMask;

        public AnimationClip exitClip;
        public AvatarMask exitAvatarMask;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public RuntimeAnimatorController GetRuntimeAnimatorController()
        {
            switch (this.type)
            {
                case StateType.Simple : return this.rtcSimple;
                case StateType.Locomotion : return this.rtcLocomotion;
                case StateType.Other : return this.rtcOther;
            }

            return null;
        }

        public void StartState(CharacterAnimation character)
        {
            if (character == null) return;
            if (this.enterClip == null) return;

            character.CrossFadeGesture(this.enterClip, this.enterAvatarMask, 0.15f, 0.15f, 1.0f);
        }

        public void ExitState(CharacterAnimation character)
        {
            if (character == null) return;
            if (this.exitClip == null) return;

            character.CrossFadeGesture(this.exitClip, this.exitAvatarMask, 0.15f, 0.15f, 1.0f);
        }
    }
}