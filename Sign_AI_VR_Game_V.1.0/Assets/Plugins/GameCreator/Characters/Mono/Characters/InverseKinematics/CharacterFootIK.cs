namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("")]
    public class CharacterFootIK : MonoBehaviour
    {
        private const float FOOT_OFFSET_Y = 0.1f;
        private const float SMOOTH_POSITION = 0.1f;
        private const float SMOOTH_ROTATION = 0.1f;
        private const float SMOOTH_WEIGHT   = 0.2f;
        private const float BODY_MAX_INCLINE = 10f;

        private static readonly int IK_L_FOOT = Animator.StringToHash("IK_leftFoot");
        private static readonly int IK_R_FOOT = Animator.StringToHash("IK_rightFoot");

        private class Foot
        {
            public bool hit;
            public int weightID;
            public AvatarIKGoal footIK;
            public Transform foot;

            public float height = 0.0f;
            public Vector3 normal = Vector3.up;

            public Foot(Transform foot, AvatarIKGoal footIK, int weightID)
            {
                this.hit = false;
                this.weightID = weightID;
                this.footIK = footIK;
                this.foot = foot;
            }

            public float GetWeight(Animator animator)
            {
                return animator.GetFloat(this.weightID);
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Animator animator;
        private Character character;
        private CharacterAnimator characterAnimator;
        private CharacterController controller;

        private Foot leftFoot;
        private Foot rightFoot;

        private float defaultOffset;
        private float speedPosition;

        public CharacterAnimator.EventIK eventBeforeIK = new CharacterAnimator.EventIK();
        public CharacterAnimator.EventIK eventAfterIK = new CharacterAnimator.EventIK();

        private RaycastHit[] hitBuffer = new RaycastHit[1];

        // INITIALIZERS: --------------------------------------------------------------------------

        public void Setup(Character character)
        {
            this.character = character;
            this.characterAnimator = this.character.GetCharacterAnimator();
            this.animator = this.characterAnimator.animator;
            this.controller = gameObject.GetComponentInParent<CharacterController>();
            if (this.animator == null || !this.animator.isHuman || this.controller == null) return;

            Transform lFoot = this.animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            Transform rFoot = this.animator.GetBoneTransform(HumanBodyBones.RightFoot);

            this.leftFoot = new Foot(lFoot, AvatarIKGoal.LeftFoot, IK_L_FOOT);
            this.rightFoot = new Foot(rFoot, AvatarIKGoal.RightFoot, IK_R_FOOT);

            this.defaultOffset = transform.localPosition.y;
        }

        private void LateUpdate()
        {
            if (this.character == null || this.characterAnimator == null) return;
            if (!this.characterAnimator.useFootIK) return;
            if (this.character.IsRagdoll()) return;

            this.WeightCompensationPosition();
        }

        // IK METHODS: ----------------------------------------------------------------------------

        private void OnAnimatorIK(int layerIndex)
        {
            if (this.animator == null || !this.animator.isHuman) return;
            if (this.character == null || this.characterAnimator == null) return;
            if (this.character.IsRagdoll()) return;

            this.eventBeforeIK.Invoke(layerIndex);

            if (!this.characterAnimator.useFootIK) return;

            if (this.controller.isGrounded)
            {
                UpdateFoot(this.leftFoot);
                UpdateFoot(this.rightFoot);

                SetFoot(this.leftFoot);
                SetFoot(this.rightFoot);
            }

            this.eventAfterIK.Invoke(layerIndex);
        }

        private void UpdateFoot(Foot foot)
        {
            float rayMagnitude = this.controller.height/2.0f;
            Vector3 rayPosition = foot.foot.position;
            rayPosition.y += rayMagnitude/2.0f;

            int layerMask = this.characterAnimator.footLayerMask;
            QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;

            int hitCount = Physics.RaycastNonAlloc(
                rayPosition, -Vector3.up, hitBuffer,
                rayMagnitude, layerMask, queryTrigger
            );

            if (hitCount > 0)
            {
                foot.hit = true;
                foot.height = hitBuffer[0].point.y;
                foot.normal = hitBuffer[0].normal;
            }
            else
            {
                foot.hit = false;
                foot.height = foot.foot.position.y;
            }
        }

        private void SetFoot(Foot foot)
        {
            float weight = foot.GetWeight(this.animator);

            if (foot.hit)
            {
                Vector3 rotationAxis = Vector3.Cross(Vector3.up, foot.normal);
                float angle = Vector3.Angle(transform.up, foot.normal);
                Quaternion rotation = Quaternion.AngleAxis(angle * weight, rotationAxis);

                this.animator.SetIKRotationWeight(foot.footIK, weight);
                this.animator.SetIKRotation(foot.footIK, rotation * this.animator.GetIKRotation(foot.footIK));

                float baseHeight = this.transform.position.y - FOOT_OFFSET_Y;
                float animHeight = (foot.foot.position.y - baseHeight) / (rotation * Vector3.up).y;
                Vector3 position = new Vector3(
                    foot.foot.position.x, 
                    Mathf.Max(foot.height, baseHeight) + animHeight, 
                    foot.foot.position.z
                );

                this.animator.SetIKPositionWeight(foot.footIK, weight);
                this.animator.SetIKPosition(foot.footIK, position);
            }
            else
            {
                this.animator.SetIKPositionWeight(foot.footIK, weight);
                this.animator.SetIKRotationWeight(foot.footIK, weight);
            }
        }

        // WEIGHT COMPENSATION: -------------------------------------------------------------------

        private void WeightCompensationPosition()
        {
            float position = this.controller.transform.position.y + this.defaultOffset;

            if (this.controller.isGrounded)
            {
                float targetHeight = transform.position.y;

                if (this.leftFoot.hit && this.leftFoot.height < targetHeight) targetHeight = this.leftFoot.height;
                if (this.rightFoot.hit && this.rightFoot.height < targetHeight) targetHeight = this.rightFoot.height;

                targetHeight += FOOT_OFFSET_Y;
                if (position > targetHeight)
                {
                    float maxDistance = this.controller.transform.position.y + this.defaultOffset;
                    maxDistance -= this.controller.height * 0.075f;
                    position = Mathf.Max(targetHeight, maxDistance);
                }
            }

            float yAxis = Mathf.SmoothDamp(
                transform.position.y,
                position,
                ref this.speedPosition,
                SMOOTH_POSITION
            );

            transform.position = new Vector3(transform.position.x, yAxis, transform.position.z);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Vector3 GetControllerBase()
        {
            Vector3 position = this.controller.transform.TransformPoint(this.controller.center);
            position.y -= (this.controller.height * 0.5f - this.controller.radius);

            return position;
        }
    }
}