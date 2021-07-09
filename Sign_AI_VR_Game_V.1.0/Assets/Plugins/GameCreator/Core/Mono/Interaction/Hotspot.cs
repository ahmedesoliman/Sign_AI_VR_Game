namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("Game Creator/Hotspot", 0)]
	public class Hotspot : MonoBehaviour 
	{
		public HPCursor.Data cursorData;
		public HPProximity.Data proximityData;
		public HPHeadTrack.Data headTrackData;

        public Trigger trigger;

		// INITIALIZE: ----------------------------------------------------------------------------

		private void Awake()
		{
            this.trigger = GetComponent<Trigger>();

			this.cursorData = HPCursor.Create<HPCursor>(this, this.cursorData);
			this.proximityData = HPProximity.Create<HPProximity>(this, this.proximityData);
			this.headTrackData = HPHeadTrack.Create<HPHeadTrack>(this, this.headTrackData);
		}

		// INTERACTION METHODS: -------------------------------------------------------------------

		private void OnMouseEnter() 
		{ 
			if (this.cursorData.enabled) this.cursorData.instance.HotspotMouseEnter();
		}

		private void OnMouseExit() 
		{ 
			if (this.cursorData.enabled) this.cursorData.instance.HotspotMouseExit();
		}

        private void OnMouseOver()
        {
            if (this.cursorData.enabled) this.cursorData.instance.HotspotMouseOver();
        }

        private void OnDestroy()
		{
			if (this.cursorData.enabled) this.cursorData.instance.HotspotMouseExit();
		}

		// GIZMO METHODS: -------------------------------------------------------------------------

		private void OnDrawGizmos()
		{
			Gizmos.DrawIcon(transform.position, "GameCreator/Hotspot/hotspot", true);
		}
	}
}