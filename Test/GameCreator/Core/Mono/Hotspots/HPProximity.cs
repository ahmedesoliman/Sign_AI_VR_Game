namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class HPProximity : IHPMonoBehaviour<HPProximity.Data>
	{
		[System.Serializable]
		public class Data : IHPMonoBehaviour<HPProximity.Data>.IData
		{
			public GameObject prefab;
            public Vector3 offset;
			private GameObject prefabInstance;

			[Range(0.0f, 20.0f)]
			public float radius = 1.0f;

			public bool targetPlayer = true;
			public GameObject target;

			public void ChangeState(Transform parent, bool state)
			{
				if (this.prefab == null) return;
				if (this.prefabInstance == null)
				{
					this.prefabInstance = Instantiate(this.prefab, parent);
					this.prefabInstance.transform.localPosition = this.offset;
					this.prefabInstance.transform.localRotation = Quaternion.identity;
					this.prefabInstance.transform.localScale = Vector3.one;
				}

				this.prefabInstance.SetActive(state);
			}
		}

		// OVERRIDE METHODS: ----------------------------------------------------------------------

		public override void Initialize()
		{
			if (!this.data.enabled) return;

			gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
			sphereCollider.isTrigger = true;
			sphereCollider.radius = this.data.radius;
		}

        // TRIGGER METHODS: -----------------------------------------------------------------------

        private void OnTriggerEnter(Collider collider) 
		{ 
			if (!this.data.enabled || this.data.prefab == null) return;
			if (!this.HotspotIndicatorIsTarget(collider)) return;

			this.data.ChangeState(transform, true);
		}

		private void OnTriggerExit(Collider collider)  
		{ 
			if (!this.data.enabled || this.data.prefab == null) return;
			if (!this.HotspotIndicatorIsTarget(collider)) return;

			this.data.ChangeState(transform, false);
		}

		private bool HotspotIndicatorIsTarget(Collider collider)
		{
			if (this.data.targetPlayer && 
				collider.gameObject.GetInstanceID() == Hooks.HookPlayer.Instance.gameObject.GetInstanceID())
			{
				return true;
			}

            if (!this.data.targetPlayer &&
				collider.gameObject.GetInstanceID() == this.data.target.GetInstanceID()) 
			{
				return true;
			}

			return false;
		}
	}
}