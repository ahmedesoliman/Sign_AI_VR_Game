namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Characters;
    using GameCreator.Variables;

    [AddComponentMenu("")]
    public class IgniterOnLand : Igniter 
	{
		#if UNITY_EDITOR
		public new static string NAME = "Character/On Land";
        #endif

        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);

        [Space][VariableFilter(Variable.DataType.Number)]
        public VariableProperty storeVerticalSpeed = new VariableProperty();

        private void Start()
        {
            Character target = this.character.GetCharacter(gameObject);
            if (target != null)
            {
                target.onLand.AddListener(this.OnLand);
            }
        }

        private void OnLand(float verticalSpeed)
        {
            Character instance = this.character.GetCharacter(gameObject);

            this.storeVerticalSpeed.Set(verticalSpeed, instance.gameObject);
            base.ExecuteTrigger(instance.gameObject);
        }
    }
}