namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Core;
    using GameCreator.Variables;

    [AddComponentMenu("")]
	public class ActionCharacterMount : IAction
	{
        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);

        [Space]
        public bool mounted = true;

       public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character _character = this.character.GetCharacter(target);
            if (_character == null) return true;

            CharacterLocomotion locomotion = _character.characterLocomotion;
            CharacterController controller = locomotion.characterController;

            switch (this.mounted)
            {
                case true:
                    _character.enabled = false;
                    if (controller != null) controller.detectCollisions = false;
                    break;

                case false:
                    if (controller != null) controller.detectCollisions = true;
                    _character.enabled = true;
                    break;
            }

            return true;
        }

		#if UNITY_EDITOR
        public static new string NAME = "Character/Character Mount";
        private const string NODE_TITLE = "Set {0} in {1} mode";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.character,
                this.mounted ? "Mounted" : "Not Mounted"
            );
        }
        #endif
    }
}
