namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;

    [AddComponentMenu("")]
    public class CharacterHandIK : MonoBehaviour
    {
        public enum Limb
        {
            LeftHand,
            RightHand,
            NearestHand,
            BothHands
        }

        private class Hand
        {
            public AvatarIKGoal handIK;
            public Transform hand;

            private float changeTime;
            private float currentWeight;
            private Vector3 currentPosition;

            private bool targetReach;
            private float targetTime;
            private Transform targetTransform;
            private Vector3 targetPosition;

            public Hand(Transform hand, AvatarIKGoal handIK)
            {
                this.handIK = handIK;
                this.hand = hand;

                this.changeTime = 0.0f;
                this.currentWeight = 0.0f;

                this.targetReach = false;
                this.targetTime = 1.0f;

                this.targetPosition = Vector3.zero;
            }

            public void Update(Animator animator)
            {
                if (this.targetTransform != null)
                {
                    this.targetPosition = this.targetTransform.position;
                }

                if (this.targetReach)
                {
                    float t = Time.time - this.changeTime;
                    t = Easing.QuadInOut(0.0f, 1.0f, t / this.targetTime);

                    this.currentWeight = Mathf.Lerp(
                        this.currentWeight, 
                        1.0f, 
                        t
                    );

                    this.currentPosition = Vector3.Slerp(
                        this.currentPosition, 
                        this.targetPosition,
                        Easing.QuadInOut(0.0f, 1.0f, t/this.targetTime)
                    );
                }
                else
                {
                    float t = Time.time - this.changeTime;
                    t = Easing.QuadInOut(0.0f, 1.0f, t / this.targetTime);

                    this.currentWeight = Mathf.Lerp(
                        this.currentWeight,
                        0.0f,
                        t
                    );

                    this.currentPosition = this.targetPosition;
                }

                animator.SetIKPositionWeight(this.handIK, this.currentWeight);
                animator.SetIKPosition(this.handIK, this.currentPosition);
            }

            public void Reach(Animator animator, Transform targetTransform, float duration)
            {
                this.targetReach = true;
                this.targetTime = Mathf.Max(duration, 0.01f);

                this.targetTransform = targetTransform;
                this.targetPosition = targetTransform.position;

                this.currentPosition = this.hand.position;
                this.changeTime = Time.time;
            }

            public void Unreach(float duration)
            {
                this.targetReach = false;
                this.targetTime = Mathf.Max(duration, 0.01f);
                this.changeTime = Time.time;
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Animator animator;
        private Character character;
        private CharacterAnimator characterAnimator;
        private CharacterController controller;

        private Hand handL;
        private Hand handR;

        public CharacterAnimator.EventIK eventBeforeIK = new CharacterAnimator.EventIK();
        public CharacterAnimator.EventIK eventAfterIK = new CharacterAnimator.EventIK();

        // INITIALIZERS: --------------------------------------------------------------------------

        public void Setup(Character character)
        {
            this.character = character;
            this.characterAnimator = this.character.GetCharacterAnimator();
            this.animator = this.characterAnimator.animator;
            this.controller = gameObject.GetComponentInParent<CharacterController>();
            if (this.animator == null || !this.animator.isHuman || this.controller == null) return;

            Transform handLTransform = this.animator.GetBoneTransform(HumanBodyBones.LeftHand);
            Transform handRTransform = this.animator.GetBoneTransform(HumanBodyBones.RightHand);

            this.handL = new Hand(handLTransform, AvatarIKGoal.LeftHand);
            this.handR = new Hand(handRTransform, AvatarIKGoal.RightHand);
        }

        // IK METHODS: ----------------------------------------------------------------------------

        private void OnAnimatorIK(int layerIndex)
        {
            if (this.animator == null || !this.animator.isHuman) return;
            if (this.character == null || this.characterAnimator == null) return;
            if (this.character.IsRagdoll()) return;

            this.eventBeforeIK.Invoke(layerIndex);

            if (!this.characterAnimator.useHandIK) return;

            UpdateHand(this.handL);
            UpdateHand(this.handR);

            this.eventAfterIK.Invoke(layerIndex);
        }

        private void UpdateHand(Hand hand)
        {
            hand.Update(this.animator);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Reach(Limb limb, Transform target, float duration)
        {
            switch (limb)
            {
                case Limb.LeftHand  : 
                    this.handL.Reach(this.animator, target, duration); 
                    break;

                case Limb.RightHand : 
                    this.handR.Reach(this.animator, target, duration); 
                    break;
                
                case Limb.NearestHand : 
                    this.NearestHand(target).Reach(this.animator, target, duration); 
                    break;

                case Limb.BothHands: 
                    this.handL.Reach(this.animator, target, duration); 
                    this.handR.Reach(this.animator, target, duration); 
                    break;
            }
        }

        public void LetGo(Limb limb, float duration)
        {
            switch (limb)
            {
                case Limb.LeftHand:
                    this.handL.Unreach(duration);
                    break;

                case Limb.RightHand:
                    this.handR.Unreach(duration);
                    break;

                default:
                    this.handL.Unreach(duration);
                    this.handR.Unreach(duration);
                    break;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Hand NearestHand(Transform target)
        {
            Vector3 tL = this.handL.hand.position;
            Vector3 tR = this.handR.hand.position;

            return (Vector3.Distance(tL, target.position) < Vector3.Distance(tR, target.position) 
                ? this.handL 
                : this.handR
            );
        }
    }
}