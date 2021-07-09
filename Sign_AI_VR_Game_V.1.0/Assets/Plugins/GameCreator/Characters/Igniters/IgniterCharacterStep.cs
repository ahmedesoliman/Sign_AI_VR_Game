namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Core;

    [AddComponentMenu("")]
    public class IgniterCharacterStep : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "Character/On Step";
        #endif

        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);
        public CharacterLocomotion.STEP step = CharacterLocomotion.STEP.Any;

        private void Start()
        {
            Character target = this.character.GetCharacter(gameObject);
            if (target != null)
            {
                target.onStep.AddListener(this.OnStep);
            }
        }

        private void OnDestroy()
        {
            if (this.isExitingApplication) return;
            Character target = this.character.GetCharacter(gameObject);
            if (target != null)
            {
                target.onStep.RemoveListener(this.OnStep);
            }
        }

        private void OnStep(CharacterLocomotion.STEP stepType)
        {
            if (this.step != CharacterLocomotion.STEP.Any && stepType != CharacterLocomotion.STEP.Any)
            {
                if (this.step != stepType) return;
            }

            Character target = this.character.GetCharacter(gameObject);
            this.ExecuteTrigger(target.gameObject);
        }
	}
}