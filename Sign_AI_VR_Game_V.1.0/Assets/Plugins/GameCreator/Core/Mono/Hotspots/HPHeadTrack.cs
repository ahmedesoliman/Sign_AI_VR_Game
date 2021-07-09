namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Characters;
	using GameCreator.Core.Hooks;

	[AddComponentMenu("")]
	public class HPHeadTrack : IHPMonoBehaviour<HPHeadTrack.Data>
	{
        private const float HEAD_SPEED = 0.5f;

		[System.Serializable]
		public class Data : IHPMonoBehaviour<HPHeadTrack.Data>.IData
		{
            public List<TargetCharacter> characters = new List<TargetCharacter>();
			[Range(0f, 20f)] public float radius = 5.0f;
		}

		// OVERRIDE METHODS: ----------------------------------------------------------------------

		public override void Initialize()
		{
			if (!this.data.enabled) return;

            gameObject.layer = Physics.IgnoreRaycastLayer;
			SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
			sphereCollider.isTrigger = true;
			sphereCollider.radius = this.data.radius;
		}

		// TRIGGER METHODS: -----------------------------------------------------------------------

		private void OnTriggerEnter(Collider collider) 
		{ 
			if (!this.data.enabled) return;

			int numCharacters = this.data.characters.Count;
			for (int i = 0; i < numCharacters; ++i)
			{
				CharacterHeadTrack characterInfo = this.HotspotIndicatorIsTarget(collider.gameObject, i);
				if (characterInfo == null) continue;

				characterInfo.Track(transform, HEAD_SPEED);
			}
		}

		private void OnTriggerExit(Collider collider)  
		{ 
			if (!this.data.enabled) return;

			int numCharacters = this.data.characters.Count;
			for (int i = 0; i < numCharacters; ++i)
			{
				CharacterHeadTrack characterInfo = this.HotspotIndicatorIsTarget(collider.gameObject, i);
				if (characterInfo == null) continue;

				characterInfo.Untrack();
			}
		}

		private CharacterHeadTrack HotspotIndicatorIsTarget(GameObject collider, int charsIndex)
		{
            Character character = this.data.characters[charsIndex].GetCharacter(gameObject);
            if (character != null)
            {
                if (collider == character.gameObject)
                {
                    return character.GetHeadTracker();
                }
            }

            return null;
		}
	}
}