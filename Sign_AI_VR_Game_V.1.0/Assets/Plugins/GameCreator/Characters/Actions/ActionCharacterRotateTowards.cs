namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Core;

	[AddComponentMenu("")]
	public class ActionCharacterRotateTowards : IAction
	{
        private static readonly Vector3 PLANE = new Vector3(1, 0, 1);

        public TargetCharacter character = new TargetCharacter();
        public TargetPosition target = new TargetPosition();

        private float duration;
        private bool wasControllable;
		private Character cacheCharacter;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            this.cacheCharacter = this.character.GetCharacter(target);
            if (this.cacheCharacter == null) return true;

            this.wasControllable = this.cacheCharacter.IsControllable();
            this.cacheCharacter.characterLocomotion.SetIsControllable(false);

            Vector3 rotationDirection = (
                this.target.GetPosition(target) - this.cacheCharacter.gameObject.transform.position
            );

            rotationDirection = Vector3.Scale(rotationDirection, PLANE).normalized;
            this.duration = Vector3.Angle(
                this.cacheCharacter.transform.TransformDirection(Vector3.forward),
                rotationDirection
            ) / this.cacheCharacter.characterLocomotion.angularSpeed;

            this.cacheCharacter.characterLocomotion.SetRotation(rotationDirection);

			return false;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index, params object[] parameters)
        {

            WaitForSeconds wait = new WaitForSeconds(this.duration);
            yield return wait;

            if (this.cacheCharacter != null)
            {
                CharacterLocomotion locomotion = this.cacheCharacter.characterLocomotion;
                locomotion.SetIsControllable(this.wasControllable);
            }
            
            yield return 0;
        }

        #if UNITY_EDITOR

        public static new string NAME = "Character/Character Rotate Towards";
        private const string NODE_TITLE = "Rotate {0} towards {1}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this.character, this.target);
        }

        #endif
    }
}
