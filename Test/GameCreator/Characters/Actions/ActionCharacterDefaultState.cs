namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Core;

	[AddComponentMenu("")]
	public class ActionCharacterDefaultState : IAction
	{
        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);
        [Space] public CharacterState state;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character _character = this.character.GetCharacter(target);
            if (_character == null) return true;

            CharacterAnimator _animator = _character.GetCharacterAnimator();
            if (_animator == null) return true;

            if (this.state == null) return true;
            _animator.ResetControllerTopology(this.state.GetRuntimeAnimatorController());

            return true;
        }

        #if UNITY_EDITOR

        public static new string NAME = "Character/Character Default State";
        private const string NODE_TITLE = "Set {0} default state to {1}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.character,
                this.state == null ? "(none)" : this.state.name
            );
        }

        #endif
    }
}
