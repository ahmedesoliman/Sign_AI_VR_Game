namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;
    using UnityEditor;

    public class LocomotionSystemRotation : ILocomotionSystem
    {
        private const float ERROR_MARGIN = 0.1f;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected Vector3 desiredDirection = Vector3.zero;

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override CharacterLocomotion.LOCOMOTION_SYSTEM Update()
        {
            base.Update();

            if (this.characterLocomotion.navmeshAgent != null)
            {
                this.characterLocomotion.navmeshAgent.updatePosition = false;
                this.characterLocomotion.navmeshAgent.updateUpAxis = false;
            }

            Quaternion targetRotation = this.UpdateRotation(this.desiredDirection);
            Transform charTransform = this.characterLocomotion.characterController.transform;

            Vector3 charForward = charTransform.TransformDirection(Vector3.forward);
            Vector3 charRight = charTransform.TransformDirection(Vector3.right);

            float difference = Vector3.Dot(charForward, this.desiredDirection);

            if (Mathf.Abs(difference) < ERROR_MARGIN) this.pivotSpeed = 0f;
            else
            {
                this.pivotSpeed = Vector3.Dot(charRight, this.desiredDirection);
                if (difference < 0f) this.pivotSpeed = this.pivotSpeed >= 0 ? 1f : -1f;
            }

            this.characterLocomotion.characterController.transform.rotation = targetRotation;

            Vector3 targetDirection = Vector3.up * this.characterLocomotion.verticalSpeed;
            this.characterLocomotion.characterController.Move(targetDirection);

            if (this.characterLocomotion.navmeshAgent != null &&
                this.characterLocomotion.navmeshAgent.isActiveAndEnabled)
            {
                this.characterLocomotion.navmeshAgent.enabled = false;
            }

            return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
        }

        public override void OnDestroy()
        {
            return;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetDirection(Vector3 direction, TargetRotation rotation = null)
        {
            this.desiredDirection = direction;
        }
    }
}