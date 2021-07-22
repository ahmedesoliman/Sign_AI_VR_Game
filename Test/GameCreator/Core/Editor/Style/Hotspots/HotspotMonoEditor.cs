namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	public abstract class HotspotSubEditors
	{
		// COMMON PAINT METHOD: ----------------------------------------------------------------------------------------

		private static void PaintMessage()
		{
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("These are not the Components you are looking for", MessageType.Warning);
			EditorGUILayout.Space();
		}

		[CustomEditor(typeof(HPCursor))]
		public class HPCursorEditor : Editor 
		{
			public override void OnInspectorGUI ()
			{
				HotspotSubEditors.PaintMessage();
			}
		}

		[CustomEditor(typeof(HPProximity))]
		public class HPProximityEditor : Editor 
		{
			public override void OnInspectorGUI ()
			{
				HotspotSubEditors.PaintMessage();
			}
		}

		[CustomEditor(typeof(HPHeadTrack))]
		public class HPHeadTrackEditor : Editor 
		{
			public override void OnInspectorGUI ()
			{
				HotspotSubEditors.PaintMessage();
			}
		}
	}
}