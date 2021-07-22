namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core;

    [AddComponentMenu("")]
	public class ConditionCharacterBusy : ICondition
	{
        public enum Busy
        {
            IsBusy,
            IsNotBusy
        }

        public TargetCharacter character = new TargetCharacter();
        public Busy state = Busy.IsBusy;

		// EXECUTABLE: ----------------------------------------------------------------------------

		public override bool Check(GameObject target)
		{
            Character charTarget = this.character.GetCharacter(target);
            if (charTarget == null) return false;

			bool isBusy = charTarget.characterLocomotion.isBusy;
            switch (this.state)
            {
				case Busy.IsBusy: return isBusy;
				case Busy.IsNotBusy: return !isBusy;
            }

			return false;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    public static new string NAME = "Characters/Character Busy";
        private const string NODE_TITLE = "Character {0} {1}";

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
                NODE_TITLE,
                this.character,
                UnityEditor.ObjectNames.NicifyVariableName(this.state.ToString())
			);
		}

		#endif
	}
}
