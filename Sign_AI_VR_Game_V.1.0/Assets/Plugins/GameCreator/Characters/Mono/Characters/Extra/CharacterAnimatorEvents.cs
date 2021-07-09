namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Core;

    [AddComponentMenu("")]
	public class CharacterAnimatorEvents : MonoBehaviour 
	{
		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private Character character;

		// INITIALIZER: ------------------------------------------------------------------------------------------------

		public void Setup(Character character)
		{
			this.character = character;
		}

		// EVENTS: -----------------------------------------------------------------------------------------------------

		public void Event_KeepPosition(float timeout)
		{
            CoroutinesManager.Instance.StartCoroutine(
                this.CoroutineTrigger(CharacterLocomotion.ANIM_CONSTRAINT.KEEP_POSITION, timeout)
            );
		}

		public void Event_KeepMovement(float timeout)
		{
            CoroutinesManager.Instance.StartCoroutine(
                this.CoroutineTrigger(CharacterLocomotion.ANIM_CONSTRAINT.KEEP_MOVEMENT, timeout)
            );
		}

        public void Event_BothSteps(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight < 0.4f) return;
            if (this.character.onStep != null) this.character.onStep.Invoke(CharacterLocomotion.STEP.Any);
        }

        public void Event_LeftStep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight < 0.4f) return;
            if (this.character.onStep != null) this.character.onStep.Invoke(CharacterLocomotion.STEP.Left);
        }

        public void Event_RightStep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight < 0.4f) return;
            if (this.character.onStep != null) this.character.onStep.Invoke(CharacterLocomotion.STEP.Right);
        }

        // COROUTINES: -------------------------------------------------------------------------------------------------

        private IEnumerator CoroutineTrigger(CharacterLocomotion.ANIM_CONSTRAINT constraint, float timeout)
		{
			this.character.characterLocomotion.SetAnimatorConstraint(constraint);

			WaitForSeconds coroutine = new WaitForSeconds(timeout);
			yield return coroutine;

			this.character.characterLocomotion.SetAnimatorConstraint(CharacterLocomotion.ANIM_CONSTRAINT.NONE);
		}
	}
}