namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Core.Hooks;

	public abstract class IHPMonoBehaviour<TData> : MonoBehaviour where TData : IHPMonoBehaviour<TData>.IData
	{
		[System.Serializable]
		public abstract class IData
		{
			public Hotspot hotspot;
			public IHPMonoBehaviour<TData> instance;
			public bool enabled = false;

			public void Setup(Hotspot hotspot, GameObject instance)
			{
				this.hotspot = hotspot;
				this.instance = instance.GetComponent<IHPMonoBehaviour<TData>>();
			}
		}

        // PROPERTIES: ----------------------------------------------------------------------------

		public TData data;

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public static TData Create<THotspot>(Hotspot hotspot, TData data) where THotspot : IHPMonoBehaviour<TData>
		{
			GameObject instance = new GameObject(typeof(THotspot).ToString());
			instance.transform.SetParent(hotspot.transform, false);

			THotspot component = instance.AddComponent<THotspot>();
			component.data = data;
			component.ConfigureData(hotspot, instance);
			component.Initialize();
			return component.data;
		}

		private void ConfigureData(Hotspot hotspot, GameObject instance)
		{
			((IData)this.data).hotspot = hotspot;
			((IData)this.data).instance = instance.GetComponent<IHPMonoBehaviour<TData>>();
		}

		// ABSTRACT & VIRTUAL METHODS: ------------------------------------------------------------

		public abstract void Initialize();

		public virtual void HotspotMouseEnter() {}
		public virtual void HotspotMouseExit()  {}
        public virtual void HotspotMouseOver()  {}

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected bool IsWithinConstrainedRadius()
        {
            if (this.data.hotspot.trigger == null) return true;
            if (!this.data.hotspot.trigger.minDistance) return true;
            if (HookPlayer.Instance == null) return false;

            float distance = Vector3.Distance(
                HookPlayer.Instance.transform.position,
                transform.position
            );

            return (distance <= this.data.hotspot.trigger.minDistanceToPlayer);
        }
	}
}