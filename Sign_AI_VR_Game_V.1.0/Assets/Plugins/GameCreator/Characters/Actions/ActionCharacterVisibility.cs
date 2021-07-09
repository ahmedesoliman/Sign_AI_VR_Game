namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
    using GameCreator.Core;
    using GameCreator.Variables;
    using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class ActionCharacterVisibility : IAction
	{
        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Invoker);

        [Space]
        public BoolProperty visible = new BoolProperty(true);

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character instance = this.character.GetCharacter(target);
            if (instance == null) return true;

            CharacterAnimator animator = instance.GetCharacterAnimator();
            if (animator == null) return true;

            bool value = this.visible.GetValue(target);
            animator.SetVisibility(value);

            return true;
        }

        #if UNITY_EDITOR

        public static new string NAME = "Character/Character Visible";
        private const string NODE_TITLE = "Character {0} is visible {1}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this.character, this.visible);
        }

        #endif
    }
}
