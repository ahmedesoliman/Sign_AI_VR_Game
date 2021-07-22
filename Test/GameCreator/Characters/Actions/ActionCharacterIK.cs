namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Core;

	[AddComponentMenu("")]
	public class ActionCharacterIK : IAction
	{
        public enum Section
        {
            Head,
            Hands,
            Feet
        }

        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);

        [Space] public Section part = Section.Head;
        public bool enable = false;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character instance = this.character.GetCharacter(target);
            if (instance == null) return true;

            CharacterAnimator animator = instance.GetCharacterAnimator();
            if (animator == null) return true;

            switch (this.part)
            {
                case Section.Head: animator.useSmartHeadIK = this.enable; break;
                case Section.Hands: animator.useHandIK = this.enable; break;
                case Section.Feet: animator.useFootIK = this.enable; break;
            }

            return true;
        }

		#if UNITY_EDITOR

        public static new string NAME = "Character/Inverse Kinematics";
        private const string NODE_TITLE = "Set {0} {1} IK as {2}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this.character, this.part, enable.ToString());
        }

        #endif
    }
}
