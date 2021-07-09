namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;

	[CustomEditor(typeof(Hotspot))]
	public class HotspotEditor : Editor 
	{
		private const string MSG_REQUIRE_COLLIDER = "A Collider component is required.";
		private const float ANIM_BOOL_SPEED = 3.0f;
		private const string PROP_CURSOR = "cursorData";
		private const string PROP_PROXIM = "proximityData";
		private const string PROP_HEADTR = "headTrackData";
		private const string PROP_ENABLED = "enabled";

		private const string PROP_PROXIM_RADIUS = "radius";
		private const string PROP_HEADTR_RADIUS = "radius";

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private Hotspot hotspot;
		private SerializedProperty spCursor; private SerializedProperty spCursorEnabled; private AnimBool cursorState;
		private SerializedProperty spProxim; private SerializedProperty spProximEnabled; private AnimBool proximState;
		private SerializedProperty spHeadTr; private SerializedProperty spHeadTrEnabled; private AnimBool headTrState;

		private SerializedProperty spProximRadius;
		private SerializedProperty spHeadTrRadius;

		// INITIALIZE: -------------------------------------------------------------------------------------------------

		private void OnEnable()
		{
            if (target == null || serializedObject == null) return;
			this.hotspot = (Hotspot)target;

			this.spCursor = serializedObject.FindProperty(PROP_CURSOR);
			this.spProxim = serializedObject.FindProperty(PROP_PROXIM);
			this.spHeadTr = serializedObject.FindProperty(PROP_HEADTR);

			this.spCursorEnabled = this.spCursor.FindPropertyRelative(PROP_ENABLED);
			this.spProximEnabled = this.spProxim.FindPropertyRelative(PROP_ENABLED);
			this.spHeadTrEnabled = this.spHeadTr.FindPropertyRelative(PROP_ENABLED);

			this.spProximRadius = this.spProxim.FindPropertyRelative(PROP_PROXIM_RADIUS);
			this.spHeadTrRadius = this.spHeadTr.FindPropertyRelative(PROP_HEADTR_RADIUS);

			this.SetupAnimBool(ref this.cursorState, this.spCursorEnabled.boolValue);
			this.SetupAnimBool(ref this.proximState, this.spProximEnabled.boolValue);
			this.SetupAnimBool(ref this.headTrState, this.spHeadTrEnabled.boolValue);
		}

		// GUI METHODS: ------------------------------------------------------------------------------------------------

		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			EditorGUILayout.Space();

			this.PaintItem(this.spCursor, this.spCursorEnabled, this.cursorState, "Cursor", true);
			this.PaintItem(this.spProxim, this.spProximEnabled, this.proximState, "Proximity Hint", false);
			this.PaintItem(this.spHeadTr, this.spHeadTrEnabled, this.headTrState, "Head Look At", false);

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}

		private void PaintItem(SerializedProperty property, SerializedProperty enabled, AnimBool state, 
			string title, bool requiresCollider)
		{
			this.PaintHeader(enabled, state, title);
			this.PaintContent(property, state, requiresCollider);
		}

		private void PaintHeader(SerializedProperty spEnabled, AnimBool state, string title)
		{
			EditorGUILayout.BeginHorizontal();

			GUIStyle style = (spEnabled.boolValue
				? CoreGUIStyles.GetToggleButtonOn()
				: CoreGUIStyles.GetToggleButtonOff()
			);

			bool buttonPressed = GUILayout.Button(title, style);
			Rect buttonRect = GUILayoutUtility.GetLastRect();

			if (buttonPressed)
			{
				spEnabled.boolValue = !spEnabled.boolValue;
				state.target = spEnabled.boolValue;
			}

			if (UnityEngine.Event.current.type == EventType.Repaint)
			{
				Rect toggleRect = new Rect(
					buttonRect.x + 5f,
					buttonRect.y + (buttonRect.height/2 - 8),
					12,
					12
				);

				bool isOn = spEnabled.boolValue;
				EditorStyles.toggle.Draw(toggleRect, GUIContent.none, false, false, isOn, isOn);
			}

			EditorGUILayout.EndHorizontal();
		}

		private void PaintContent(SerializedProperty property, AnimBool state, bool requiresCollider)
		{
			using (var group = new EditorGUILayout.FadeGroupScope (state.faded))
			{
				if (group.visible)
				{
					EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
					EditorGUILayout.PropertyField(property);
					if (requiresCollider && this.hotspot.gameObject.GetComponent<Collider>() == null)
					{
						EditorGUILayout.HelpBox(MSG_REQUIRE_COLLIDER, MessageType.Warning);
					}
					EditorGUILayout.EndVertical();
				}
			}
		}

		// SCENE UI: ---------------------------------------------------------------------------------------------------

		private void OnSceneGUI()
		{
			if (this.spProximEnabled.boolValue)
			{
				serializedObject.Update();

				Handles.color = Color.white;
				this.spProximRadius.floatValue = Handles.RadiusHandle(
					Quaternion.identity, 
					this.hotspot.transform.position, 
					this.spProximRadius.floatValue
				);

				this.spProximRadius.floatValue = Mathf.Clamp(this.spProximRadius.floatValue, 0.0f, 20.0f);

				serializedObject.ApplyModifiedPropertiesWithoutUndo();
			}

			if (this.spHeadTrEnabled.boolValue)
			{
				serializedObject.Update();

				Handles.color = Color.cyan;
				this.spHeadTrRadius.floatValue = Handles.RadiusHandle(
					Quaternion.identity, 
					this.hotspot.transform.position, 
					this.spHeadTrRadius.floatValue
				);

				this.spHeadTrRadius.floatValue = Mathf.Clamp(this.spHeadTrRadius.floatValue, 0.0f, 20.0f);

				serializedObject.ApplyModifiedPropertiesWithoutUndo();
			}
		}

		// PRIVATE METHODS: --------------------------------------------------------------------------------------------

		private void SetupAnimBool(ref AnimBool animBool, bool state)
		{
			animBool = new AnimBool(state, () => { this.Repaint(); });
			animBool.speed = ANIM_BOOL_SPEED;
		}

		// HIERARCHY CONTEXT MENU: -------------------------------------------------------------------------------------

		[MenuItem("GameObject/Game Creator/Other/Hotspot", false, 0)]
		public static void CreateHotspot()
		{
			GameObject trigger = CreateSceneObject.Create("Hotspot");
			SphereCollider collider = trigger.AddComponent<SphereCollider>();
			collider.isTrigger = true;
			trigger.AddComponent<Hotspot>();
		}
	}
}